using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UsableSkillPoint : MonoBehaviour
{
    private TextMeshProUGUI skillPoint;

    private void Awake()
    {
        skillPoint = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        skillPoint.text = $"Skill Point : {TechTreeUnlock.skillPoint}";
    }
}
