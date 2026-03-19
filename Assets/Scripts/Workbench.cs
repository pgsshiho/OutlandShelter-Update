using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : Guide
{
    [SerializeField] private GameObject workbench;

    public override void Disable()
    {
        workbench.SetActive(false);
    }

    public override void Enable()
    {
        workbench.SetActive(true);
    }
}
