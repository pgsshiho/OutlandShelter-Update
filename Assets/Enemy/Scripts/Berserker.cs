using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berserker : BasicZombie
{
    [SerializeField]
    private float healPerSecond = 1.0f;

    protected override void Update()
    {
        base.Update();
        if (!isDead && hp < HP)
        {
            hp = Mathf.Clamp(hp + (healPerSecond * Time.deltaTime), 0f, HP);
            HpBar = hp / HP;
        }
    }
    protected override void Attack(Transform target)
    {
        float lostHpRatio = 1f - (hp / HP);
        int originalDamage = damage;
        int bonusDamage = Mathf.RoundToInt(lostHpRatio * 3f);
        damage = originalDamage + bonusDamage;
        base.Attack(target);
        damage = originalDamage;
    }
}