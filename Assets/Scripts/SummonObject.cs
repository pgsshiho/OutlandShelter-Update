using UnityEngine;

public enum Kind
{
    Hammer = 0,
    Melee = 1,
    Gun = 2,
    Mine = 3,
    Throw = 4,
    Turret = 5,
    Armor = 6,
    Wooden,
    Steel,
    TurretBullet,
    Zombie,
    ZombieDeathEffect
}

public abstract class SummonObject : MonoBehaviour
{
    public float damage;
    [SerializeField] protected float knockBackForce = 0;
    public static int overWrap = 0;

    protected abstract void Awake();

    protected abstract void OnEnable();

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IEnemyDamage enemy))
        {
            Vector2 direction = (other.transform.position - transform.position).normalized;
            Attack(enemy, direction);
        }
    }

    protected virtual void Attack(IEnemyDamage enemy, Vector2 direction)
    {
        enemy.Damage(damage * TechTreeUnlock.weaponDamage, knockBackForce * direction);
    }
}
