using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berserker : BasicZombie
{
    [SerializeField]
    private float baseHealPerSecond = 1.0f; // 기본 초당 재생량

    protected override void Update()
    {
        if (isDead) return;
        float lostHpRatio = 1f - (hp / HP);
        float originalSpeed = speed;
        speed = originalSpeed * (1f + lostHpRatio);
        float originalAttackCool = attackCool;
        attackCool = originalAttackCool * (1f - lostHpRatio);
        float currentHealPerSecond = baseHealPerSecond * (1f + lostHpRatio);
        base.Update();
        speed = originalSpeed;
        attackCool = originalAttackCool;
        if (hp < HP)
        {
            hp = Mathf.Clamp(hp + (currentHealPerSecond * Time.deltaTime), 0f, HP);
            HpBar = hp / HP;
        }
    }

    protected override void Attack(Transform target)
    {
        float lostHpRatio = 1f - (hp / HP);
        int originalDamage = damage;
        int bonusDamage = Mathf.RoundToInt(originalDamage * lostHpRatio);
        damage = originalDamage + bonusDamage;

        base.Attack(target);

        damage = originalDamage;
    }
}