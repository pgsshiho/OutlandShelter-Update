using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonShotGunEffect : SummonObject
{
    private Animator anim;
    private Collider2D col;
    [HideInInspector] public bool isAuto = false;

    protected override void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    protected override void OnEnable()
    {
        col.enabled = true;
        anim.SetTrigger("Attack");
        StartCoroutine(WaitAction.waitOneFrame(() =>
        {
            col.enabled = false;
        }));
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
            * (PlayerAvoidSkill.damageUp ? TechTreeUnlock.afterAvoidDamage : 1) * (isAuto ? TechTreeUnlock.autoGunDamage : 1)
             * TechTreeUnlock.weaponDamage, knockBackForce * direction);
    }
}
