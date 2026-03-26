using System.Collections;
using UnityEngine;

public class Charger : BasicZombie
{
    private bool isShooting = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        isShooting = false;
    }

    protected override void Update()
    {
        if (isShooting)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        base.Update();
    }

    protected override void Attack(Transform target)
    {
        if (target == null || isShooting) return;

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
        if (target != null) target.enabled = true;
    }

    private IEnumerator ActionDelay()
    {
        isShooting = true;
        yield return new WaitForSeconds(3f);
        isShooting = false;
    }
}