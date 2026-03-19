using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockFacility : MonoBehaviour
{
    [SerializeField] private GameObject facility;

    private void OnEnable()
    {
        if (TechTreeUnlock.infectionTreat && !facility.activeSelf) facility.SetActive(true);
    }
}
