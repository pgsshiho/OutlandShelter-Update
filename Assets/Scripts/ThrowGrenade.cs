using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrenade : SummonThrow
{
    [SerializeField] private GameObject fire;
    [SerializeField] private GameObject rangeExpect;
    Vector2 expectPoint;
    GameObject range;

    protected override void OnEnable()
    {
        base.OnEnable();

        StartCoroutine(WaitAction.waitOneFrame(() =>
        {
            expectPoint = new Vector2(transform.position.x + rb.linearVelocity.x * movingTime, transform.position.y + rb.linearVelocity.y * movingTime);
            range = Instantiate(rangeExpect, expectPoint, Quaternion.identity);
            range.transform.localScale = rangeExpect.transform.localScale * TechTreeUnlock.grenadeRange * TechTreeUnlock.throwScale;
        }));
    }

    protected override void Skill()
    {
        transform.localScale = ObjectPoolManager.instance[Kind.Throw].summonPrefab[2].transform.localScale * TechTreeUnlock.grenadeRange * TechTreeUnlock.throwScale;

        base.Skill();

        SoundManager.SFX.PlayOneShot(SFXReference.Instance.bomb);
        anim.SetTrigger("Boom");

        Instantiate(fire, transform.position, Quaternion.Euler(-90, 0, 0));
        
        Destroy(range);

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
