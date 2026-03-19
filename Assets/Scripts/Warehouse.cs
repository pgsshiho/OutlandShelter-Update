using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warehouse : Guide
{
    [SerializeField] private GameObject bag;

    public override void Enable()
    {
        bag.SetActive(true);
    }

    public override void Disable()
    {
        bag.SetActive(false);
    }
}
