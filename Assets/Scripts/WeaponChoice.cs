using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChoice : MonoBehaviour
{
    [SerializeField]
    private Kind kind;

    [SerializeField]
    private int myIndex;

    GameObject child;

    ChangeWeapon changeWeapon;

    private void Awake()
    {
        child = transform.GetChild(0).gameObject;

        changeWeapon = FindAnyObjectByType<ChangeWeapon>();
    }

    private void OnEnable()
    {
        child.SetActive(ItemOwnManager.ownWeapon[kind][myIndex]);
        UIOpen.isBlocking = true;
    }

    public void Change()
    {
        if (ItemOwnManager.ownWeapon[kind][myIndex])
        {
            ObjectPoolManager.instance[kind].weaponIndex = myIndex;
            changeWeapon.indexes[(int)kind] = myIndex;
            transform.parent.parent.gameObject.SetActive(false);
        }
    }

    public void Close(GameObject go)
    {
        go.SetActive(false);
    }

    private void OnDisable()
    {
        UIOpen.isBlocking = false;
    }
}
