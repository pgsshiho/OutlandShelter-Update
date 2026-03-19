using UnityEngine;

public interface IEnemyDamage
{
    public void Damage(float damage, Vector2 knockBack = new Vector2());
}