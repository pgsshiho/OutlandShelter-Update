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
        ContactFilter2D filter = new()
        {
            layerMask = ~wall,
            useLayerMask = true,
            useTriggers = true,
        };
        Physics2D.OverlapCircle(Position, 1000, filter, hits);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.layer == gameObject.layer)
                continue;
            if (hit.CompareTag("Player"))
                continue;

            if (hit.TryGetComponent<IEnemyAttackable>(out _))
            {
                targets.Add(hit);
            }
        }

        if (targets.Count > 0)
        {
            targets.Sort(
                (c1, c2) =>
                {
                    if (c1 is IEnemyAttackable e1 && c2 is IEnemyAttackable e2)
                    {
                        int p1 = e1.GetPriority();
                        int p2 = e2.GetPriority();
                        if (p1 != p2)
                            return p1.CompareTo(p2);
                    }

                    bool isBase1 = c1.CompareTag("Base");
                    bool isBase2 = c2.CompareTag("Base");

                    if (isBase1 && !isBase2)
                        return -1;
                    if (!isBase1 && isBase2)
                        return 1;

                    return Vector2
                        .Distance(Position, c1.transform.position)
                        .CompareTo(Vector2.Distance(Position, c2.transform.position));
                }
            );

            targetPos = targets[0].transform.position + (Vector3)targets[0].offset;
            return targets[0].transform;
        }

        targetPos = Position;
        return null;
    }

    protected override void Attack(Transform target)
    {
        if (target == null)
            return;
        if (target.CompareTag("Player"))
            return;

        if (target.TryGetComponent(out IFacility facility))
            facility.Damage(damage * 2);
        else if (target.TryGetComponent(out ITurret turret))
            turret.Damage(damage * 2);
        else if (target.TryGetComponent(out ICenter center))
            center.Damage(damage * 2);
    }

    protected override void OnCollisionStay2D(Collision2D collision) { }
}
