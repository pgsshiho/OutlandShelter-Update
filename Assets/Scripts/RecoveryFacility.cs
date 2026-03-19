using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryFacility : ResourceReturn
{
    Transform child;

    protected override void Awake()
    {
        base.Awake();

        child = transform.GetChild(0);
    }

    protected override void Update()
    {
        base.Update();

        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(5, 5) * TechTreeUnlock.healthFacilityRecoveryRange, 0);

        child.localScale = new Vector3(5, 5, 1) * TechTreeUnlock.healthFacilityRecoveryRange;

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable player))
            {
                player.Damage(-Time.deltaTime * TechTreeUnlock.healthFacilityRecoverySpeed);
            }
        }
    }
}
