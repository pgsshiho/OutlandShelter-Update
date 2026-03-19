using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saliva : MonoBehaviour
{
    [HideInInspector] public float damage;
    Rigidbody2D rb;

    [SerializeField] private float duration = 0.8f;

    [SerializeField] private GameObject linoleum;

    private Transform target;

    private void Awake()
    {
        target = FindAnyObjectByType<PlayerMove>().transform;
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, duration);

        StartCoroutine(WaitAction.wait(() => Vector2.Distance(transform.position, target.position) < (TechTreeUnlock.additionalAvoidAbleTiming ? 2 : 1), () =>
        {
            Vector2 direction = (target.position - transform.position).normalized;
            if (Random.Range(0f, 1f) < TechTreeUnlock.avoidProbability) PlayerAvoidSkill.SkillUse(direction, true);
            PlayerAvoidSkill.useable = true;
            PlayerAvoidSkill.targetPos = transform.position;

            StartCoroutine(WaitAction.wait(() => Vector2.Distance(transform.position, target.position) >= 2, () =>
            {
                PlayerAvoidSkill.useable = false;
            }));
        }));
    }

    public void ThrowTo(Vector2 direction, float force)
    {
        rb.linearVelocity = direction * force;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IFacility facility))
        {
            facility.Damage(damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.TryGetComponent(out ITurret turret))
        {
            turret.Damage(damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.TryGetComponent(out ICenter center))
        {
            center.Damage(damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.TryGetComponent(out IDamageable player))
        {
            player.Damage(damage, type: AttackType.Explosion);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        GameObject temp = Instantiate(linoleum, transform.position, Quaternion.identity);

        if (temp.TryGetComponent(out SalivaLinoleum lino))
        {
            lino.damage = damage * Time.fixedDeltaTime;
        }
    }
}
