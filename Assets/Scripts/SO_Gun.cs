using System;
using System.Collections;
using UnityEngine;

public abstract class SO_Gun : ScriptableObject
{
    [Header("Gun Setting")]
    public float deviation;

    public int oneMagazine;

    public float relodingTime;

    public int shotSpeed = 20;

    [Header("위치 보정 (X는 좌우, Height는 높이)")]
    public float fireXOffset = 0f; // X축 미세조정용

    public float characterHandHeight = 0.7f;

    [SerializeField]
    protected bool isAuto;

    private ObjectPoolManager poolManager;
    protected ObjectPoolManager PoolManager =>
        poolManager = poolManager != null ? poolManager : ObjectPoolManager.instance[Kind.Gun];

    public abstract int UsingBullet { get; set; }

    public abstract void Shoot(Weapons.Gun gun);

    public abstract IEnumerator Reload(
        Weapons.Gun gun,
        Func<int> loadedBulletGetter,
        Action<int> loadedBulletSetter,
        int capacity,
        float reloadTime
    );

    public virtual void Clear() { }
}
