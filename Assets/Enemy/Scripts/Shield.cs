using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : BasicZombie
{
    [Header("Shield Mechanics")]
    [SerializeField]
    private float defenseStackUnit = 5.0f;

    [SerializeField]
    private float maxShieldDefense = 80.0f;

    [SerializeField]
    private float ShieldDuration = 1.5f;
    [SerializeField]
    private float defenseDecaySpeed = 20.0f;

    private float currentBonusDefense = 0f;
    private float lastHitTime = 0f;

    protected override void Update()
    {
        if (Time.time - lastHitTime > ShieldDuration)
        {
            if (currentBonusDefense > 0f)
            {
                currentBonusDefense -= defenseDecaySpeed * Time.deltaTime;
                currentBonusDefense = Mathf.Max(0f, currentBonusDefense);
            }
        }
        float originalDefense = defense;
        defense = originalDefense + currentBonusDefense;
        base.Update();
        defense = originalDefense;
    }

    public override void Damage(float damage, Vector2 knockBack = default)
    {
        lastHitTime = Time.time;
        float originalDefense = defense;
        defense = Mathf.Min(100f, originalDefense + currentBonusDefense);

        base.Damage(damage, knockBack);

        defense = originalDefense;

        if (!isDead)
        {
            currentBonusDefense += defenseStackUnit;
            currentBonusDefense = Mathf.Min(currentBonusDefense, maxShieldDefense);

        }
    }
}