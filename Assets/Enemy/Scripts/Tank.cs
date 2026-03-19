using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Tank : BasicZombie
{
    public enum Phase
    {
        One,
        Two
    }

    public Phase phase;
    private bool isUsingSkill = false;
    private bool isSkillUsable = true;
    private bool isKamekaze = false;

    private Transform muzzle;

    protected override void Awake()
    {
        base.Awake();
        target = FindAnyObjectByType<PlayerMove>().transform;
        muzzle = transform.GetChild(0);

        StartCoroutine(WaitAction.wait(() => phase == Phase.Two, () =>
        {
            GetComponent<SpriteRenderer>().DOColor(new(1f, 0.5f, 0.5f), 0.5f);
        }));
    }

    protected override void Update()
    {
        Vector2 direction = (targetPos - Position).normalized;
        
        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            direction.x = Mathf.Sign(direction.x);
            direction.y = 0;
        }
        else
        {
            direction.x = 0;
            direction.y = Mathf.Sign(direction.y);
        }

        anim.SetFloat("DirX", direction.x);
        anim.SetFloat("DirY", direction.y);

        if (hp / HP <= 0.05f && !isKamekaze)
        {
            isKamekaze = true;
            direction = (targetPos - Position).normalized;
            KameKaze(direction, speed * 3f);
        }

        if (isSkillUsable && !isUsingSkill && !isKamekaze)
        {
            isSkillUsable = false;

            if (phase == Phase.One)
            {
                int pattern = Random.Range(0, 2);

                if (Mathf.Abs(targetPos.x - Position.x) < 1 || Mathf.Abs(targetPos.y - Position.y) < 1)
                {
                    Dash(direction, speed * 2f);
                }
                else
                {
                    switch (pattern)
                    {
                        case 0:
                            Cannon(3);
                            break;
                        case 1:
                            Rifle(5);
                            break;
                    }
                }
            }
            else
            {
                int pattern = Random.Range(0, 2);

                if (Mathf.Abs(targetPos.x - Position.x) < 1 || Mathf.Abs(targetPos.y - Position.y) < 1)
                {
                    Dash(direction, speed * 3f);
                    StartCoroutine(WaitAction.wait(() => !isDashing, () =>
                    {
                        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
                        {
                            direction.x = Mathf.Sign(direction.x);
                            direction.y = 0;
                            spriteRenderer.flipX = direction.x == 1;
                        }
                        else
                        {
                            direction.x = 0;
                            direction.y = Mathf.Sign(direction.y);
                        }

                        Dash(direction, speed * 3f);
                    }));
                }
                else
                {
                    switch (pattern)
                    {
                        case 0:
                            Cannon(5);
                            break;
                        case 1:
                            Rifle(5);
                            break;
                    }
                }
            }

            float coolTime = Random.Range(3f, 8f);
            StartCoroutine(WaitAction.wait(coolTime, () =>
            {
                isSkillUsable = true;
            }));
        }

        if (!isUsingSkill && !isKamekaze) rb.linearVelocity = direction * speed;

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 1000f);

        if (hpBar.transform.parent.gameObject.activeSelf) hpBar.transform.parent.position = Camera.main.WorldToScreenPoint(transform.position + (Vector3)offset);
    }

    public override void Damage(float damage, Vector2 knockBack = default)
    {
        base.Damage(damage, knockBack);

        if (hp <= HP / 2f) phase = Phase.Two;
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable player))
        {
            player.Damage(100 * Time.fixedDeltaTime);
        }
        else if (collision.gameObject.TryGetComponent(out ICenter basecamp))
        {
            basecamp.Damage(100 * Time.fixedDeltaTime);
        }
        else if (collision.gameObject.TryGetComponent(out IFacility facility))
        {
            facility.Damage(100 * Time.fixedDeltaTime);
        }
        else if (collision.gameObject.TryGetComponent(out ITurret turret))
        {
            turret.Damage(100 * Time.fixedDeltaTime);
        }
        else if (collision.gameObject.TryGetComponent(out ResourceObject resource))
        {
            resource.Break(100 * Time.fixedDeltaTime);
        }
    }

    [SerializeField] private GameObject bomb;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isKamekaze)
        {
            Instantiate(bomb, transform.position, Quaternion.identity);

            Collider2D[] hits = Physics2D.OverlapCircleAll(Position, 3);

            foreach (Collider2D hit in hits)
            {
                if (hit.TryGetComponent(out IDamageable player))
                {
                    player.Damage(damage);
                }
                else if (hit.TryGetComponent(out ICenter basecamp))
                {
                    basecamp.Damage(500);
                }
                else if (hit.TryGetComponent(out IFacility facility))
                {
                    facility.Damage(damage);
                }
                else if (hit.TryGetComponent(out ITurret turret))
                {
                    turret.Damage(damage);
                }
            }

            Death();

            Camera.main.DOShakePosition(1.2f);
        }
        else if (isDashing)
        {
            if (collision.gameObject.TryGetComponent(out IDamageable player))
            {
                player.Damage(damage);
            }
            else if (collision.gameObject.TryGetComponent(out ICenter basecamp))
            {
                basecamp.Damage(500);
            }
            else if (collision.gameObject.TryGetComponent(out IFacility facility))
            {
                facility.Damage(damage);
            }
            else if (collision.gameObject.TryGetComponent(out ITurret turret))
            {
                turret.Damage(damage);
            }
        }
    }

    bool isDashing = false;

    private void Dash(Vector2 direction, float speed)
    {
        isUsingSkill = true;
        isDashing = true;

        Vector2 initPos = transform.position;

        rb.linearVelocity = direction * speed;

        StartCoroutine(WaitAction.wait(() => Vector2.Distance(initPos, transform.position) >= 15, () =>
        {
            isDashing = false;

            StartCoroutine(WaitAction.waitOneFrame(() => isUsingSkill = false));
        }));
    }

    [SerializeField] private GameObject cannon;

    private void Cannon(int count)
    {
        isUsingSkill = true;
        StartCoroutine(CannonShot(count));
    }

    private IEnumerator CannonShot(int count)
    {
        for (int i = 0; i < count; i++)
        {
            anim.SetTrigger("Attack");

            yield return new WaitForSeconds(2f / count);

            GameObject temp = Instantiate(cannon, muzzle.position, Quaternion.identity);

            if (temp.TryGetComponent(out CannonAttack attack))
            {
                attack.damage = damage;
            }

            yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"));
        }
        isUsingSkill = false;
    }

    [SerializeField] private GameObject bullet;

    private void Rifle(int count)
    {
        isUsingSkill = true;
        Vector2 direction = (target.position - muzzle.position).normalized;

        StartCoroutine(RifleShot(count, direction));
    }

    private IEnumerator RifleShot(int count, Vector2 direction)
    {
        yield return null;

        for (int i = 0; i < count; i++)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

            GameObject temp = Instantiate(bullet, muzzle.position, Quaternion.Euler(0, 0, angle));

            if (temp.TryGetComponent(out Rigidbody2D rigid))
            {
                rigid.linearVelocity = rigid.transform.up * 20;
            }

            yield return new WaitForSeconds(1f / count);
        }
        isUsingSkill = false;
    }

    private void KameKaze(Vector2 direction, float speed)
    {
        rb.linearVelocity = direction * speed;
    }
}
