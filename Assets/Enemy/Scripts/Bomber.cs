using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : BasicZombie
{
    [Header("Bomber Settings")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionDamage = 10f;
    [SerializeField] private float selfDestructRange = 1.5f;
    [SerializeField] private LayerMask targetLayer;

    private bool hasExploded = false;
    private bool isExploding = false; // 애니메이션 재생 중인지 체크s

    protected override void OnEnable()
    {
        base.OnEnable();
        hasExploded = false;
        isExploding = false;
    }

    protected override void Update()
    {
        // 이미 죽었거나, 자폭 애니메이션 중이면 로직 중단
        if (isDead || hasExploded || isExploding)
        {
            if (isExploding) rb.linearVelocity = Vector2.zero; // 자폭 중엔 멈춤
            return;
        }

        target = SelectTarget();

        if (target != null)
        {
            float distance = Vector2.Distance(Position, targetPos);

            if (distance <= selfDestructRange)
            {
                // 즉시 Death()가 아닌 자폭 시퀀스 시작
                StartCoroutine(SelfDestructSequence());
                return;
            }

            Vector2 direction = (targetPos - Position).normalized;
            spriteRenderer.flipX = direction.x <= 0 && (direction.x < 0 || spriteRenderer.flipX);
            rb.linearVelocity = direction * speed;
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 1000f);
        if (hpBar.transform.parent.gameObject.activeSelf)
            hpBar.transform.parent.position = Camera.main.WorldToScreenPoint(transform.position + (Vector3)offset);
    }

    private IEnumerator SelfDestructSequence()
    {
        isExploding = true;
        rb.linearVelocity = Vector2.zero; // 이동 정지

        // 1. 공격(자폭) 애니메이션 트리거
        anim.SetTrigger("Attack");

        // 2. 애니메이션에서 폭발이 일어나는 타이밍까지 대기
        // 만약 애니메이션의 특정 스프레이트(예: 3번)에서 터져야 한다면 WaitAction.wait을 사용해도 됩니다.
        // 여기서는 예시로 0.5초 뒤에 터지게 설정했습니다.
        yield return new WaitForSeconds(0.5f);

        // 3. 실제 폭발 로직 실행
        if (!isDead) Death();
    }

    public override void Death()
    {
        if (hasExploded) return;
        hasExploded = true;
        isDead = true;

        // 범위 피해 및 감염 처리
        Explode();

        // 부모의 Death 호출 (경험치, 재화 지급 및 오브젝트 풀 반환)
        base.Death();
    }

    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(Position, explosionRadius, targetLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable damageable))
            {
                Vector2 dir = ((Vector2)hit.transform.position - (Vector2)Position).normalized;
                damageable.Damage(explosionDamage, dir * knockBackForce, AttackType.Explosion, 0.1f);
            }

            if (hit.TryGetComponent(out PlayerMove player))
            {
                // 플레이어 감염 확률 로직
                if (Random.Range(0, 10) > 8)
                {
                    Personal_resource.instance.isInfect = true;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, selfDestructRange);

        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}