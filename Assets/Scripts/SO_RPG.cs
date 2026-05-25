using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "RPG", menuName = "Gun/RPG")]
public class SO_RPG : SO_Gun
{
    private readonly Dictionary<GameObject, Rigidbody2D> rbCache = new();
    private readonly Dictionary<GameObject, SummonRPG> rpgCache = new();

    private static T GetCachedComponent<T>(GameObject key, Dictionary<GameObject, T> cache)
        where T : Component
    {
        if (key == null)
            return null;

        if (cache.TryGetValue(key, out T component))
        {
            if (component != null)
                return component;
            cache.Remove(key);
        }

        if (key.TryGetComponent(out component))
        {
            cache[key] = component;
            return component;
        }

        return null;
    }

    public override int UsingBullet
    {
        get => BulletManager.granadeLauncherBullet;
        set => BulletManager.granadeLauncherBullet = value;
    }

    public override IEnumerator Reload(
        Weapons.Gun gun,
        Func<int> loadedBulletGetter,
        Action<int> loadedBulletSetter,
        int capacity,
        float reloadTime
    )
    {
        // 추후 기획이 수정될 경우 구현(현재 기획상 RPG는 일회용 궁극 무기)
        yield break;
    }

    public override void Shoot(Weapons.Gun gun)
    {
        SoundManager.SFX.PlayOneShot(SFXReference.Instance.rpgShot); // RPG 발사음

        GameObject temp = PoolManager.Pool.Get();
        temp.transform.parent = gun.AttackPivot;
        Camera.main.transform.DOComplete(); // 이전 흔들림 캔슬로 딜레이 방지
        Camera.main.transform.DOShakePosition(0.2f, 0.3f, 15, 90, false, true);
        // 위치 설정 (기존 총구 오프셋 활용)
        temp.transform.localPosition = new Vector3(
            fireXOffset,
            characterHandHeight + gun.distanceBetweenPlayer[PoolManager.weaponIndex],
            0
        );
        temp.transform.localEulerAngles = Vector3.zero;
        temp.transform.parent = null;

        // RPG 탄두 속도 및 물리 설정
        Rigidbody2D rpgRb = GetCachedComponent(temp, rbCache);
        if (rpgRb)
            rpgRb.linearVelocity = shotSpeed * 0.5f * temp.transform.up; // RPG는 보통 탄속이 느림

        // RPG 전용 스크립트가 있다면 데이터 전달
        SummonRPG rpgScript = GetCachedComponent(temp, rpgCache);
        if (rpgScript)
            rpgScript.pool = PoolManager.Pool;

        // 무기 소유권 박탈 (내려놓지도 못하게 하려면 인벤토리에서 제거)
        ItemOwnManager.ownWeapon[Kind.Gun][6] = false;

        // 알림 메시지
        Notion.Log("The RPG has been used and is no longer available.");
    }

    public override void Clear()
    {
        rbCache.Clear();
        rpgCache.Clear();
    }
}
