using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WalkerTuto : MonoBehaviour, IEnemyDamage
{
    public int bonusVib;
    public float bonusStrength;
    public float speed = 3.0f;
    public Transform target; // ◀ 항상 null 상태 유지 (AI 없음)
    public float HP = 10;
    protected float hp;
    public int damage = 3;
    public float attackCool = 0.7f;
    protected bool canAttack = true;
    protected Rigidbody2D rb;
    protected LayerMask wall;
    public float defense;
    [SerializeField] protected Image hpBar;
    public bool isShake;
    [SerializeField] protected float knockBackForce;
    public static int deathCount = 0;

    [SerializeField] protected int dropExp;
    [SerializeField] protected int dropMatarial;
    protected bool isDead = false;
    private float currentSlowMultiplier = 1.0f;
    private Coroutine slowCoroutine;

    protected float HpBar
    {
        set
        {
            if (!hpBar.transform.parent.gameObject.activeSelf)
                StartCoroutine(
                    WaitAction.wait(
                        5f,
                        () => { hpBar.transform.parent.gameObject.SetActive(false); }
                    )
                );

            hpBar.transform.parent.gameObject.SetActive(true);
            hpBar.fillAmount = value;
        }
    }

    public Vector2 offset = new(0, 1.5f);
    protected Animator anim;
    protected SpriteRenderer spriteRenderer;

    [SerializeField] protected float range = 2;
    [SerializeField] protected int zombieIndex;
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
            HP = 8f + (6f * MapManager.waveCount);
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
            hpBar.transform.parent.position = Camera.main.WorldToScreenPoint(transform.position + (Vector3)offset);

        Transform temp = hpBar.transform.parent;
        while (temp.parent != null) { temp = temp.parent; }

        canAttack = true;
        temp.gameObject.SetActive(true);
        currentSlowMultiplier = 1.0f;
        if (slowCoroutine != null) StopCoroutine(slowCoroutine);
        slowCoroutine = null;
    }

    protected virtual void Update()
    {
        // 💡 [변경] 타겟 지정 로직(SelectTarget)이 완전히 삭제되었습니다.
        // AI가 빠진 것처럼 움직이지 않도록 속도를 0으로 묶고 공격/추적을 건너뜁니다.
        rb.linearVelocity = Vector2.zero;

        // Y축 기준 레이어 소팅 및 체력바 UI 위치 동기화만 정상 작동시킵니다.
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 1000f);

        if (hpBar.transform.parent.gameObject.activeSelf)
            hpBar.transform.parent.position = Camera.main.WorldToScreenPoint(transform.position + (Vector3)offset);
    }

    public void ApplySlow(float slowPercent, float duration)
    {
        if (isDead) return;
        if (slowCoroutine != null) StopCoroutine(slowCoroutine);
        slowCoroutine = StartCoroutine(SlowRoutine(slowPercent, duration));
    }

    private IEnumerator SlowRoutine(float slowPercent, float duration)
    {
        currentSlowMultiplier = Mathf.Clamp01(1.0f - slowPercent);
        yield return new WaitForSeconds(duration);
        currentSlowMultiplier = 1.0f;
        slowCoroutine = null;
    }

    // 💡 타겟이 없으므로 실제로 실행되지는 않지만, 구조 유지를 위해 남겨둔 기본 Attack 메서드
    protected virtual void Attack(Transform target)
    {
        if (target == null) return;

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
            if (target.TryGetComponent(out IFacility facility)) { facility.Damage(damage); }
            else if (target.TryGetComponent(out ITurret turret)) { turret.Damage(damage); }
            else if (target.TryGetComponent(out ICenter center)) { center.Damage(damage); }
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
                    Camera.main.transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, 90, false, true);
                }
            }
        }
    }

    // 💡 [삭제 완료] SelectTarget() 메서드와 관련 가비지 컬렉션용 List(hits, targets)가 모두 제거되었습니다.

    public static float increaseSpeed = 1;
    public static float stack = 0;

    public virtual void Death()
    {
        if (Tutorial.instance.nowpage == 5 || Tutorial.instance.nowpage == 12)
        {
            Tutorial.instance.nextpage();
        }
        else
        {
            hp = HP;
            return;
        }
        if (isDead) return;
        isDead = true;
        stack++;
        increaseSpeed = 1 + TechTreeUnlock.continuousIncreaseMoveSpeed * Mathf.Clamp(stack, 0, TechTreeUnlock.S22MAXOVERWRAP);
        StartCoroutine(WaitAction.wait(3f, () => { stack--; }));
        col.enabled = false;
        deathCount++;

        MapManager.currentZombieCount--;
        int finalExp = MainmenuManager.isLong ? Mathf.Max(1, Mathf.RoundToInt(dropExp * 0.6f)) : dropExp;
        int finalMat = MainmenuManager.isLong ? Mathf.Max(1, Mathf.RoundToInt(dropMatarial * 0.6f)) : dropMatarial;

        Personal_resource.CurExp += finalExp;
        Personal_resource.instance.Metal += finalMat + 99999;

        Notion.Log($"+{finalExp}EXP, +{finalMat}Metal");
        if (SFXReference.Instance.zombieDie != null) SoundManager.SFX.PlayOneShot(SFXReference.Instance.zombieDie, 0.5f);

        GameObject temp = ObjectPoolManager.instance[Kind.ZombieDeathEffect].Pool.Get();
        temp.transform.position = transform.position;

        ObjectPoolManager.instance[Kind.ZombieDeathEffect].StartCoroutine(
            WaitAction.wait(0.4f, () => { ObjectPoolManager.instance[Kind.ZombieDeathEffect].Pool.Release(temp); })
        );

        ObjectPoolManager.instance[Kind.Zombie].weaponIndex = zombieIndex;
        ObjectPoolManager.instance[Kind.Zombie].Pool.Release(gameObject);
    }

    protected void OnDisable()
    {
        Transform temp = hpBar.transform.parent;
        while (temp.parent != null) { temp = temp.parent; }
        temp.gameObject.SetActive(false);
    }

    public virtual void Damage(float damage, Vector2 knockBack = default)
    {
        if (isDead) return;
        float finalDamage = damage * Mathf.Max(0, (1f - (defense / 100f)));

        hp = Mathf.Clamp(hp - finalDamage, 0, HP);
        HpBar = hp / HP;
        OnHitPolished();
        if (knockBack != Vector2.zero)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            rb.linearVelocity = knockBack;
        }

        if (hp == 0) { Death(); }
        if (Camera.main != null && !isShake)
        {
            isShake = true;
            Camera.main.transform.DOComplete();
            float shakeDuration = Mathf.Min(0.08f + (damage * 0.01f), 0.3f);
            float shakeStrength = Mathf.Min(0.1f + (damage * 0.01f), 0.5f);

            int shakeVibrato = Mathf.Clamp(3 + Mathf.RoundToInt(damage * 0.5f), 3, 12);
            Camera.main.transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, 90, false, true).OnComplete(() => isShake = false);
        }
    }

    public void OnHitPolished()
    {
        transform.DOKill();
        spriteRenderer.DOKill();
        spriteRenderer.color = Color.white;
        transform.localScale = Vector3.one;

        Sequence hitSeq = DOTween.Sequence();
        hitSeq.Join(spriteRenderer.DOColor(Color.red, 0.03f));
        hitSeq.Join(transform.DOScale(1.2f, 0.03f));
        hitSeq.Append(spriteRenderer.DOColor(Color.white, 0.1f));
        hitSeq.Append(transform.DOScale(1.0f, 0.1f));
    }
}