using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SummonBullet : SummonObject
{
    public IObjectPool<GameObject> pool;
    public int throughCount = 1;
    private int _throughCount;
    private TrailRenderer trailRenderer;
    [HideInInspector] public bool isAuto = false;

    protected override void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    protected override void OnEnable()
    {
        _throughCount = throughCount;

        StartCoroutine(WaitAction.wait(10, () => pool.Release(gameObject)));
    }

    protected override void Attack(IEnemyDamage enemy, Vector2 direction)
    {
        if (Random.Range(0, 1) < TechTreeUnlock.increaseMoveSpeedProbability)
        {
            overWrap++;
            TechTreeUnlock.moveSpeed = 1.1f;

            ObjectPoolManager.instance[Kind.Gun].StartCoroutine(WaitAction.wait(2f, () =>
            {
                overWrap--;

                if (overWrap == 0) TechTreeUnlock.moveSpeed = 1;
            }));
        }

        enemy.Damage(damage * GunStatManager.instance[(GunKind)ObjectPoolManager.instance[Kind.Gun].weaponIndex].damage
            * (PlayerAvoidSkill.damageUp ? TechTreeUnlock.afterAvoidDamage : 1) * (isAuto ? TechTreeUnlock.autoGunDamage : 1) * TechTreeUnlock.weaponDamage
            , knockBackForce * direction);
        if (--_throughCount <= 0) pool.Release(gameObject);
    }

    private void OnDisable()
    {
        trailRenderer.Clear();
    }
}
