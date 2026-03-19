using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChangeOnUI : MonoBehaviour
{
    [SerializeField] private Kind kind;

    private GameObject[] weapons;

    public int WeaponIndex
    {
        get
        {
            return ObjectPoolManager.instance[kind].weaponIndex;
        }
    }

    [SerializeField] private GameObject weaponChangeUI;

    private void Awake()
    {
        weapons = new GameObject[transform.childCount];

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i] = transform.GetChild(i).gameObject;
        }

        weapons[^1].SetActive(true);
    }

    private void Update()
    {
        bool temp = true;

        for (int i = 0; i < weapons.Length - 1; i++)
        {
            if (weapons[i].activeSelf != (i == WeaponIndex))
            {
                weapons[i].SetActive(i == WeaponIndex && ItemOwnManager.ownWeapon[kind][WeaponIndex]);
            }
            if (weapons[i].activeSelf)
            {
                temp = false;
            }
        }

        weapons[^1].SetActive(temp);
    }

    public void OpenList()
    {
        weaponChangeUI.SetActive(true);
    }
}
