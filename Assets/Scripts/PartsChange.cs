using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartsChange : MonoBehaviour
{
    [HideInInspector] public string myParts;

    public GunKind kind;

    Image child;

    private void Awake()
    {
        child = transform.GetChild(0).GetComponent<Image>();
    }

    public void OpenList(GameObject go)
    {
        go.transform.parent.gameObject.SetActive(true);
        go.SetActive(true);
    }

    public void ChangeImage(Sprite sprite, Vector2 localPosition, Vector2 scale)
    {
        child.sprite = sprite;
        child.transform.localPosition = localPosition * (2f / 3f);
        if (child.TryGetComponent(out RectTransform rectTransform))
        {
            rectTransform.sizeDelta = scale * (2f / 3f);
        }
        if (!child.gameObject.activeSelf) child.gameObject.SetActive(true);
    }
}
