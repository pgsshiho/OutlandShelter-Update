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
    public static Dictionary<WorkbenchInputTaskTuto, float> remainingTimeTuto =
       new Dictionary<WorkbenchInputTaskTuto, float>();
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
    public static int[] throwCounts = new int[4];

    private void Awake()
    {
        // 💡 [수정] 일반 데이터 초기화
        remainingTime.Clear();
        partsOwn.Clear();
        WorkbenchInputTask.taskQueue.Clear();

        // 💡 [추가] 튜토리얼 관련 데이터도 Awake 시점에 깔끔하게 비워줍니다.
        remainingTimeTuto.Clear();
        WorkbenchInputTaskTuto.taskQueue.Clear();

        for (int i = 0; i < turretCounts.Length; i++) { turretCounts[i] = 0; }
        for (int i = 0; i < throwCounts.Length; i++) { throwCounts[i] = 0; }
    }

    private void Update()
    {
        // ----------------------------------------------------
        // 1. 일반 제작 태스크 업데이트 (기존 유지)
        // ----------------------------------------------------
        foreach (var task in remainingTime.Keys.ToList())
        {
            if (task == null || task.isComplete) continue;
            if (WorkbenchInputTask.taskQueue.Count == 0 || WorkbenchInputTask.taskQueue.Peek() != task.timeCheck) continue;

            if (remainingTime[task] != -1)
                remainingTime[task] = Mathf.Clamp(task.spendTime - (Time.time - task.StartTime), 0, task.spendTime);

            if (remainingTime[task] == 0 || remainingTime[task] == -1)
            {
                WorkbenchInputTask.taskQueue.Dequeue();
                task.isComplete = true;
                task.taskBar.transform.parent.gameObject.SetActive(false);
                Notion.Log("Production is complete!!!".Localize("En", task.itemName));

                if (SFXReference.Instance.making != null)
                    SoundManager.SFX.PlayOneShot(SFXReference.Instance.making, 1f);

                Invoke(task.itemName, 0f);
            }
        }

        // ----------------------------------------------------
        // 💡 2. [추가] 튜토리얼 제작 태스크 업데이트 로직
        // ----------------------------------------------------
        foreach (var taskTuto in remainingTimeTuto.Keys.ToList())
        {
            // 예외 방지 및 이미 완료된 태스크 패스
            if (taskTuto == null || taskTuto.isComplete) continue;
            // 큐가 비어있거나, 현재 가장 앞선 순서가 내 시간 체크 텍스트가 아니라면 대기
            if (WorkbenchInputTaskTuto.taskQueue.Count == 0 || WorkbenchInputTaskTuto.taskQueue.Peek() != taskTuto.timeCheck) continue;

            // 시간 감소 처리
            if (remainingTimeTuto[taskTuto] != -1)
                remainingTimeTuto[taskTuto] = Mathf.Clamp(taskTuto.spendTime - (Time.time - taskTuto.StartTime), 0, taskTuto.spendTime);

            // 시간이 다 흘렀다면 완료 처리 진행
            if (remainingTimeTuto[taskTuto] == 0 || remainingTimeTuto[taskTuto] == -1)
            {
                WorkbenchInputTaskTuto.taskQueue.Dequeue();
                taskTuto.isComplete = true;
                taskTuto.taskBar.transform.parent.gameObject.SetActive(false);
                Notion.Log("Production is complete!!!".Localize("En", taskTuto.itemName));

                if (SFXReference.Instance.making != null)
                    SoundManager.SFX.PlayOneShot(SFXReference.Instance.making, 1f);

                // 보상 메서드 실행 (★주의: taskTuto.itemName이 스크립트에 적힌 함수명과 대소문자까지 똑같아야 함!)
                Invoke(taskTuto.itemName, 0f);
            }
        }

        // 무기 및 방어구 소유권 갱신 (기존 유지)
        for (int i = 0; i < turretCounts.Length; i++) { ItemOwnManager.ownWeapon[Kind.Turret][i] = turretCounts[i] != 0; }
        for (int i = 0; i < throwCounts.Length; i++) { ItemOwnManager.ownWeapon[Kind.Throw][i] = throwCounts[i] != 0; }
        ItemOwnManager.ownWeapon[Kind.Mine][0] = mineCount != 0;
    }

    // --- 이하 보상 지급 함수들 생략 (기존과 동일) ---
    private void PistolBullet() { BulletManager.pistolBullet += 6; }
    private void Shoes() { OnShoesCrafted?.Invoke(30); Notion.Log("Shoes Production Complete!"); }
    private void RifleBullet() { BulletManager.rifleBullet += 15; }
    private void ShotgunBullet() { BulletManager.shotGunBullet += 8; }
    private void Razor() { string temp = "Razor"; PartsOwn[temp] += 1; }
    private void LightDivice() { string temp = "LightDivice"; PartsOwn[temp] += 1; }
    private void Hologram() { string temp = "Hologram"; PartsOwn[temp] += 1; }
    private void Scope() { string temp = "Scope"; PartsOwn[temp] += 1; }
    private void Silencer() { string temp = "Silencer"; PartsOwn[temp] += 1; }
    private void Controller() { string temp = "Controller"; PartsOwn[temp] += 1; }
    private void Handle() { string temp = "Handle"; PartsOwn[temp] += 1; }
    private void Choke() { string temp = "Choke"; PartsOwn[temp] += 1; }
    private void CartridgeBelt() { string temp = "CartridgeBelt"; PartsOwn[temp] += 1; }
    private void NormalTurret() { turretCounts[0] += 1; }
    private void SnipingTurret() { turretCounts[1] += 1; }
    private void Mine() { mineCount += 1; }
    private void Alram() { throwCounts[0] += 1; }
    private void FireBottle() { throwCounts[1] += 1; }
    private void Grenade() { throwCounts[2] += 1; }
    private void WoodenArmor() { ItemOwnManager.ownWeapon[Kind.Armor][0] = true; }
    private void MetalArmor() { ItemOwnManager.ownWeapon[Kind.Armor][1] = true; }
    private void SteelArmor() { ItemOwnManager.ownWeapon[Kind.Armor][2] = true; }
    private void Pistol() { ItemOwnManager.ownWeapon[Kind.Gun][(int)GunKind.Pistol] = true; }
    private void Revolver() { ItemOwnManager.ownWeapon[Kind.Gun][(int)GunKind.Revolver] = true; }
    private void Rifle() { ItemOwnManager.ownWeapon[Kind.Gun][(int)GunKind.Rifle] = true; }
    private void HalfAutoRifle() { ItemOwnManager.ownWeapon[Kind.Gun][(int)GunKind.HalfAutoRifle] = true; }
    private void ShotGun() { ItemOwnManager.ownWeapon[Kind.Gun][(int)GunKind.Shotgun] = true; }
    private void AutoShotGun() { ItemOwnManager.ownWeapon[Kind.Gun][(int)GunKind.AutoShotgun] = true; }
    private void SMG() { ItemOwnManager.ownWeapon[Kind.Gun][(int)GunKind.SMG] = true; }
    private void DBS() { ItemOwnManager.ownWeapon[Kind.Gun][(int)GunKind.DBS] = true; }
    private void AWP() { ItemOwnManager.ownWeapon[Kind.Gun][(int)GunKind.AWP] = true; }
    private void RPG()
    {
        int rpgIndex = (int)GunKind.RPG;
        ItemOwnManager.ownWeapon[Kind.Gun][rpgIndex] = true;
        var playerWeapon = FindAnyObjectByType<Weapons.Gun>();
        if (playerWeapon != null && playerWeapon.kind == Kind.Gun) { Weapon.WeaponChange(playerWeapon, rpgIndex); }
        Notion.Log("Special RPG Crafted: One shot only.");
    }
    private void firstaidkit() { throwCounts[3] += 1; }
}