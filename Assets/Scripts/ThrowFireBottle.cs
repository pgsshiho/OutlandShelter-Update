using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowFireBottle : SummonThrow
{
    [SerializeField] private GameObject fire;

    protected override void Skill()
    {
        transform.localScale = ObjectPoolManager.instance[Kind.Throw].summonPrefab[1].transform.localScale * TechTreeUnlock.throwScale;

        base.Skill();

        SoundManager.SFX.PlayOneShot(SFXReference.Instance.fire);
        anim.SetTrigger("Boom");

        Instantiate(fire, transform.position, Quaternion.identity);

        StartCoroutine(WaitAction.waitOneFrame(() =>
        {
            StartCoroutine(WaitAction.wait(() => !anim.GetCurrentAnimatorStateInfo(0).IsTag("Skill"), () => pool.Release(gameObject)));
        }));
    }

    protected override void Attack(IEnemyDamage enemy, Vector2 direction)
    {
        if (Random.Range(0, 1) < TechTreeUnlock.increaseMoveSpeedProbability)
        {
            overWrap++;
            TechTreeUnlock.moveSpeed = 1.1f;

            ObjectPoolManager.instance[Kind.Throw].StartCoroutine(WaitAction.wait(2f, () =>
            {
                overWrap--;

                if (overWrap == 0) TechTreeUnlock.moveSpeed = 1;
            }));
        }

        enemy.Damage(damage * (PlayerAvoidSkill.damageUp ? TechTreeUnlock.afterAvoidDamage : 1) * TechTreeUnlock.weaponDamage, knockBackForce * direction);
    }
}
