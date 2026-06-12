using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BasicZombie : MonoBehaviour, IEnemyDamage
{
    public int bonusVib;
    public float bonusStrength;
    public float speed = 3.0f;
    public Transform target;
    public float HP = 10;
    protected float hp;
    public int damage = 3;
    public float attackCool = 0.7f;
    protected bool canAttack = true;
    protected Rigidbody2D rb;
    protected LayerMask wall;
    public float defense;
    [SerializeField]
    protected Image hpBar;
    public bool isShake;
    [SerializeField]
    protected float knockBackForce;
    public static int deathCount = 0;

    [SerializeField]
    protected int dropExp;

    [SerializeField]
    protected int dropMatarial;
    protected bool isDead = false;
    private float currentSlowMultiplier = 1.0f; // 현재 둔화 배율 (1 = 정상 속도)
    private Coroutine slowCoroutine;
    protected float HpBar
    {
        set
        {
            if (!hpBar.transform.parent.gameObject.activeSelf)
                StartCoroutine(
                    WaitAction.wait(
                        5f,
                        () =>
                        {
                            hpBar.transform.parent.gameObject.SetActive(false);
                        }
                    )
                );

            hpBar.transform.parent.gameObject.SetActive(true);
            hpBar.fillAmount = value;
        }
    }

    public Vector2 offset = new(0, 1.5f);
    protected Animator anim;
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    protected float range = 2;

    [SerializeField]
    protected int zombieIndex;
    protected Collider2D col;

    protected Vector3 Position
    {
        get { return transform.position + (Vector3)col.offset; }
    }

    protected Vector3 targetPos;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        wall = LayerMask.GetMask("Wall");
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isShake = false;
    }

    protected virtual void OnEnable()
    {
        col.enabled = true;
        isDead = false;

        if (MainmenuManager.isLong)
        {
            // 20웨이브 기준: 초기 체력 80%, 웨이브당 증가치 60%
            HP = 8f + (6f * MapManager.waveCount);
            // 공격력 70% 수준 (3 * 0.7 = 2.1 -> 약 2)
            damage = Mathf.Max(1, Mathf.RoundToInt(3f * 0.7f));
        }
        else
        {
            HP = 10f + (10f * MapManager.waveCount);
            damage = 3;
        }

        hp = HP;
        HpBar = hp / HP;

        if (hpBar.transform.parent.gameObject.activeSelf)
            hpBar.transform.parent.position = Camera.main.WorldToScreenPoint(
                transform.position + (Vector3)offset
            );

        Transform temp = hpBar.transform.parent;
        while (temp.parent != null)
        {
            temp = temp.parent;
        }

        canAttack = true;
        temp.gameObject.SetActive(true);
        currentSlowMultiplier = 1.0f;
        if (slowCoroutine != null) StopCoroutine(slowCoroutine);
        slowCoroutine = null;
    }

    protected virtual void Update()
    {
        target = SelectTarget();

        Collider2D[] hits = Physics2D.OverlapCircleAll(Position, range);
        bool inRange = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.transform == target)
            {
                inRange = true;
                break;
            }
        }

        if (!inRange)
        {
            if (target != null)
            {
                Vector2 direction = (targetPos - Position).normalized;
                spriteRenderer.flipX = direction.x <= 0 && (direction.x < 0 || spriteRenderer.flipX);

                // ★ 기존 속도(speed)에 '둔화 배율'과 '사망 시 속도 버프(increaseSpeed)'를 함께 적용
                rb.linearVelocity = direction * (speed * currentSlowMultiplier * increaseSpeed);
            }
        }
        else if (canAttack)
        {
            canAttack = false;
            anim.SetTrigger("Attack");

            bool temp = target.TryGetComponent<PlayerAvoidSkill>(out _);

            if (temp)
            {
                StartCoroutine(
                    WaitAction.wait(
                        () =>
                        {
                            return TechTreeUnlock.additionalAvoidAbleTiming
                                || spriteRenderer.sprite.name[^1] == '2';
                        },
                        () =>
                        {
                            Vector2 direction = (targetPos - Position).normalized;
                            if (Random.Range(0f, 1f) < TechTreeUnlock.avoidProbability)
                                PlayerAvoidSkill.SkillUse(direction, true);
                            PlayerAvoidSkill.useable = true;
                            PlayerAvoidSkill.targetPos = Position;
                        }
                    )
                );
            }

            StartCoroutine(
                WaitAction.wait(
                    () =>
                    {
                        return spriteRenderer.sprite.name[^1] == '3';
                    },
                    () =>
                    {
                        if (temp)
                            PlayerAvoidSkill.useable = false;
                        Attack(target);
                    }
                )
            );
            StartCoroutine(
                WaitAction.wait(
                    attackCool,
                    () =>
                    {
                        canAttack = true;
                    }
                )
            );
        }

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            transform.position.y / 1000f
        );

        if (hpBar.transform.parent.gameObject.activeSelf)
            hpBar.transform.parent.position = Camera.main.WorldToScreenPoint(
                transform.position + (Vector3)offset
            );
    }
    public void ApplySlow(float slowPercent, float duration)
    {
        if (isDead) return;

        // 기존에 걸려있던 둔화 코루틴이 있다면 인터셉트(취소)하고 새로 시작
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }

        slowCoroutine = StartCoroutine(SlowRoutine(slowPercent, duration));
    }

    private IEnumerator SlowRoutine(float slowPercent, float duration)
    {
        // 예: 0.4(40%) 둔화면, 이동 속도는 원래의 0.6배(60%)가 됨
        currentSlowMultiplier = Mathf.Clamp01(1.0f - slowPercent);

        yield return new WaitForSeconds(duration);

        // 시간 다 되면 원래 속도로 복구
        currentSlowMultiplier = 1.0f;
        slowCoroutine = null;
    }
    protected virtual void Attack(Transform target)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(Position, range);
        bool inRange = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.transform == target)
            {
                inRange = true;
                break;
            }
        }

        if (inRange)
        {
            if (target.TryGetComponent(out IFacility facility))
            {
                facility.Damage(damage);
            }
            else if (target.TryGetComponent(out ITurret turret))
            {
                turret.Damage(damage);
            }
            else if (target.TryGetComponent(out ICenter center))
            {
                center.Damage(damage);
            }
            else if (target.TryGetComponent(out IDamageable player))
            {
                Vector2 direction = (targetPos - Position).normalized;
                player.Damage(damage, direction * knockBackForce, AttackType.Close, 0.1f);
                SoundManager.SFX.PlayOneShot(SFXReference.Instance.Hit);
                if (Camera.main != null)
                {
                    Camera.main.transform.DOComplete();
                    float shakeDuration = 0.05f + (damage * 0.015f);
                    float shakeStrength = 0.05f + (damage * 0.025f) + bonusStrength;
                    int shakeVibrato = Mathf.Clamp(5 + damage, 5, 25) + bonusVib;
                    Camera.main.transform.DOShakePosition(
                        shakeDuration,
                        shakeStrength,
                        shakeVibrato,
                        90,
                        false,
                        true
                    );
                }
            }
        }
    }

    protected readonly List<Collider2D> hits = new(100);
    protected readonly List<Collider2D> targets = new();

    protected virtual Transform SelectTarget()
    {
        targets.Clear();
        ContactFilter2D filter = new()
        {
            layerMask = ~wall,
            useLayerMask = true,
            useTriggers = true,
        };
        Physics2D.OverlapCircle(Position, 1000, filter, hits);

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<IEnemyAttackable>(out var attackable))
            {
                if (attackable is MonoBehaviour mono && mono.enabled == false)
                {
                    continue;
                }

                targets.Add(hit);
            }
        }

        if (targets.Count != 0)
        {
            targets.Sort(
                (c1, c2) =>
                {
                    if (c1 is IEnemyAttackable e1 && c2 is IEnemyAttackable e2)
                    {
                        int p1 = e1.GetPriority();
                        int p2 = e2.GetPriority();
                        if (p1 != p2)
                            return p1.CompareTo(p2);
                    }

                    return Vector2
                        .Distance(Position, c1.transform.position)
                        .CompareTo(Vector2.Distance(Position, c2.transform.position));
                }
            );
            targetPos = targets[0].transform.position + (Vector3)targets[0].offset;
            return targets[0].transform;
        }
        else
        {
            targetPos = Position;
            return null;
        }
    }

    public static float increaseSpeed = 1;
    public static float stack = 0;

    public virtual void Death()
    {
        if (isDead)
            return;
        isDead = true;
        stack++;
        increaseSpeed =
            1
            + TechTreeUnlock.continuousIncreaseMoveSpeed
                * Mathf.Clamp(stack, 0, TechTreeUnlock.S22MAXOVERWRAP);
        StartCoroutine(
            WaitAction.wait(
                3f,
                () =>
                {
                    stack--;
                }
            )
        );
        col.enabled = false;
        deathCount++;

        MapManager.currentZombieCount--;
        int finalExp = MainmenuManager.isLong
            ? Mathf.Max(1, Mathf.RoundToInt(dropExp * 0.6f))
            : dropExp;
        int finalMat = MainmenuManager.isLong
            ? Mathf.Max(1, Mathf.RoundToInt(dropMatarial * 0.6f))
            : dropMatarial;

        Personal_resource.CurExp += finalExp;
        Personal_resource.instance.Metal += finalMat;

        Notion.Log($"+{finalExp}EXP, +{finalMat}Metal");
        if (SFXReference.Instance.zombieDie != null)
        {
            SoundManager.SFX.PlayOneShot(SFXReference.Instance.zombieDie, 0.5f);
        }
        GameObject temp = ObjectPoolManager.instance[Kind.ZombieDeathEffect].Pool.Get();
        temp.transform.position = transform.position;

        ObjectPoolManager
            .instance[Kind.ZombieDeathEffect]
            .StartCoroutine(
                WaitAction.wait(
                    0.4f,
                    () =>
                    {
                        ObjectPoolManager.instance[Kind.ZombieDeathEffect].Pool.Release(temp);
                    }
                )
            );

        ObjectPoolManager.instance[Kind.Zombie].weaponIndex = zombieIndex;
        ObjectPoolManager.instance[Kind.Zombie].Pool.Release(gameObject);
    }

    protected void OnDisable()
    {
        Transform temp = hpBar.transform.parent;
        while (temp.parent != null)
        {
            temp = temp.parent;
        }

        temp.gameObject.SetActive(false);
    }

    public virtual void Damage(float damage, Vector2 knockBack = default)
    {
        if (isDead)
            return;
        float finalDamage = damage * Mathf.Max(0, (1f - (defense / 100f)));

        hp = Mathf.Clamp(hp - finalDamage, 0, HP);
        HpBar = hp / HP;
        OnHitPolished();
        if (knockBack != Vector2.zero)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            rb.linearVelocity = knockBack;
        }

        if (hp == 0)
        {
            Death();
        }
        if (Camera.main != null && !isShake)
        {
            isShake = true;
            Camera.main.transform.DOComplete();
            float shakeDuration = Mathf.Min(0.08f + (damage * 0.01f), 0.3f);

            // [추천 공식] 기본 0.1 + 데미지당 0.02 추가 (최대치 0.5 제한)
            float shakeStrength = Mathf.Min(0.1f + (damage * 0.01f), 0.5f);

            // 진동 횟수도 데미지에 따라 촘촘하게 설정
            int shakeVibrato = Mathf.Clamp(5 + Mathf.RoundToInt(damage), 8, 50);
            Camera.main.transform.DOShakePosition(
                shakeDuration,
                shakeStrength,
                shakeVibrato,
                90,
                false,
                true
            ).OnComplete(() => isShake = false);
        }
    }
    public void OnHitPolished()
    {
        // 기존에 작동 중인 트윈이 있다면 꼬이지 않게 종료
        transform.DOKill();
        spriteRenderer.DOKill();

        // 원래 상태로 초기화 (연타로 맞을 때를 대비)
        spriteRenderer.color = Color.white;
        transform.localScale = Vector3.one;

        // DOTween 시퀀스 생성
        Sequence hitSeq = DOTween.Sequence();

        // [연출 1] 맞자마자 0.03초 만에 흰색(or빨간색)으로 변하고, 크기는 1.2배로 커짐
        hitSeq.Join(spriteRenderer.DOColor(Color.red, 0.03f));
        hitSeq.Join(transform.DOScale(1.2f, 0.03f));

        // [연출 2] 그 다음 0.05초 만에 원래 색과 원래 크기로 복귀
        hitSeq.Append(spriteRenderer.DOColor(Color.white, 0.1f));
        hitSeq.Append(transform.DOScale(1.0f, 0.1f));
    }
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer != wall && collision.gameObject.layer != gameObject.layer)
        {
            target = collision.transform;
            targetPos = collision.transform.position + (Vector3)collision.collider.offset;
        }
    }
}
