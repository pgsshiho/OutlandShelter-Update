using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOwnManager : MonoBehaviour
{
    public static Dictionary<Kind, bool[]> ownWeapon = new Dictionary<Kind, bool[]>();

    private void Start()
    {
        Dictionary<Kind, ObjectPoolManager> temp = ObjectPoolManager.instance;

        foreach (Kind kind in Enum.GetValues(typeof(Kind)))
        {
            if (temp.ContainsKey(kind)) ownWeapon[kind] = new bool[temp[kind].defaultCapacity.Length];
        }

        ownWeapon[Kind.Melee][(int)FindAnyObjectByType<PlayerMove>().gender] = true;
        ownWeapon[Kind.Hammer][0] = true;
    }
}
