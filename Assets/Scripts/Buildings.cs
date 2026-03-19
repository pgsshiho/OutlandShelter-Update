using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Buildings : MonoBehaviour
{
    [System.Serializable]
    public struct Resource
    {
        public int watt;
        public int wooden;
        public int steel;
    }
    public Resource price;
    [SerializeField] private GameObject buildObject;
    private Personal_resource resource;
    [SerializeField] private GameObject buildUI;

    [SerializeField] private TextMeshProUGUI wattText;
    [SerializeField] private TextMeshProUGUI woodenText;
    [SerializeField] private TextMeshProUGUI steelText;

    private void Awake()
    {
        resource = FindAnyObjectByType<Personal_resource>();
    }

    private void OnEnable()
    {
        wattText.text = "" + Mathf.FloorToInt(price.watt * TechTreeUnlock.useElectric);
        woodenText.text = "" + Mathf.FloorToInt(price.wooden * TechTreeUnlock.resourceSpending);
        steelText.text = "" + Mathf.FloorToInt(price.steel * TechTreeUnlock.resourceSpending);
    }

    public void Build()
    {
        int tempWatt = Mathf.FloorToInt(price.watt * TechTreeUnlock.useElectric);
        int tempWooden = Mathf.FloorToInt(price.wooden * TechTreeUnlock.resourceSpending);
        int tempSteel = Mathf.FloorToInt(price.steel * TechTreeUnlock.resourceSpending);

        if (tempWatt <= global::Resource.public_watt && tempWooden <= resource.Wooden && tempSteel <= resource.Steel)
        {
            global::Resource.public_watt -= tempWatt;
            resource.Wooden -= tempWooden;
            resource.Steel -= tempSteel;
            GameObject temp = Instantiate(buildObject, Input.mousePosition, Quaternion.identity);

            if (temp.TryGetComponent(out BuildExampleImage buildExample))
            {
                buildExample.price.watt = tempWatt;
                buildExample.price.wooden = tempWooden;
                buildExample.price.steel = tempSteel;
            }

            UIOpen.isEnable[KeyCode.B] = false;
            buildUI.SetActive(false);
        }
        else
        {
            Notion.Warning("자원이 부족합니다!!!");
        }
    }
}
