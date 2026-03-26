using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melten : BasicZombie
{
    [SerializeField] private float damageInterval = 0.5f;
    private float damageTimer;

    protected override void Update()
    {
        base.Update();
        damageTimer += Time.deltaTime;
    }

    protected override void Attack(Transform target) { }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        base.OnCollisionStay2D(collision);

        if (damageTimer >= damageInterval)
        {
            damageTimer = 0;
            Transform t = collision.transform;

            if (t.TryGetComponent(out IDamageable player))
            {
                player.Damage(damage, Vector2.zero, AttackType.Close, 0.1f);
            }
            else if (t.TryGetComponent(out IFacility facility))
            {
                facility.Damage(damage);
            }
            else if (t.TryGetComponent(out ITurret turret))
            {
                turret.Damage(damage);
            }
            else if (t.TryGetComponent(out ICenter center))
            {
                center.Damage(damage);
            }
        }
    }
}