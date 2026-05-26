using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonShotGunEffect : SummonObject
{
    private Animator anim;
    private Collider2D col;

    [HideInInspector]
    public bool isAuto = false;

    private readonly List<Collider2D> damagedEnemies = new();

    [SerializeField]
    private LayerMask enemyLayer;

    protected override void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        if (col is BoxCollider2D boxCol)
        {
            // 0.5f나 1.0f 정도 수치를 조절하며 테스트해보세요.
            boxCol.offset = new Vector2(0, -0.5f);
            // 필요하다면 크기도 살짝 키워줄 수 있습니다.
            boxCol.size = new Vector2(boxCol.size.x, boxCol.size.y + 1.0f);
        }
    }

    protected override void OnEnable()
    {
        anim.SetTrigger("Attack");

        col.Overlap(
            new ContactFilter2D()
            {
                layerMask = enemyLayer,
                useLayerMask = true,
                useTriggers = true,
            },
            damagedEnemies
        );
        foreach (Collider2D enemyCol in damagedEnemies)
        {
            if (enemyCol.TryGetComponent(out IEnemyDamage enemy))
            {
                Vector2 direction = (enemyCol.transform.position - transform.position).normalized;
                Attack(enemy, direction);
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other) { }

    protected override void Attack(IEnemyDamage enemy, Vector2 direction)
    {
        if (Random.Range(0, 1) < TechTreeUnlock.increaseMoveSpeedProbability)
        {
            overWrap++;
            TechTreeUnlock.moveSpeed = 1.1f;

            ObjectPoolManager
                .instance[Kind.Gun]
                .StartCoroutine(
                    WaitAction.wait(
                        2f,
                        () =>
                        {
                            overWrap--;

                            if (overWrap == 0)
                                TechTreeUnlock.moveSpeed = 1;
                        }
                    )
                );
        }

        enemy.Damage(
            damage
                * GunStatManager
                    .instance[(GunKind)ObjectPoolManager.instance[Kind.Gun].weaponIndex]
                    .damage
                * (PlayerAvoidSkill.damageUp ? TechTreeUnlock.afterAvoidDamage : 1)
                * (isAuto ? TechTreeUnlock.autoGunDamage : 1)
                * TechTreeUnlock.weaponDamage,
            knockBackForce * direction
        );
    }
}
