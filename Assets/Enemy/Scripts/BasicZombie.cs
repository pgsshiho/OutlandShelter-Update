using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicZombie : MonoBehaviour, IEnemyDamage
{
    public float speed = 3.0f;
    public Transform target;
    public float HP = 10;
    protected float hp;
    public int damage = 3;
    public float attackCool = 0.7f;
    protected bool canAttack = true;
    protected Rigidbody2D rb;
    protected LayerMask wall;
    [SerializeField] protected Image hpBar;
    [SerializeField] protected float knockBackForce;
    public static int deathCount = 0;
    [SerializeField] protected int dropExp;
    [SerializeField] protected int dropMatarial;
    protected bool isDead = false;

    protected float HpBar
    {
        set
        {
            if (!hpBar.transform.parent.gameObject.activeSelf)
                StartCoroutine(WaitAction.wait(5f, () =>
                {
                    hpBar.transform.parent.gameObject.SetActive(false);
                }));

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

        Transform temp = hpBar.transform.parent;
        while (temp.parent != null)
        {
            temp = temp.parent;
        }

        canAttack = true;
        temp.gameObject.SetActive(true);
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
                rb.linearVelocity = direction * speed;
            }
        }
        else if (canAttack)
        {
            canAttack = false;
            anim.SetTrigger("Attack");

            bool temp = target.TryGetComponent<PlayerAvoidSkill>(out _);

            if (temp)
            {
                StartCoroutine(WaitAction.wait(() => { return TechTreeUnlock.additionalAvoidAbleTiming || spriteRenderer.sprite.name[^1] == '2'; }, () =>
                {
                    Vector2 direction = (targetPos - Position).normalized;
                    if (Random.Range(0f, 1f) < TechTreeUnlock.avoidProbability) PlayerAvoidSkill.SkillUse(direction, true);
                    PlayerAvoidSkill.useable = true;
                    PlayerAvoidSkill.targetPos = Position;
                }));
            }

            StartCoroutine(WaitAction.wait(() => { return spriteRenderer.sprite.name[^1] == '3'; }, () =>
            {
                if (temp) PlayerAvoidSkill.useable = false;
                Attack(target);
            }));
            StartCoroutine(WaitAction.wait(attackCool, () =>
            {
                canAttack = true;
            }));
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 1000f);

        if (hpBar.transform.parent.gameObject.activeSelf)
            hpBar.transform.parent.position = Camera.main.WorldToScreenPoint(transform.position + (Vector3)offset);
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
            }
        }
    }

    protected virtual Transform SelectTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(Position, 1000, ~wall);
        List<Collider2D> targets = new();

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<IEnemyAttackable>(out _))
            {
                targets.Add(hit);
            }
        }

        if (targets.Count != 0)
        {
            targets.Sort((c1, c2) => Vector2.Distance(Position, c1.transform.position).CompareTo(Vector2.Distance(Position, c2.transform.position)));
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
        if (isDead) return;
        isDead = true;
        stack++;
        increaseSpeed = 1 + TechTreeUnlock.continuousIncreaseMoveSpeed * Mathf.Clamp(stack, 0, TechTreeUnlock.S22MAXOVERWRAP);
        StartCoroutine(WaitAction.wait(3f, () =>
        {
            stack--;
        }));
        col.enabled = false;
        deathCount++;

        MapManager.currentZombieCount--;
        int finalExp = MainmenuManager.isLong ? Mathf.Max(1, Mathf.RoundToInt(dropExp * 0.6f)) : dropExp;
        int finalMat = MainmenuManager.isLong ? Mathf.Max(1, Mathf.RoundToInt(dropMatarial * 0.6f)) : dropMatarial;
       
            Personal_resource.CurExp += finalExp;
        Personal_resource.instance.Metal += finalMat;

        Notion.Log($"+{finalExp}EXP, +{finalMat}Metal");
        if (SFXReference.Instance.zombieDie != null)
        {
            SoundManager.SFX.PlayOneShot(SFXReference.Instance.zombieDie, 0.5f);
        }
        GameObject temp = ObjectPoolManager.instance[Kind.ZombieDeathEffect].Pool.Get();
        temp.transform.position = transform.position;

        ObjectPoolManager.instance[Kind.ZombieDeathEffect].StartCoroutine(WaitAction.wait(0.4f, () =>
        {
            ObjectPoolManager.instance[Kind.ZombieDeathEffect].Pool.Release(temp);
        }));

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
        if (isDead) return;
        hp = Mathf.Clamp(hp - damage, 0, hp);
        HpBar = hp / HP;

        if (hp == 0) { Death(); }
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