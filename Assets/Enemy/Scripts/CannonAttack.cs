using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonAttack : MonoBehaviour
{
    public float damage;
    private Transform target;
    public GameObject rangeExpect;
    public GameObject effect;

    private void Awake()
    {
        target = FindAnyObjectByType<PlayerMove>().transform;
        transform.DOMoveY(target.position.y + 50, 0.5f).OnComplete(() =>
        {
            Vector2 targetPos = target.position;
            GameObject temp = Instantiate(rangeExpect, targetPos, Quaternion.identity);
            StartCoroutine(WaitAction.wait(1.5f, () =>
            {
                transform.position = new Vector3(targetPos.x, targetPos.y + 50);

                if (TechTreeUnlock.additionalAvoidAbleTiming) PlayerAvoidSkill.useable = true;
                else    StartCoroutine(WaitAction.wait(() => Vector2.Distance(transform.position, targetPos) < 10, () =>
                        {
                            Vector2 direction = (target.position - transform.position).normalized;
                            if (Random.Range(0f, 1f) < TechTreeUnlock.avoidProbability) PlayerAvoidSkill.SkillUse(direction, true);
                            PlayerAvoidSkill.useable = true;
                            PlayerAvoidSkill.targetPos = transform.position;
                        }));

                transform.DOMoveY(targetPos.y, 0.5f).OnComplete(() =>
                {
                    GetComponent<Collider2D>().enabled = true;
                    PlayerAvoidSkill.useable = false;
                    Destroy(temp);
                    Destroy(gameObject, Time.fixedDeltaTime);
                });
            }));
        });
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

    private void OnCollisionEnter2D(Collision2D collision)
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
        Instantiate(effect, transform.position, Quaternion.identity);
    }
}
