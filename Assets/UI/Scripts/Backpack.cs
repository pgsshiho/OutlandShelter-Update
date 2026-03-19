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
        if (resources[(int)resourceKind] < Mathf.FloorToInt(capacity * TechTreeUnlock.capacity))
        {
            if (resources[(int)resourceKind] + count <= Mathf.FloorToInt(capacity * TechTreeUnlock.capacity))
            {
                resources[(int)resourceKind] += count;
                return 0;
            }
            else
            {
                int need = Mathf.FloorToInt(capacity * TechTreeUnlock.capacity) - resources[(int)resourceKind];

                count -= need;

                resources[(int)resourceKind] += need;
            }
        }
        else
        {
            Notion.Warning("해당 자원은 가득 찼습니다!!!");
        }

        return count;
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

    // Update is called once per frame
    protected virtual void Update()
    {
        for (int i = 0; i < resourceCounts.Length; i++)
        {
            resourceCounts[i].text = resources[i] + "/" + Mathf.FloorToInt(capacity * TechTreeUnlock.capacity) + "개";
        }
    }
}
