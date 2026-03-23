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
            Notion.Warning("Generatoriuninstall".Localize());
        }
    }

    protected override void OnDestroy()
    {
        Resource.public_watt -= watt;
        base.OnDestroy();
    }
}
