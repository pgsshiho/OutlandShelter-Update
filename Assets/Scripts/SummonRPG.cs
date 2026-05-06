using UnityEngine;
using UnityEngine.Pool;

public class SummonRPG : SummonObject
{
    public IObjectPool<GameObject> pool;
    public GameObject explosionEffect; // 폭발 이펙트 프리팹
    public float explosionRadius = 3f;

    protected override void Awake()
    {
        // 필요시 초기화 코드 작성
    }

    protected override void OnEnable()
    {
        // 필요시 활성화 시 동작 추가
    }

    protected override void Attack(IEnemyDamage enemy, Vector2 direction)
    {
        Explode();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
    }

    void Explode()
    {
        // 주변 적 탐색 및 데미지
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D obj in objects)
        {
            if (obj.TryGetComponent(out IEnemyDamage enemy))
            {
                enemy.Damage(damage * TechTreeUnlock.weaponDamage, Vector2.zero);
            }
        }

        // 이펙트 생성 및 풀 복귀
        // Instantiate(explosionEffect, transform.position, Quaternion.identity);
        pool.Release(gameObject);
    }
}