using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBullet : MonoBehaviour
{
    public float damage;
    private TrailRenderer trailRenderer;

    private Transform target;

    void Awake()
    {
        target = FindAnyObjectByType<PlayerMove>().transform;
        trailRenderer = GetComponent<TrailRenderer>();
        Destroy(gameObject, 5f);

        StartCoroutine(WaitAction.wait(() => Vector2.Distance(transform.position, target.position) < (TechTreeUnlock.additionalAvoidAbleTiming ? 5 : 2.5f), () =>
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable player))
        {
            player.Damage(damage);
        }
        else if (collision.gameObject.TryGetComponent(out ICenter basecamp))
        {
            basecamp.Damage(damage);
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

    private void OnDestroy()
    {
        trailRenderer.Clear();
    }
}
