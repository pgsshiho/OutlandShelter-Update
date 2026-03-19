using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonMelee : SummonObject
{
    private Animator anim;
    private static int combo = 0;
    private static int comboOverWrap = 0;
    private PlayerMove player;

    protected override void Awake()
    {
        anim = GetComponent<Animator>();
        player = FindAnyObjectByType<PlayerMove>();
        combo = 0;
        comboOverWrap = 0;
    }

    protected override void OnEnable()
    {
        anim.SetTrigger("Attack");
    }

    protected override void Attack(IEnemyDamage enemy, Vector2 direction)
    {
        if (Random.Range(0, 1) < TechTreeUnlock.increaseMoveSpeedProbability)
        {
            overWrap++;
            TechTreeUnlock.moveSpeed = 1.1f;

            ObjectPoolManager.instance[Kind.Melee].StartCoroutine(WaitAction.wait(2f, () =>
            {
                overWrap--;

                if (overWrap == 0) TechTreeUnlock.moveSpeed = 1;
            }));
        }

        combo++;
        comboOverWrap++;
        player.StartCoroutine(WaitAction.wait(1.5f, () =>
        {
            comboOverWrap--;

            if (comboOverWrap == 0) combo = 0;
        }));

        enemy.Damage(damage * (PlayerAvoidSkill.damageUp ? TechTreeUnlock.afterAvoidDamage : 1) * TechTreeUnlock.meleeDamage
            * (1 + (TechTreeUnlock.comboDamageIncrease * combo)) * TechTreeUnlock.weaponDamage, knockBackForce * direction);
    }
}
