using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBag : Backpack
{
    public GameObject playerBag;

    protected override void Awake()
    {
        
    }

    public override void OtherBagReceive(Backpack backpack)
    {
        otherBag = backpack;
        
        playerBag.SetActive(true);

        StartCoroutine(WaitAction.wait(() => { return !otherBag.gameObject.activeSelf; }, () =>
        {
            playerBag.SetActive(false);
        }));
    }

    public override int Put(ResourceKind resourceKind, int count)
    {
        if (resources[(int)resourceKind] < capacity)
        {
            if (resources[(int)resourceKind] + count <= capacity)
            {
                resources[(int)resourceKind] += count;
                return 0;
            }
            else
            {
                int need = capacity - resources[(int)resourceKind];

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
}
