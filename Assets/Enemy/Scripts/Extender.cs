using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extender : BasicZombie
{
    [SerializeField] private GameObject arm;
    [HideInInspector] public MonoBehaviour[] playerComponents;
    [HideInInspector] public List<MonoBehaviour> disableComponents = new();
    [HideInInspector] public bool attackTiming = false;
    [HideInInspector] public bool isFail = false;
    [HideInInspector] public PlayerMove playerMove;
    private bool attack = false;
    private IDamageable player;
    private bool isAttacking = false;

    protected override void Awake()
    {
        base.Awake();
        playerMove = FindAnyObjectByType<PlayerMove>();
        target = playerMove.transform;
        target.TryGetComponent(out player);
    }

    protected override void Update()
    {
        targetPos = target.position;

        if (!isAttacking)
        {
            Vector2 direction = (targetPos - Position).normalized;
            spriteRenderer.flipX = direction.x <= 0 && (direction.x < 0 || spriteRenderer.flipX);
            rb.linearVelocity = direction * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (Vector2.Distance(Position, targetPos) <= range && canAttack)
        {
            canAttack = false;
            anim.SetTrigger("Attack");
            isAttacking = true;

            StartCoroutine(WaitAction.wait(() => spriteRenderer.sprite.name[^1] == '7', () =>
            {
                Attack(target);
            }));

            StartCoroutine(WaitAction.wait(attackCool, () =>
            {
                canAttack = true;
            }));
        }

        if (attack)
        {
            player.Damage(damage * Time.deltaTime);
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 1000f);

        if (hpBar.transform.parent.gameObject.activeSelf) hpBar.transform.parent.position = Camera.main.WorldToScreenPoint(transform.position + (Vector3)offset);
    }

    protected override void Attack(Transform target)
    {
        GameObject temp = Instantiate(arm, targetPos, Quaternion.identity);

        temp.TryGetComponent(out ExtenderArm exArm);

        exArm.owner = this;

        StartCoroutine(WaitAction.wait(() => attackTiming, () =>
        {
            attack = true;
            anim.SetTrigger("Catch" + (playerMove.gender == PlayerMove.Gender.Man ? "M" : "W"));
            target.position = transform.position;

            StartCoroutine(WaitAction.wait(3f, () =>
            {
                foreach (MonoBehaviour temp in playerComponents)
                {
                    if (temp is not Personal_resource)
                    {
                        if (!disableComponents.Contains(temp)) temp.enabled = true;
                    }
                }
                target.GetComponent<SpriteRenderer>().enabled = true;
                target.GetComponent<Collider2D>().enabled = true;
                attack = false;
                isAttacking = false;
                attackTiming = false;
                anim.SetTrigger("Fail");

                StartCoroutine(WaitAction.waitOneFrame(() =>
                {
                    anim.ResetTrigger("Fail");
                }));
            }));
        }));

        StartCoroutine(WaitAction.wait(() => isFail, () =>
        {
            isAttacking = false;
            isFail = false;
            anim.SetTrigger("Fail");
        }));
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        
    }

    public override void Damage(float damage, Vector2 knockBack = default)
    {
        if (!isAttacking)
        {
            base.Damage(damage, knockBack);
        }
    }
}
