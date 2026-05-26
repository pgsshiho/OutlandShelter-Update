using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "Rifle", menuName = "Gun/Rifle")]
public class SO_Rifle : SO_Gun
{
    private readonly Dictionary<GameObject, Rigidbody2D> rbCache = new();
    private readonly Dictionary<GameObject, SummonBullet> bulletCache = new();
    private readonly Dictionary<GameObject, SummonObject> summonObjectCache = new();

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
        get => BulletManager.rifleBullet;
        set => BulletManager.rifleBullet = value;
    }

    public override IEnumerator Reload(
        Weapons.Gun gun,
        Func<int> loadedBulletGetter,
        Action<int> loadedBulletSetter,
        int capacity,
        float reloadTime
    )
    {
        yield return new WaitForSeconds(reloadTime * TechTreeUnlock.reloadingTime);

        int reloadCount = Mathf.Clamp(capacity - loadedBulletGetter(), 0, UsingBullet);

        // 인벤토리에서 차감
        UsingBullet -= reloadCount;

        SoundManager.SFX.PlayOneShot(SFXReference.Instance.reload);
        loadedBulletSetter(loadedBulletGetter() + reloadCount);
    }

    public override void Shoot(Weapons.Gun gun)
    {
        SoundManager.SFX.PlayOneShot(SFXReference.Instance.gun);
        GameObject temp = PoolManager.Pool.Get();
        Camera.main.transform.DOComplete(); // 이전 흔들림 캔슬로 딜레이 방지
        Camera.main.transform.DOShakePosition(0.1f, 0.1f, 10, 90, false, true);

        SummonBullet summonBullet = GetCachedComponent(temp, bulletCache);
        if (summonBullet)
        {
            summonBullet.pool = PoolManager.Pool;
            summonBullet.isAuto = isAuto;
        }

        float currentDeviation =
            deviation * (TechTreeUnlock.duringMovingAccuracyFixed || !gun.IsMoving ? 1 : 1.2f);

        temp.transform.parent = gun.AttackPivot;

        // 수정: X축 오프셋 반영 (기본값 0)
        temp.transform.localPosition = new Vector3(
            fireXOffset,
            characterHandHeight + gun.distanceBetweenPlayer[PoolManager.weaponIndex],
            0
        );

        temp.transform.localEulerAngles = new Vector3(
            0,
            0,
            UnityEngine.Random.Range(
                -currentDeviation
                    / GunStatManager.instance[(GunKind)PoolManager.weaponIndex].accuracy,
                currentDeviation
                    / GunStatManager.instance[(GunKind)PoolManager.weaponIndex].accuracy
            )
        );
        temp.transform.parent = null;

        Rigidbody2D rb = GetCachedComponent(temp, rbCache);
        if (rb)
            rb.linearVelocity = shotSpeed * TechTreeUnlock.shotSpeed * temp.transform.up;

        SummonObject summonObject = GetCachedComponent(temp, summonObjectCache);
        if (summonObject)
            summonObject.StartCoroutine(
                WaitAction.wait(
                    7f * TechTreeUnlock.gunRange,
                    () =>
                    {
                        PoolManager.Pool.Release(temp);
                    }
                )
            );
    }

    public override void Clear()
    {
        rbCache.Clear();
        bulletCache.Clear();
        summonObjectCache.Clear();
    }
}
