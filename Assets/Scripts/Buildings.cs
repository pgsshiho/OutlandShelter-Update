using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Buildings : MonoBehaviour
{
    [System.Serializable]
    public struct Resource
    {
        public int watt;
        public int wooden;
        public int steel;
    }
    public Resource price;
    [SerializeField] private GameObject buildObject;
    private Personal_resource resource;
    [SerializeField] private GameObject buildUI;

    [SerializeField] private TextMeshProUGUI wattText;
    [SerializeField] private TextMeshProUGUI woodenText;
    [SerializeField] private TextMeshProUGUI steelText;

    private void Awake()
    {
        resource = FindAnyObjectByType<Personal_resource>();
    }

    private void OnEnable()
    {
        wattText.text = "" + Mathf.FloorToInt(price.watt * TechTreeUnlock.useElectric);
        woodenText.text = "" + Mathf.FloorToInt(price.wooden * TechTreeUnlock.resourceSpending);
        steelText.text = "" + Mathf.FloorToInt(price.steel * TechTreeUnlock.resourceSpending);
    }

    public void Build()
    {
        // 1. 필요한 자원량 계산 (기술 트리 적용)
        int reqWatt = Mathf.FloorToInt(price.watt * TechTreeUnlock.useElectric);
        int reqWooden = Mathf.FloorToInt(price.wooden * TechTreeUnlock.resourceSpending);
        int reqSteel = Mathf.FloorToInt(price.steel * TechTreeUnlock.resourceSpending);

        // 2. 총 보유량 확인 (공용 창고 자원 + 개인 가방 자원)
        // 참고: Metal은 현재 price 구조체에 없어서 제외했지만, 필요시 추가 로직은 동일합니다.
        int totalWooden = global::Resource.public_wooden + resource.Wooden;
        int totalSteel = global::Resource.public_steel + resource.Steel;

        // 3. 자원 충분 여부 검사
        if (reqWatt <= global::Resource.public_watt && totalWooden >= reqWooden && totalSteel >= reqSteel)
        {
            // --- 자원 소모 로직 (공용 우선 차감) ---

            // 전력 소모
            global::Resource.public_watt -= reqWatt;

            // 나무 소모 (공용에서 먼저 뺌)
            if (global::Resource.public_wooden >= reqWooden)
            {
                global::Resource.public_wooden -= reqWooden;
            }
            else
            {
                int remain = reqWooden - global::Resource.public_wooden;
                global::Resource.public_wooden = 0;
                resource.Wooden -= remain;
            }

            // 철강 소모 (공용에서 먼저 뺌)
            if (global::Resource.public_steel >= reqSteel)
            {
                global::Resource.public_steel -= reqSteel;
            }
            else
            {
                int remain = reqSteel - global::Resource.public_steel;
                global::Resource.public_steel = 0;
                resource.Steel -= remain;
            }

            // --- 건물 생성 로직 (기존 유지) ---
            GameObject temp = Instantiate(buildObject, Input.mousePosition, Quaternion.identity);

            if (temp.TryGetComponent(out BuildExampleImage buildExample))
            {
                buildExample.price.watt = reqWatt;
                buildExample.price.wooden = reqWooden;
                buildExample.price.steel = reqSteel;
            }

            UIOpen.isEnable[KeyCode.B] = false;
            buildUI.SetActive(false);
        }
        else
        {
            Notion.Warning("Notenough".Localize());
        }
    }
}
