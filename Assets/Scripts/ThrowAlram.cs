using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAlram : SummonThrow, IEnemyAttackable
{
    [SerializeField] private float skillTime = 5;

    protected override void Skill()
    {
        base.Skill();

        SoundManager.SFX.PlayOneShot(SFXReference.Instance.alram);
        anim.SetTrigger("Alram");

        StartCoroutine(WaitAction.wait(skillTime, () =>
        {
            pool.Release(gameObject);
        }));
    }
}
