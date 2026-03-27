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

        // 타임어택 설정 (4~6분 중 5분 설정)
        if (mm != null) mm.waveTimerLimit = 300f;
    }

    protected override void Update()
    {
        // 스킬 사용 중에는 기본 이동 및 방향 전환 중지
        if (isSpecialSkillUsing) return;

        // 기본 이동 로직 (Move 트리)
        Vector2 direction = (targetPos - Position).normalized;
        HandleSpriteFlip(direction);

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
            StartCoroutine(SkillRoutine());
        }
    }

    // 앞/뒤/사이드 판정 로직 (블렌드 트리 파라미터 제어)
    private void HandleSpriteFlip(Vector2 dir)
    {
        if (dir == Vector2.zero) return;

        // X축(좌우) 이동이 Y축(상하)보다 클 때 -> 사이드 모션
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            spriteRenderer.flipX = dir.x < 0;
            anim.SetFloat("DirX", 1f);
            anim.SetFloat("DirY", 0f);
        }
        // Y축 이동이 더 클 때 -> 앞/뒤 모션
        else
        {
            anim.SetFloat("DirX", 0f);
            // dir.y가 양수면 1(뒤), 음수면 -1(앞)
            anim.SetFloat("DirY", dir.y > 0 ? 1f : -1f);
        }
    }

    private IEnumerator SkillRoutine()
    {
        isSkillUsable = false;
        int pattern = Random.Range(0, 2);

        if (pattern == 0) yield return StartCoroutine(GasExplosionPattern());
        else yield return StartCoroutine(RushPattern());

        yield return new WaitForSeconds(Random.Range(3f, 5f)); // 쿨타임
        isSkillUsable = true;
    }

    // 패턴 1: 가스 폭발
    private IEnumerator GasExplosionPattern()
    {
        isSpecialSkillUsing = true;

        // 1. 가스를 뿜기 직전 플레이어 쪽으로 방향 갱신
        Vector2 dirToTarget = (targetPos - Position).normalized;
        HandleSpriteFlip(dirToTarget);
        rb.linearVelocity = Vector2.zero;

        // 2. Gas 블렌드 트리로 전환
        anim.SetTrigger("Gas");

        yield return new WaitForSeconds(0.5f); // 선딜레이 (가스 뿌리는 모션)

        // 가스 장판 생성
        Instantiate(gasCloudPrefab, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(1f); // 스킬 지속 시간 대기
        isSpecialSkillUsing = false;
    }

    // 패턴 2: 돌격 (Run -> Strike)
    private IEnumerator RushPattern()
    {
        isSpecialSkillUsing = true;

        // 1. 돌진 직전 플레이어 쪽으로 방향 갱신 (빨갛게 깜빡임)
        Vector2 rushDir = (targetPos - Position).normalized;
        HandleSpriteFlip(rushDir);

        spriteRenderer.DOColor(Color.red, 0.5f);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.8f);

        spriteRenderer.color = Color.white;

        // 2. Run 블렌드 트리로 전환
        anim.SetTrigger("Run");

        float elapsed = 0f;
        bool hitTarget = false;
        GameObject hitPlayer = null;

        // 3. 돌진 이동
        while (elapsed < 3f)
        {
            rb.linearVelocity = rushDir * (speed * 4f);
            elapsed += Time.deltaTime;

            // 충돌 체크
            Collider2D hit = Physics2D.OverlapCircle(transform.position, 1.5f, LayerMask.GetMask("Player"));
            if (hit != null)
            {
                hitTarget = true;
                hitPlayer = hit.gameObject;
                break; // 부딪히면 돌진 중단
            }
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        // 4. Strike 블렌드 트리로 전환
        anim.SetTrigger("Strike");

        // 찍는 애니메이션 타격 프레임까지 대기 (약 0.5초)
        yield return new WaitForSeconds(0.5f);

        // 데미지 판정
        if (hitTarget)
        {
            ApplyRushDamage(hitPlayer);
        }

        // 찍은 후 일어나는 후딜레이
        yield return new WaitForSeconds(0.5f);

        isSpecialSkillUsing = false;
    }

    private void ApplyRushDamage(GameObject playerObj)
    {
        // 직접 데미지 30
        if (playerObj.TryGetComponent(out IDamageable p)) p.Damage(30);

        // 주변 스플래시 15 및 밀쳐내기 (독 적용)
        Collider2D[] near = Physics2D.OverlapCircleAll(transform.position, 4f);
        foreach (var c in near)
        {
            if (c.TryGetComponent(out PlayerMove pm))
            {
                if (c.TryGetComponent(out PoisonStatus ps)) ps.ApplyPoison(false);

                // 밀쳐내기
                Vector2 pushDir = (c.transform.position - transform.position).normalized;
                c.GetComponent<Rigidbody2D>()?.AddForce(pushDir * 20f, ForceMode2D.Impulse);
            }
        }
    }
}