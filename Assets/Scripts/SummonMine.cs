using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SummonMine : SummonObject
{
    [SerializeField] private GameObject bomb;
    public IObjectPool<GameObject> pool;
    [SerializeField] private float radius = 2;
    CircleCollider2D circleCollider;

    protected override void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    protected override void OnEnable()
    {
        circleCollider.radius = 0.5f;
    }

    protected override void Attack(IEnemyDamage enemy, Vector2 direction)
    {
        if (Random.Range(0, 1) < TechTreeUnlock.increaseMoveSpeedProbability)
        {
            overWrap++;
            TechTreeUnlock.moveSpeed = 1.1f;

            ObjectPoolManager.instance[Kind.Mine].StartCoroutine(WaitAction.wait(2f, () =>
            {
                overWrap--;

                if (overWrap == 0) TechTreeUnlock.moveSpeed = 1;
            }));
        }

        GetComponent<CircleCollider2D>().radius = radius;

        enemy.Damage(damage * (PlayerAvoidSkill.damageUp ? TechTreeUnlock.afterAvoidDamage : 1) * TechTreeUnlock.mineDamage
             * TechTreeUnlock.weaponDamage, knockBackForce * direction);

        SoundManager.SFX.PlayOneShot(SFXReference.Instance.bomb);
        Instantiate(bomb, transform.position, Quaternion.identity);

        Weapons.Mine.currentCount--;
        pool.Release(gameObject);
    }
}
