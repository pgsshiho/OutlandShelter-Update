using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Backpack : MonoBehaviour
{
    public int capacity = 50;
    public Backpack otherBag;

    public enum ResourceKind
    {
        Wooden = 0,
        Steel = 1,
        Metal = 2
    }

    public int[] resources = new int[3] { 0, 0, 0 };

    public TextMeshProUGUI[] resourceCounts = new TextMeshProUGUI[3];

    public virtual int Put(ResourceKind resourceKind, int count)
    {
        // --- 설치된 창고(Storage)일 경우 ---
        if (this is not PlayerBag)
        {
            int currentAmount = 0;
            if (resourceKind == ResourceKind.Wooden) currentAmount = Resource.public_wooden;
            else if (resourceKind == ResourceKind.Steel) currentAmount = Resource.public_steel;
            else if (resourceKind == ResourceKind.Metal) currentAmount = Resource.public_metal;

            // 현재 모든 창고를 합친 총 용량 (1개면 130, 2개면 260...)
            int totalMax = Resource.GetTotalStorageCapacity();

            if (currentAmount < totalMax)
            {
                // 넣을 수 있는 여유 공간 계산
                if (currentAmount + count <= totalMax)
                {
                    if (resourceKind == ResourceKind.Wooden) Resource.public_wooden += count;
                    else if (resourceKind == ResourceKind.Steel) Resource.public_steel += count;
                    else if (resourceKind == ResourceKind.Metal) Resource.public_metal += count;
                    return 0;
                }
                else
                {
                    int need = totalMax - currentAmount;
                    if (resourceKind == ResourceKind.Wooden) Resource.public_wooden += need;
                    else if (resourceKind == ResourceKind.Steel) Resource.public_steel += need;
                    else if (resourceKind == ResourceKind.Metal) Resource.public_metal += need;
                    return count - need; // 넘치는 양은 다시 내 가방으로
                }
            }
            else
            {
                Notion.Warning("resourcefull".Localize());
                return count;
            }
        }

        // --- 플레이어 가방(PlayerBag)일 경우 기존 로직 유지 ---
        int playerMax = Mathf.FloorToInt(capacity * TechTreeUnlock.capacity);
        if (resources[(int)resourceKind] < playerMax)
        {
            if (resources[(int)resourceKind] + count <= playerMax)
            {
                resources[(int)resourceKind] += count;
                return 0;
            }
            else
            {
                int need = playerMax - resources[(int)resourceKind];
                resources[(int)resourceKind] += need;
                return count - need;
            }
        }
        return count;
    }

    protected virtual void Update()
    {
        int totalMax = Resource.GetTotalStorageCapacity();

        for (int i = 0; i < resourceCounts.Length; i++)
        {
            if (this is PlayerBag)
            {
                int pMax = Mathf.FloorToInt(capacity * TechTreeUnlock.capacity);
                resourceCounts[i].text = resources[i] + "/" + pMax + "pieces";
            }
            else
            {
                // 모든 창고 UI에 "공용 자원 / 합산 용량"을 동일하게 표시
                int current = 0;
                if (i == 0) current = Resource.public_wooden;
                if (i == 1) current = Resource.public_steel;
                if (i == 2) current = Resource.public_metal;

                resourceCounts[i].text = current + "/" + totalMax + "pieces";
            }
        }
    }
    public virtual void OtherBagReceive(Backpack backpack) { }

    protected virtual void Awake()
    {
        otherBag = FindAnyObjectByType<PlayerBag>();
    }

    private void OnEnable()
    {
        if (otherBag != null) otherBag.OtherBagReceive(this);
    }
}
