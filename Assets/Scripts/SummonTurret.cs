using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AngleToImage
{
    public Sprite sprite;
    public Sprite launchSprite;
}

public class SummonTurret : ResourceReturn, ITurret, IEnemyAttackable
{
    public AngleToImage[] angleToImages;

    private Transform baseTransform;
    [SerializeField] private float range;

    private Vector2 direction;

    protected LayerMask enemy;

    protected Transform pivot;

    [SerializeField] protected float coolTime;

    protected bool canAttack = true;

    protected ObjectPoolManager poolManager;

    [SerializeField] protected Kind kind;

    public float damage = 30;

    public Range myPosition;

    protected void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemy = LayerMask.GetMask("Enemy");
        wall = LayerMask.GetMask("Wall");
        pivot = transform.Find("AttackPivot");
        baseTransform = FindAnyObjectByType<Resource>().transform;
        poolManager = ObjectPoolManager.instance[kind];
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Transform temp = hpSlider.transform.parent;
        while (temp.parent != null)
        {
            temp = temp.parent;
        }

        temp.gameObject.SetActive(true);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range * TechTreeUnlock.turretRange, enemy);

        if (hits.Length > 0)
        {
            int index = 0;

            for (int i = 1; i < hits.Length; i++)
            {
                if (Vector2.Distance(baseTransform.position, hits[i].transform.position) <= Vector2.Distance(baseTransform.position, hits[index].transform.position))
                {
                    index = i;
                }
            }

            Vector2 distance = (Vector2)hits[index].transform.position - (Vector2)transform.position;
            direction = distance.normalized;

            if (Physics2D.Raycast(transform.position, direction, distance.magnitude, wall).collider != null) return;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

            angle = (angle + 360f + 15f) % 360f;

            int imageNumber = Mathf.FloorToInt(angle / 30f);

            if (int.Parse(spriteRenderer.sprite.name[^1].ToString()) % 2 == 0) spriteRenderer.sprite = angleToImages[imageNumber].sprite;

            if (canAttack)
            {
                canAttack = false;

                GameObject temp = poolManager.Pool.Get();

                temp.transform.position = hits[index].transform.position;

                if (temp.TryGetComponent(out SummonTurretBullet bullet))
                {
                    bullet.pool = poolManager.Pool;
                }

                spriteRenderer.sprite = angleToImages[imageNumber].launchSprite;

                if (hits[index].TryGetComponent(out IEnemyDamage damage))
                {
                    damage.Damage(this.damage);
                }

                StartCoroutine(WaitAction.wait(0.1f, () =>
                {
                    spriteRenderer.sprite = angleToImages[imageNumber].sprite;
                }));

                StartCoroutine(WaitAction.wait(coolTime / TechTreeUnlock.turretAttackSpeed, () =>
                {
                    canAttack = true;
                }));
            }
        }
    }

    public override void TearDown()
    {
        ConstructionTurret.points.Add(myPosition);

        PlayerGuideUI.guides.Remove(this);

        poolManager.Pool.Release(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 center = transform.position;

        Vector3 prevPoint = center + range * TechTreeUnlock.turretRange * new Vector3(Mathf.Cos(0f), Mathf.Sin(0f), 0f);

        for (int i = 1; i <= 360; i++)
        {
            float angle = i * Mathf.Deg2Rad;
            Vector3 nextPoint = center + range * TechTreeUnlock.turretRange * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }

    private void OnDisable()
    {
        Transform temp = hpSlider.transform.parent;
        while (temp.parent != null)
        {
            temp = temp.parent;
        }

        temp.gameObject.SetActive(false);
    }
}
