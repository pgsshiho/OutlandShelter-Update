using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkbenchInputTask : MonoBehaviour
{
    public float spendTime;
    public float StartTime
    {
        get; private set;
    }
    public Image taskBar;
    public TextMeshProUGUI timeCheck;
    [SerializeField] private TextMeshProUGUI[] material;
    [SerializeField] private float[] prices;

    public string itemName;
    [HideInInspector] public bool isComplete = true;

    private PlayerBag bag;

    public static Queue<TextMeshProUGUI> taskQueue = new Queue<TextMeshProUGUI>();

    private void Awake()
    {
        bag = FindAnyObjectByType<PlayerBag>();
        material[0].text = ": " + prices[0];
        material[1].text = ": " + prices[1];
        material[2].text = ": " + prices[2];
        material[3].text = ": " + prices[3];
    }

    private void Update()
    {
        if (spendTime != -1)
        {
            if (!isComplete)
            {
                taskBar.fillAmount = (Time.time - StartTime) / spendTime;
                timeCheck.text = $"대기 순서 : {taskQueue.ToList().IndexOf(timeCheck)}\n남은 시간 : {(int)(WorkingManager.remainingTime[this] / 60f)}분 {(int)(WorkingManager.remainingTime[this] % 60)}초";
            }
            else
            {
                timeCheck.text = $"대기 순서 : {taskQueue.ToList().IndexOf(timeCheck)}\n남은 시간 : {(int)(spendTime / 60f)}분 {(int)(spendTime % 60)}초";
            }
        }
    }

    public void StartTask()
    {
        // 1. 필요한 자원량 계산 (기술 트리 적용)
        int reqWooden = (int)(prices[(int)Backpack.ResourceKind.Wooden] * TechTreeUnlock.resourceSpending);
        int reqSteel = (int)(prices[(int)Backpack.ResourceKind.Steel] * TechTreeUnlock.resourceSpending);
        int reqMetal = (int)(prices[(int)Backpack.ResourceKind.Metal] * TechTreeUnlock.resourceSpending);
        int reqWatt = (int)(prices[3] * TechTreeUnlock.useElectric);

        // 2. 총 보유량 확인 (공용 창고 + 플레이어 가방)
        int totalWooden = Resource.public_wooden + bag.resources[(int)Backpack.ResourceKind.Wooden];
        int totalSteel = Resource.public_steel + bag.resources[(int)Backpack.ResourceKind.Steel];
        int totalMetal = Resource.public_metal + bag.resources[(int)Backpack.ResourceKind.Metal];

        // 3. 모든 자원이 충분한지 체크
        if (totalWooden >= reqWooden && totalSteel >= reqSteel &&
            totalMetal >= reqMetal && Resource.public_watt >= reqWatt)
        {
            // --- 자원 소모 로직 (공용 우선 차감) ---

            // 나무 소모
            ConsumeResource(ref Resource.public_wooden, reqWooden, Backpack.ResourceKind.Wooden);
            // 철강 소모
            ConsumeResource(ref Resource.public_steel, reqSteel, Backpack.ResourceKind.Steel);
            // 금속 소모
            ConsumeResource(ref Resource.public_metal, reqMetal, Backpack.ResourceKind.Metal);

            // 전력은 공용에서만 소모
            Resource.public_watt -= reqWatt;

            // --- 제작 큐 등록 (기존 로직) ---
            taskQueue.Enqueue(timeCheck);
            StartCoroutine(WaitAction.wait(() => { return taskQueue.Peek() == timeCheck; }, () =>
            {
                StartTime = Time.time;
                isComplete = false;
            }));
            WorkingManager.remainingTime[this] = spendTime;
            taskBar.fillAmount = 0;
            taskBar.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            Notion.Warning("Notenough".Localize());
        }
    }

    // 자원 차감을 도와주는 헬퍼 메서드
    private void ConsumeResource(ref int publicRes, int required, Backpack.ResourceKind kind)
    {
        if (publicRes >= required)
        {
            publicRes -= required; // 공용 자원이 충분하면 공용에서 다 뺌
        }
        else
        {
            int remain = required - publicRes; // 모자란 양 계산
            publicRes = 0; // 공용 자원 탈탈 털기
            bag.resources[(int)kind] -= remain; // 나머지는 플레이어 가방에서 차감
        }
    }
}
