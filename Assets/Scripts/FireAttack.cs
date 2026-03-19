using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttack : MonoBehaviour
{
    [SerializeField] private float damage = 20;
    [SerializeField] private float duration = 10;

    private void Awake()
    {
        StartCoroutine(WaitAction.wait(duration, () =>
        {
            
            Destroy(transform.parent.gameObject);
        }));
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IEnemyDamage enemy))
        {
            enemy.Damage(damage * Time.fixedDeltaTime * TechTreeUnlock.weaponDamage);
        }
        else if (collision.TryGetComponent(out IDamageable player))
        {
            player.Damage(damage * Time.fixedDeltaTime, type: AttackType.Explosion);
        }
    }
}
