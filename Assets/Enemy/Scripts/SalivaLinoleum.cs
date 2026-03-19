using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalivaLinoleum : MonoBehaviour
{
    [HideInInspector] public float damage;

    [SerializeField] private float duration;

    private void Awake()
    {
        Destroy(gameObject, duration);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IFacility facility))
        {
            facility.Damage(damage);
        }
        else if (collision.TryGetComponent(out ITurret turret))
        {
            turret.Damage(damage);
        }
        else if (collision.TryGetComponent(out ICenter center))
        {
            center.Damage(damage);
        }
        else if (collision.TryGetComponent(out IDamageable player))
        {
            player.Damage(damage, type: AttackType.Explosion);
        }
    }
}
