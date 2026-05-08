using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ���� ���� enum
/// </summary>
public enum Hammer
{
    None = 0,
    Hammer = 1
}

public enum Melee
{
    None = 0,
    Axe = 1,
    Katana = 2
}

public enum Gun
{
    None = 0,
    Pistol = 1,
    Rifle = 2,
    ShotGun = 3,
    Granadelauncher = 4
}

public enum Mine
{
    None = 0,
    Bomb = 1
}

public enum Throw
{
    None = 0,
    Alram = 1,
    Molotov = 2,
    Grenade = 3
}

public enum Turrets
{
    None = 0,
}

/// <summary>
/// �÷��̾ ���� ���⸦ �����ϴ� Ŭ����
/// </summary>
public static class Weapon
{
    public static Dictionary<GameObject, List<Weapons.Weapon>> weaponList = new Dictionary<GameObject, List<Weapons.Weapon>>();

    public static void WeaponChange(Weapons.Weapon myself, int weaponIndex)
    {
        ObjectPoolManager.instance[myself.kind].weaponIndex = weaponIndex;

        GameObject key = myself.gameObject;

        string weaponName = myself.kind.ToString();
        Notion.Log("ChangeWeapon".Localize("En", weaponName));

        foreach (Weapons.Weapon attack in weaponList[key])
        {
            if (attack == myself)
            {
                attack.enabled = true;
            }
            else
            {
                attack.enabled = false;
            }
        }
    }
}
