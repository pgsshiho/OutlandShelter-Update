using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haze : BasicZombie
{
    private bool isSkillUsable = true;

    [Header("Haze Settings")]
    public GameObject gasCloudPrefab;
    public GameObject trailSmokePrefab;
    public GameObject phase2SmokePrefab;

    private bool isSpecialSkillUsing = false;
    private float trailTimer = 0f;
    private MapManager mm;

    private bool isPhase2 = false;

    protected override void Awake()
    {
        base.Awake();
        mm = FindAnyObjectByType<MapManager>();
    }

    protected override void OnEnable()
    {
        // 1. 먼저 부모의 OnEnable을 실행시켜서 일반적인 초기화 세팅을 돌립니다.
        // (이 과정에서 HP가시적으로 20 등으로 일시적 계산됨)
        base.OnEnable();

        // 2. 부모의 계산이 끝나자마자 보스 전용 스펙으로 "강제 덮어쓰기" 진행
        HP = 3000f; // 🌟 프리팹에 적어둔 보스 최대 체력 강제 지정
        hp = HP;    // 현재 체력도 만땅으로 동기화

        // 체력바 UI 비율도 3000 기준으로 다시 갱신 (1f = 100%)
        HpBar = hp / HP;

        // 기존 헤이즈 전용 플래그 초기화 및 타임어택 세팅
        isPhase2 = false;
        isSpecialSkillUsing = false;
        isSkillUsable = true;

        if (mm != null)
        {
            mm.waveTimerLimit = 300f;
        }
    }

    protected override void Update()
    {
        if (isDead) return;

        target = SelectTarget();
        if (target != null)
        {
            targetPos = target.transform.position + (Vector3)target.GetComponent<Collider2D>().offset;
        }

        // 부모의 변수(hp, HP)를 기준으로 비율 계산
        float hpRatio = hp / HP;

        // 🚨 [2페이즈] 체력 50% 이하 진입 시 연기 프리팹 및 독 상태 강화
        if (hpRatio <= 0.5f && !isPhase2)
        {
            isPhase2 = true;
            if (phase2SmokePrefab != null)
            {
                trailSmokePrefab = phase2SmokePrefab;
            }
        }

        if (!isSpecialSkillUsing)
        {
            if (target != null)
            {
                Vector2 direction = (targetPos - Position).normalized;
                HandleSpriteFlip(direction);
                rb.linearVelocity = direction * speed;

                trailTimer += Time.deltaTime;
                if (trailTimer >= 0.2f)
                {
                    Instantiate(trailSmokePrefab, transform.position, Quaternion.identity);
                    trailTimer = 0f;
                }
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }

            if (isSkillUsable && target != null)
            {
                isSkillUsable = false;
                StartCoroutine(SkillRoutine());
            }
        }

        UpdateUIAndSorting();
    }

    private void UpdateUIAndSorting()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 1000f);

        if (hpBar != null && hpBar.transform.parent != null && hpBar.transform.parent.gameObject.activeSelf)
        {
            hpBar.transform.parent.position = Camera.main.WorldToScreenPoint(transform.position + (Vector3)offset);
        }
    }

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
        GameObject hitPlayer = null;

        while (elapsed < 3f)
        {
            rb.linearVelocity = rushDir * (speed * 4f);
            elapsed += Time.deltaTime;

            Collider2D hit = Physics2D.OverlapCircle(transform.position, 1.5f, LayerMask.GetMask("Player"));
            if (hit != null)
            {
                hitPlayer = hit.gameObject;
                break;
            }
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("Strike");

        yield return new WaitForSeconds(0.5f);

        ApplyRushDamage(hitPlayer);

        yield return new WaitForSeconds(0.5f);
        isSpecialSkillUsing = false;
    }

    private void ApplyRushDamage(GameObject playerObj)
    {
        if (playerObj != null && playerObj.TryGetComponent(out IDamageable p))
        {
            Vector2 pushDir = (playerObj.transform.position - transform.position).normalized;
            p.Damage(30, pushDir * 20f, AttackType.Close, 0.1f);

            if (playerObj.TryGetComponent(out PoisonStatus ps))
            {
                ps.ApplyPoison(isPhase2);
            }
        }

        Collider2D[] near = Physics2D.OverlapCircleAll(transform.position, 4f);
        foreach (var c in near)
        {
            if (c.gameObject != playerObj && c.TryGetComponent(out PlayerMove pm))
            {
                if (c.TryGetComponent(out IDamageable sideTarget))
                {
                    Vector2 pushDir = (c.transform.position - transform.position).normalized;
                    sideTarget.Damage(15, pushDir * 20f, AttackType.Close, 0.1f);
                }

                if (c.TryGetComponent(out PoisonStatus ps))
                {
                    ps.ApplyPoison(isPhase2);
                }
            }
        }
    }

    // 평타(일반 밀착공격) 피격 시에도 중독 전이 처리
    protected override void Attack(Transform target)
    {
        base.Attack(target);

        if (target != null && target.TryGetComponent(out PlayerMove pm))
        {
            if (target.TryGetComponent(out PoisonStatus ps))
            {
                ps.ApplyPoison(isPhase2);
            }
        }
    }

    // 🏆 제한 시간 내 처치 성공 시 타임어택 플래그 리셋 및 정지 연동
    public override void Death()
    {
        if (isDead) return;

        if (mm != null)
        {
            mm.StopBossTimer();
        }

        base.Death();
    }
}