using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorkingManager : MonoBehaviour
{
    public static event Action<int> OnShoesCrafted;
    public static Dictionary<WorkbenchInputTask, float> remainingTime =
        new Dictionary<WorkbenchInputTask, float>();
    private static Dictionary<string, int> partsOwn = new();

    public class Parts
    {
        public int this[string name]
        {
            get
            {
                if (name == null || name == "")
                    return 0;

                if (!partsOwn.ContainsKey(name))
                    partsOwn[name] = 0;
                return partsOwn[name];
            }
            set { partsOwn[name] = value; }
        }

        public bool ContainsKey(string name)
        {
            return partsOwn.ContainsKey(name);
        }
    }

    public static Parts PartsOwn = new();

    public static int[] turretCounts = new int[2];
    public static int mineCount = 0;
    public static int[] throwCounts = new int[3];

    private void Awake()
    {
        remainingTime.Clear();
        partsOwn.Clear();
        for (int i = 0; i < turretCounts.Length; i++)
        {
            turretCounts[i] = 0;
        }
        for (int i = 0; i < throwCounts.Length; i++)
        {
            throwCounts[i] = 0;
        }
        WorkbenchInputTask.taskQueue.Clear();
    }

    private void Update()
    {
        foreach (var task in remainingTime.Keys.ToList())
        {
            if (task.isComplete)
                continue;
            if (WorkbenchInputTask.taskQueue.Peek() != task.timeCheck)
                continue;

            if (remainingTime[task] != -1)
                remainingTime[task] = Mathf.Clamp(
                    task.spendTime - (Time.time - task.StartTime),
                    0,
                    task.spendTime
                );

            if (remainingTime[task] == 0 || remainingTime[task] == -1)
            {
                WorkbenchInputTask.taskQueue.Dequeue();
                task.isComplete = true;
                task.taskBar.transform.parent.gameObject.SetActive(false);
                Notion.Log("Production is complete!!!".Localize("En", task.itemName));
                if (SFXReference.Instance.making != null)
                {
                    SoundManager.SFX.PlayOneShot(SFXReference.Instance.making, 1f);
                }
                Invoke(task.itemName, 0f);
            }
        }

        for (int i = 0; i < turretCounts.Length; i++)
        {
            ItemOwnManager.ownWeapon[Kind.Turret][i] = turretCounts[i] != 0;
        }
        for (int i = 0; i < throwCounts.Length; i++)
        {
            ItemOwnManager.ownWeapon[Kind.Throw][i] = throwCounts[i] != 0;
        }
        ItemOwnManager.ownWeapon[Kind.Mine][0] = mineCount != 0;
    }

    private void PistolBullet()
    {
        BulletManager.pistolBullet += 6;
    }

    private void Shoes()
    {
        OnShoesCrafted?.Invoke(30);

        Notion.Log("Shoes Production Complete!");
    }

    private void RifleBullet()
    {
        BulletManager.rifleBullet += 15;
    }

    private void ShotgunBullet()
    {
        BulletManager.shotGunBullet += 8;
    }

    private void Razor()
    {
        string temp = "Razor";

        PartsOwn[temp] += 1;
    }

    private void LightDivice()
    {
        string temp = "LightDivice";

        PartsOwn[temp] += 1;
    }

    private void Hologram()
    {
        string temp = "Hologram";

        PartsOwn[temp] += 1;
    }

    private void Scope()
    {
        string temp = "Scope";

        PartsOwn[temp] += 1;
    }

    private void Silencer()
    {
        string temp = "Silencer";

        PartsOwn[temp] += 1;
    }

    private void Controller()
    {
        string temp = "Controller";

        PartsOwn[temp] += 1;
    }

    private void Handle()
    {
        string temp = "Handle";

        PartsOwn[temp] += 1;
    }

    private void Choke()
    {
        string temp = "Choke";

        PartsOwn[temp] += 1;
    }

    private void CartridgeBelt()
    {
        string temp = "CartridgeBelt";

        PartsOwn[temp] += 1;
    }

    private void NormalTurret()
    {
        turretCounts[0] += 1;
    }

    private void SnipingTurret()
    {
        turretCounts[1] += 1;
    }

    private void Mine()
    {
        mineCount += 1;
    }

    private void Alram()
    {
        throwCounts[0] += 1;
    }

    private void FireBottle()
    {
        throwCounts[1] += 1;
    }

    private void Grenade()
    {
        throwCounts[2] += 1;
    }

    private void WoodenArmor()
    {
        ItemOwnManager.ownWeapon[Kind.Armor][0] = true;
    }

    private void MetalArmor()
    {
        ItemOwnManager.ownWeapon[Kind.Armor][1] = true;
    }

    private void SteelArmor()
    {
        ItemOwnManager.ownWeapon[Kind.Armor][2] = true;
    }

    private void Pistol()
    {
        ItemOwnManager.ownWeapon[Kind.Gun][(int)GunKind.Pistol] = true;
    }

    private void Revolver()
    {
        ItemOwnManager.ownWeapon[Kind.Gun][(int)GunKind.Revolver] = true;
    }

    private void Rifle()
    {
        ItemOwnManager.ownWeapon[Kind.Gun][(int)GunKind.Rifle] = true;
    }

    private void HalfAutoRifle()
    {
        ItemOwnManager.ownWeapon[Kind.Gun][(int)GunKind.HalfAutoRifle] = true;
    }

    private void ShotGun()
    {
        ItemOwnManager.ownWeapon[Kind.Gun][(int)GunKind.Shotgun] = true;
    }

    private void AutoShotGun()
    {
        ItemOwnManager.ownWeapon[Kind.Gun][(int)GunKind.AutoShotgun] = true;
    }
    private void SMG()
    {
        ItemOwnManager.ownWeapon[Kind.Gun][(int)GunKind.SMG] = true;
    }
    private void DBS()
    {
        ItemOwnManager.ownWeapon[Kind.Gun][(int)GunKind.DBS] = true;
    }
    private void AWP()
    {
        ItemOwnManager.ownWeapon[Kind.Gun][(int)GunKind.AWP] = true;
    }
    private void RPG()
    {
        int rpgIndex = (int)GunKind.RPG; // RPG 인덱스 번호
        ItemOwnManager.ownWeapon[Kind.Gun][rpgIndex] = true;

        // 추가 액션: 제작 완료 시 유저에게 강제로 장착시키기
        var playerWeapon = FindAnyObjectByType<Weapons.Gun>();
        if (playerWeapon != null && playerWeapon.kind == Kind.Gun)
        {
            Weapon.WeaponChange(playerWeapon, rpgIndex);
        }
        Notion.Log("Special RPG Crafted: One shot only.");
    }
}
