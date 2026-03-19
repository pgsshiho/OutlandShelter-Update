using System;
using System.Collections.Generic;
using UnityEngine;

public enum GunKind
{
    Pistol = 0,
    Revolver = 1,
    Rifle = 2,
    HalfAutoRifle = 3,
    Shotgun = 4,
    AutoShotgun = 5
}

public class GunStatManager
{
    public static Dictionary<GunKind, GunStatManager> instance = new();
    public readonly Dictionary<string, Action<GunKind>> partsEffect = new Dictionary<string, Action<GunKind>>
    {
        { "Razor", (a) => instance[a].accuracy *= 1.2f * TechTreeUnlock.partsAbility },
        { "LightDivice", (a) => instance[a].isLight = true },
        { "Hologram", (a) => instance[a].accuracy *= 1.2f * TechTreeUnlock.partsAbility },
        { "Scope", (a) => { instance[a].accuracy *= 1.2f * TechTreeUnlock.partsAbility; instance[a].range *= 1.5f * TechTreeUnlock.partsAbility; } },
        { "Silencer",  (a) => { instance[a].damage *= 0.9f * TechTreeUnlock.partsAbility; instance[a].isSilence = true; } },
        { "Controller", (a) => instance[a].accuracy *= 1.5f * TechTreeUnlock.partsAbility },
        { "Handle", (a) => instance[a].attackSpeed *= 1.2f * TechTreeUnlock.partsAbility },
        { "Choke", (a) => instance[a].range *= 1.5f * TechTreeUnlock.partsAbility },
        { "CartridgeBelt", (a) => instance[a].reloadingTime *= 0.7f / TechTreeUnlock.partsAbility }
    };
    public readonly Dictionary<string, Action<GunKind>> partsUnEquipEffect = new Dictionary<string, Action<GunKind>>
    {
        { "Razor", (a) => instance[a].accuracy /= 1.2f * TechTreeUnlock.partsAbility },
        { "LightDivice", (a) => instance[a].isLight = false },
        { "Hologram", (a) => instance[a].accuracy /= 1.2f * TechTreeUnlock.partsAbility },
        { "Scope", (a) => { instance[a].accuracy /= 1.2f * TechTreeUnlock.partsAbility; instance[a].range /= 1.5f * TechTreeUnlock.partsAbility; } },
        { "Silencer",  (a) => { instance[a].damage /= 0.9f * TechTreeUnlock.partsAbility; instance[a].isSilence = false; } },
        { "Controller", (a) => instance[a].accuracy /= 1.5f * TechTreeUnlock.partsAbility },
        { "Handle", (a) => instance[a].attackSpeed /= 1.2f * TechTreeUnlock.partsAbility },
        { "Choke", (a) => instance[a].range /= 1.5f * TechTreeUnlock.partsAbility },
        { "CartridgeBelt", (a) => instance[a].reloadingTime /= 0.7f / TechTreeUnlock.partsAbility }
    };

    public float accuracy = 1;
    public bool isLight = false;
    public float range = 1;
    public float damage = 1;
    public bool isSilence = false;
    public float attackSpeed = 1;
    public float reloadingTime = 1;

    public static void Awake()
    {
        foreach (GunKind temp in Enum.GetValues(typeof(GunKind)))
        {
            instance[temp] = new GunStatManager();
        }
    }
}
