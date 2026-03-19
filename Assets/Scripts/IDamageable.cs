using UnityEngine;

public enum AttackType
{
    None,
    Close,
    Explosion
}

public interface IDamageable
{
    public void Damage(float damage, Vector2 knockBackDirection = new Vector2(), AttackType type = AttackType.None, float stiffenTime = 0.2f);
}
