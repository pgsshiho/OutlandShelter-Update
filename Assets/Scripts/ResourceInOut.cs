using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResourceInOut : MonoBehaviour, IPointerClickHandler
{
    public Backpack.ResourceKind resourceKind;
    [SerializeField] private Backpack backpack;

    public void OnPointerClick(PointerEventData eventData)
    {
        backpack.resources[(int)resourceKind] = backpack.otherBag.Put(resourceKind, backpack.resources[(int)resourceKind]);
    }
}
