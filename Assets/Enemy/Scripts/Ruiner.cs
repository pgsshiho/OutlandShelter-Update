using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruiner : BasicZombie
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override Transform SelectTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(Position, 1000, ~wall);
        List<Collider2D> potentialTargets = new();

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.layer == gameObject.layer) continue;
            if (hit.CompareTag("Player")) continue;

            if (hit.TryGetComponent<IEnemyAttackable>(out _))
            {
                potentialTargets.Add(hit);
            }
        }

        if (potentialTargets.Count > 0)
        {
            potentialTargets.Sort((c1, c2) =>
            {
                bool isBase1 = c1.CompareTag("Base");
                bool isBase2 = c2.CompareTag("Base");

                if (isBase1 && !isBase2) return -1;
                if (!isBase1 && isBase2) return 1;

                return Vector2.Distance(Position, c1.transform.position)
                    .CompareTo(Vector2.Distance(Position, c2.transform.position));
            });

            targetPos = potentialTargets[0].transform.position + (Vector3)potentialTargets[0].offset;
            return potentialTargets[0].transform;
        }

        targetPos = Position;
        return null;
    }

    protected override void Attack(Transform target)
    {
        if (target == null) return;
        if (target.CompareTag("Player")) return;

        if (target.TryGetComponent(out IFacility facility)) facility.Damage(damage * 2);
        else if (target.TryGetComponent(out ITurret turret)) turret.Damage(damage * 2);
        else if (target.TryGetComponent(out ICenter center)) center.Damage(damage * 2);
    }

    protected override void OnCollisionStay2D(Collision2D collision) { }
}