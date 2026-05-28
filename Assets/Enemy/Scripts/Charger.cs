using System.Collections;
using UnityEngine;

public class Charger : BasicZombie
{
    private bool isShooting = false; // 공격 전용 액션 딜레이 플래그

    [Header("Heal Area Spawn Settings")]
    [SerializeField]
    private GameObject healAreaPrefab;

    [SerializeField]
    private float healAmountValue = 10f;

    [SerializeField]
    private float healCooldown = 30f;

    private bool canHeal = true;

    protected override void OnEnable()
    {
        base.OnEnable();
        isShooting = false;
        canHeal = true;
    }

    protected override void Update()
    {
        if (isShooting)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        if (canHeal)
        {
            StartCoroutine(SpawnHealAreaRoutine());
        }

        base.Update();
    }

    private IEnumerator SpawnHealAreaRoutine()
    {
        canHeal = false;

        if (healAreaPrefab != null)
        {
            GameObject temp = Instantiate(healAreaPrefab, transform.position, Quaternion.identity);

            if (temp.TryGetComponent(out ChargerHealarea healArea))
            {
                healArea.Healamount = healAmountValue;
            }
        }

        yield return new WaitForSeconds(healCooldown);
        canHeal = true;
    }

    protected override void Attack(Transform target)
    {
        if (target == null || isShooting)
            return;

        if (target.TryGetComponent(out SummonTurret turret))
        {
            StartCoroutine(StunRoutine(turret));
            turret.Damage(damage);
            StartCoroutine(ActionDelay());
        }
        else if (target.TryGetComponent(out IFacility facility))
        {
            facility.Damage(damage);
            StartCoroutine(ActionDelay());
        }
        else if (target.TryGetComponent(out ICenter center))
        {
            center.Damage(damage);
            StartCoroutine(ActionDelay());
        }
        else
        {
            base.Attack(target);
        }
    }

    private IEnumerator StunRoutine(SummonTurret target)
    {
        target.enabled = false;
        yield return new WaitForSeconds(3f);
        if (target != null)
            target.enabled = true;
    }

    private IEnumerator ActionDelay()
    {
        isShooting = true;
        yield return new WaitForSeconds(3f);
        isShooting = false;
    }
}
