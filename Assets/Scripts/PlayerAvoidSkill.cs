using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAvoidSkill : MonoBehaviour
{
    public static bool isUsing = false;
    private bool isReady = true;
    public static bool useable = false;
    public static Vector2 targetPos = Vector2.zero;
    private static Rigidbody2D rb;
    private static MonoBehaviour instance;

    public static bool damageUp = false;

    private Transform attackPivot;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        attackPivot = transform.Find("AttackPivot");
        isUsing = false;
        useable = false;
        targetPos = Vector2.zero;
        damageUp = false;
        overWrap = 0;
        isDash = false;
        instance = this;
    }

    void Update()
    {
        if (TechTreeUnlock.haveAvoidSkill)
        {
            if (useable)
            {
                if (isReady)
                {
                    if (!isUsing && Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        isReady = false;
                        StartCoroutine(WaitAction.wait(TechTreeUnlock.avoidSkillCoolTime, () =>
                        {
                            isReady = true;
                        }));

                        Vector2 direction = -(targetPos - (Vector2)transform.position).normalized;

                        SkillUse(direction, true);
                    }
                }
            }
            else
            {
                if (!isUsing && isReady && Input.GetKeyDown(KeyCode.LeftShift))
                {
                    isReady = false;
                    StartCoroutine(WaitAction.wait(TechTreeUnlock.avoidSkillCoolTime, () =>
                    {
                        isReady = true;
                    }));

                    SkillUse(new Vector2(Mathf.Cos((attackPivot.eulerAngles.z + 90) * Mathf.Deg2Rad), Mathf.Sin((attackPivot.eulerAngles.z + 90) * Mathf.Deg2Rad)), false);
                }
            }
        }
    }

    static int overWrap = 0;
    public static bool isDash = false;

    public static void SkillUse(Vector2 direction, bool isInvincibility)
    {
        isUsing = true;
        overWrap++;
        damageUp = true;
        isDash = true;

        rb.AddForce(direction * 10f * TechTreeUnlock.avoidSpeed, ForceMode2D.Impulse);

        if (isInvincibility)
        {
            instance.StartCoroutine(WaitAction.wait(0.1f + TechTreeUnlock.additionalInvincibilityTime, () =>
            {
                isUsing = false;
            }));
        }
        else
        {
            isUsing = false;
        }

        instance.StartCoroutine(WaitAction.wait(0.2f, () =>
        {
            isDash = false;
        }));

        instance.StartCoroutine(WaitAction.wait(3f, () =>
        {
            overWrap--;
            if (overWrap == 0) damageUp = false;
        }));
    }
}
