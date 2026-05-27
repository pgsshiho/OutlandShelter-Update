using UnityEngine;

public class ChargerHealarea : MonoBehaviour
{
    [HideInInspector]
    public float Healamount;

    [SerializeField]
    private float duration = 3f;

    private void Awake()
    {
        Destroy(gameObject, duration);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IEnemyDamage enemy))
        {
            enemy.Damage(-Healamount * Time.deltaTime);
        }
    }
}
