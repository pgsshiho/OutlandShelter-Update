using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "DBS", menuName = "Gun/DBS")]
public class SO_DBS : SO_Gun
{
    private readonly Dictionary<GameObject, SummonShotGunEffect> effectCache = new();
    private readonly Dictionary<GameObject, Animator> animCache = new();

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
        get => BulletManager.shotGunBullet;
        set => BulletManager.shotGunBullet = value;
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
        SoundManager.SFX.PlayOneShot(SFXReference.Instance.shotgun);
        PlayerMove.canMove = false;

        GameObject temp = PoolManager.Pool.Get();
        temp.transform.parent = gun.AttackPivot;
        Camera.main.transform.DOComplete(); // 이전 흔들림 캔슬로 딜레이 방지
        Camera.main.transform.DOShakePosition(0.4f, 0.6f, 20, 90, false, true);
        temp.transform.localScale =
            GunStatManager.instance[(GunKind)PoolManager.weaponIndex].range
            * TechTreeUnlock.gunRange
            * PoolManager.summonPrefab[PoolManager.weaponIndex].transform.localScale;

        // 수정: 샷건도 X축 오프셋 반영 및 Y축 거리 고정
        float shotgunForwardOffset = 0.1f;
        temp.transform.localPosition = new Vector3(
            fireXOffset,
            characterHandHeight
                + gun.distanceBetweenPlayer[PoolManager.weaponIndex]
                + shotgunForwardOffset,
            0
        );

        temp.transform.localEulerAngles = Vector3.zero;
        temp.transform.parent = null;

        SummonShotGunEffect summonShotGunEffect = GetCachedComponent(temp, effectCache);
        if (summonShotGunEffect)
            summonShotGunEffect.isAuto = isAuto;

        gun.StartCoroutine(
            WaitAction.waitOneFrame(() =>
            {
                Animator anim = GetCachedComponent(temp, animCache);
                if (anim)
                {
                    gun.StartCoroutine(
                        WaitAction.wait(
                            () =>
                            {
                                return !anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
                            },
                            () =>
                            {
                                PoolManager.Pool.Release(temp);
                            }
                        )
                    );
                }
            })
        );

        gun.StartCoroutine(
            WaitAction.wait(
                (
                    gun.coolTime[PoolManager.weaponIndex]
                    / GunStatManager.instance[(GunKind)PoolManager.weaponIndex].attackSpeed
                )
                    / TechTreeUnlock.attackSpeed
                    / 2,
                () =>
                {
                    PlayerMove.canMove = true;
                }
            )
        );
    }

    public override void Clear()
    {
        effectCache.Clear();
        animCache.Clear();
    }
}
