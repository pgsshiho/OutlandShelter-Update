using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 순서 기억용 enum
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
    ShotGun = 3
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
/// 플레이어가 가진 무기를 관리하는 클래스
/// </summary>
public static class Weapon
{
    public static Dictionary<GameObject, List<Weapons.Weapon>> weaponList = new Dictionary<GameObject, List<Weapons.Weapon>>();

    public static void WeaponChange(Weapons.Weapon myself, int weaponIndex)
    {
        ObjectPoolManager.instance[myself.kind].weaponIndex = weaponIndex;

        GameObject key = myself.gameObject;

        Notion.Log($"{myself.kind}로 무기를 교체합니다");

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
