using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartsChoice : MonoBehaviour
{
    [SerializeField] private PartsChange parts;

    [SerializeField] private string myName;

    [SerializeField] private Image linkImage;

    private Transform child;
    private Sprite childImage;
    private TextMeshProUGUI itemCount;

    private void Awake()
    {
        child = transform.GetChild(0);
        itemCount = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        childImage = child.GetComponent<Image>().sprite;
    }

    private void OnEnable()
    {
        itemCount.text = "" + WorkingManager.PartsOwn[myName];
        UIOpen.isBlocking = true;
    }

    public void SelectParts()
    {
        if (WorkingManager.PartsOwn[myName] > 0)
        {
            if (WorkingManager.PartsOwn.ContainsKey(parts.myParts))
            {
                WorkingManager.PartsOwn[parts.myParts]++;
                GunStatManager.instance[parts.kind].partsUnEquipEffect[parts.myParts](parts.kind);
            }

            parts.myParts = myName;

            WorkingManager.PartsOwn[parts.myParts]--;

            GunStatManager.instance[parts.kind].partsEffect[parts.myParts](parts.kind);

            itemCount.text = "" + WorkingManager.PartsOwn[myName];

            if (child.TryGetComponent(out RectTransform rect))
            {
                parts.ChangeImage(childImage, child.localPosition, rect.sizeDelta);
            }

            linkImage.sprite = childImage;
            linkImage.gameObject.SetActive(true);

            Close(transform.parent.parent.gameObject);
        }
        else
        {
            Notion.Warning("해당 파츠를 보유하고 있지 않습니다!!");
        }
    }

    public void TakeOff()
    {
        if (parts.myParts != "")
        {
            WorkingManager.PartsOwn[parts.myParts]++;

            GunStatManager.instance[parts.kind].partsUnEquipEffect[parts.myParts](parts.kind);

            parts.myParts = "";
            
            linkImage.gameObject.SetActive(false);
            
            Close(transform.parent.parent.gameObject);
        }
        else
        {
            Notion.Warning("파츠가 장착되어 있지 않습니다!!");
        }
    }

    public void Close(GameObject go)
    {
        GameObject parent = transform.parent.gameObject;

        parts.StartCoroutine(WaitAction.waitOneFrame(() =>
        {
            parent.SetActive(false);
            go.SetActive(false);
        }));
    }

    private void OnDisable()
    {
        UIOpen.isBlocking = false;
    }
}
