using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExtenderArm : MonoBehaviour
{
    [HideInInspector] public Extender owner;
    [SerializeField] private float range;
    private LayerMask player;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        player = LayerMask.GetMask("Player");
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(WaitAction.waitOneFrame(() =>
        {
            Vector2 direction = (owner.target.position - transform.position).normalized;
            if (Random.Range(0f, 1f) < TechTreeUnlock.avoidProbability) PlayerAvoidSkill.SkillUse(direction, true);
            PlayerAvoidSkill.useable = true;
            PlayerAvoidSkill.targetPos = transform.position;

            StartCoroutine(WaitAction.wait(() => spriteRenderer.sprite.name[spriteRenderer.sprite.name.Length - 1] == '2'
            && Physics2D.OverlapCircle(transform.position, range, player) != null, () =>
            {
                StartCoroutine(WaitAction.wait(0.08f, () =>
                {
                    PlayerAvoidSkill.useable = false;
                    anim.SetTrigger("Catch" + (owner.playerMove.gender == PlayerMove.Gender.Man ? "M" : "W"));
                    owner.target.position = transform.position;

                    owner.playerComponents = owner.target.GetComponents<MonoBehaviour>();

                    owner.target.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

                    foreach (MonoBehaviour temp in owner.playerComponents)
                    {
                        if (temp is not Personal_resource)
                        {
                            if (temp.enabled != false) temp.enabled = false;
                            else if (!owner.disableComponents.Contains(temp)) owner.disableComponents.Add(temp);
                        }
                    }
                    owner.target.GetComponent<SpriteRenderer>().enabled = false;
                    owner.target.GetComponent<Collider2D>().enabled = false;

                    StartCoroutine(WaitAction.waitOneFrame(() =>
                    {
                        StartCoroutine(WaitAction.wait(() => !anim.GetCurrentAnimatorStateInfo(0).IsTag("Catch"), () =>
                        {
                            owner.attackTiming = true;
                            Destroy(gameObject);
                        }));
                    }));
                }));
            }));

            StartCoroutine(WaitAction.waitOneFrame(() =>
            {
                StartCoroutine(WaitAction.wait(() => anim.GetCurrentAnimatorStateInfo(0).IsTag("Try")
                && spriteRenderer.sprite.name[spriteRenderer.sprite.name.Length - 1] == '6', () =>
                {
                    owner.isFail = true;
                    Destroy(gameObject, 0.08f);
                }));
            }));
        }));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 center = transform.position;

        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0f), Mathf.Sin(0f), 0f) * range * TechTreeUnlock.turretRange;

        for (int i = 1; i <= 360; i++)
        {
            float angle = i * Mathf.Deg2Rad;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * range * TechTreeUnlock.turretRange;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}
