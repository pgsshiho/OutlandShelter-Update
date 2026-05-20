using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haze : BasicZombie
{
    private bool isSkillUsable = true; // 스킬 사용 가능 여부

    [Header("Haze Settings")]
    public GameObject gasCloudPrefab;    // 패턴1: 가스 폭발 프리팹
    public GameObject trailSmokePrefab;  // 패시브1: 지나간 연기 프리팹

    private bool isSpecialSkillUsing = false;
    private float trailTimer = 0f;
    private MapManager mm;

    protected override void Awake()
    {
        base.Awake();
        mm = FindAnyObjectByType<MapManager>();

        if (mm != null) mm.waveTimerLimit = 300f;
    }

    protected override void Update()
    {
        // 1. 타겟 실시간 위치 동기화 및 부모 클래스의 타겟 선택 로직 보완
        target = SelectTarget();
        if (target != null)
        {
            targetPos = target.position;
        }

        // 2. 🌟 [체력바 버그 해결의 핵심] 스킬 사용 중이어도 '이동/공격'만 건너뛰고, UI 위치 갱신은 무조건 실행
        if (!isSpecialSkillUsing)
        {
            // 기본 이동 로직
            Vector2 direction = (targetPos - Position).normalized;
            HandleSpriteFlip(direction);

            // 스킬을 안 쓰고 있을 때만 순수 이동 속도 주입
            rb.linearVelocity = direction * speed;

            // 패시브1: 지나간 자리에 연기 생성 (0.2초마다)
            trailTimer += Time.deltaTime;
            if (trailTimer >= 0.2f)
            {
                Instantiate(trailSmokePrefab, transform.position, Quaternion.identity);
                trailTimer = 0f;
            }

            // 패턴 결정 로직
            if (isSkillUsable)
            {
                isSkillUsable = false;
                StartCoroutine(SkillRoutine());
            }
        }

        // 3. 🌟 [부모의 핵심 기능 강제 동기화] 
        // 렌더링 소팅을 위한 Z축 연산 및 UI(체력바) 스크린 좌표 추적은 return에 상관없이 매 프레임 실행되어야 합니다.
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 1000f);

        if (hpBar != null && hpBar.transform.parent != null && hpBar.transform.parent.gameObject.activeSelf)
        {
            hpBar.transform.parent.position = Camera.main.WorldToScreenPoint(transform.position + (Vector3)offset);
        }
    }

    // 앞/뒤/사이드 판정 로직 (블렌드 트리 파라미터 제어)
    private void HandleSpriteFlip(Vector2 dir)
    {
        if (dir == Vector2.zero) return;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            spriteRenderer.flipX = dir.x < 0;
            anim.SetFloat("DirX", 1f);
            anim.SetFloat("DirY", 0f);
        }
        else
        {
            anim.SetFloat("DirX", 0f);
            anim.SetFloat("DirY", dir.y > 0 ? 1f : -1f);
        }
    }

    private IEnumerator SkillRoutine()
    {
        int pattern = Random.Range(0, 2);

        if (pattern == 0) yield return StartCoroutine(GasExplosionPattern());
        else yield return StartCoroutine(RushPattern());

        yield return new WaitForSeconds(Random.Range(3f, 5f));
        isSkillUsable = true;
    }

    // 패턴 1: 가스 폭발
    private IEnumerator GasExplosionPattern()
    {
        isSpecialSkillUsing = true;

        Vector2 dirToTarget = (targetPos - Position).normalized;
        HandleSpriteFlip(dirToTarget);
        rb.linearVelocity = Vector2.zero;

        anim.SetTrigger("Gas");
        yield return new WaitForSeconds(0.5f);

        Instantiate(gasCloudPrefab, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(1f);
        isSpecialSkillUsing = false;
    }

    // 패턴 2: 돌격 (Run -> Strike)
    private IEnumerator RushPattern()
    {
        isSpecialSkillUsing = true;

        Vector2 rushDir = (targetPos - Position).normalized;
        HandleSpriteFlip(rushDir);

        spriteRenderer.DOColor(Color.red, 0.5f);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.8f);

        spriteRenderer.color = Color.white;
        anim.SetTrigger("Run");

        float elapsed = 0f;
        bool hitTarget = false;
        GameObject hitPlayer = null;

        while (elapsed < 3f)
        {
            rb.linearVelocity = rushDir * (speed * 4f);
            elapsed += Time.deltaTime;

            Collider2D hit = Physics2D.OverlapCircle(transform.position, 1.5f, LayerMask.GetMask("Player"));
            if (hit != null)
            {
                hitTarget = true;
                hitPlayer = hit.gameObject;
                break;
            }
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("Strike");

        yield return new WaitForSeconds(0.5f);

        if (hitTarget)
        {
            ApplyRushDamage(hitPlayer);
        }

        yield return new WaitForSeconds(0.5f);
        isSpecialSkillUsing = false;
    }

    private void ApplyRushDamage(GameObject playerObj)
    {
        if (playerObj.TryGetComponent(out IDamageable p)) p.Damage(30);

        Collider2D[] near = Physics2D.OverlapCircleAll(transform.position, 4f);
        foreach (var c in near)
        {
            if (c.TryGetComponent(out PlayerMove pm))
            {
                if (c.TryGetComponent(out PoisonStatus ps)) ps.ApplyPoison(false);

                Vector2 pushDir = (c.transform.position - transform.position).normalized;
                c.GetComponent<Rigidbody2D>()?.AddForce(pushDir * 20f, ForceMode2D.Impulse);
            }
        }
    }
}