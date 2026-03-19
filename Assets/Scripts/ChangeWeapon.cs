using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeWeapon : MonoBehaviour
{
    private Weapons.Weapon weapon;
    public int[] indexes;

    private void Awake()
    {
        indexes = new int[GetComponents<Weapons.Weapon>().Length];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weapon = GetComponent<Weapons.Hammer>();
            if (ItemOwnManager.ownWeapon[Kind.Hammer][indexes[0]]) Weapon.WeaponChange(weapon, indexes[0]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weapon = GetComponent<Weapons.Melee>();
            if (ItemOwnManager.ownWeapon[Kind.Melee][indexes[1]]) Weapon.WeaponChange(weapon, indexes[1]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            weapon = GetComponent<Weapons.Gun>();
            if (ItemOwnManager.ownWeapon[Kind.Gun][indexes[2]]) Weapon.WeaponChange(weapon, indexes[2]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            weapon = GetComponent<Weapons.Throw>();
            if (ItemOwnManager.ownWeapon[Kind.Throw][indexes[4]]) Weapon.WeaponChange(weapon, indexes[4]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            weapon = GetComponent<Weapons.Mine>();
            if (ItemOwnManager.ownWeapon[Kind.Mine][indexes[3]]) Weapon.WeaponChange(weapon, indexes[3]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            weapon = GetComponent<Weapons.Turret>();
            if (ItemOwnManager.ownWeapon[Kind.Turret][indexes[5]]) Weapon.WeaponChange(weapon, indexes[5]);
        }
    }
}
