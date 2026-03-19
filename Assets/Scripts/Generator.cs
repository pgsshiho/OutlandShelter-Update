using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : ResourceReturn
{
    public int watt;

    protected override void Awake()
    {
        base.Awake();
        Resource.public_watt += watt;
    }

    public override void TearDown()
    {
        if (Resource.public_watt >= watt)
        {
            base.TearDown();
        }
        else
        {
            Notion.Warning("발전기를 철거할 전력이 부족합니다!!!");
        }
    }

    protected override void OnDestroy()
    {
        Resource.public_watt -= watt;
        base.OnDestroy();
    }
}
