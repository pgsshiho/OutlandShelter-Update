using UnityEngine;

public class GasCloud : MonoBehaviour
{
    public float lifetime = 5f;
    public float damagePerTick = 1f;
    public float tickInterval = 0.3f;
    private float timer = 0f;

    void Start() => Destroy(gameObject, lifetime);

    void OnTriggerStay2D(Collider2D other)
    {
        timer += Time.deltaTime;
        if (timer >= tickInterval)
        {
            if (other.TryGetComponent(out IDamageable target))
            {
                target.Damage(damagePerTick);
                // 가스에 닿아도 중독 적용
                if (other.TryGetComponent(out PoisonStatus ps)) ps.ApplyPoison(false);
            }
            timer = 0f;
        }
    }
}