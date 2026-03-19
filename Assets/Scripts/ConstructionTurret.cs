using System.Collections.Generic;
using UnityEngine;
using Weapons;

public class ConstructionTurret : MonoBehaviour
{
    private ResourceSpawner area;

    private Turret turret;

    public static List<Range> points;

    private Vector3 center;
    private Vector3 size;

    private GameObject notConstructableArea;
    private readonly List<GameObject> constructableArea = new List<GameObject>();
    [SerializeField] private GameObject rectangle;
    [SerializeField] private GameObject turretPointPrefab;
    private GameObject turretPoint;

    public int turretIndex = 0;

    private void Awake()
    {
        area = FindAnyObjectByType<ResourceSpawner>();

        if (points == null) points = area.constraints.GetRange(2, 4);

        turret = FindAnyObjectByType<Turret>();

        Range range;

        if (!TechTreeUnlock.isPortableTurret)
        {
            range = area.range;
            center = (range.leftBottom + range.rightTop) / 2f;
            size = range.rightTop - range.leftBottom;
            notConstructableArea = Instantiate(rectangle, center, Quaternion.identity);
            notConstructableArea.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.2f);
            notConstructableArea.transform.localScale = size;
        }

        for (int i = 0; i < points.Count; i++)
        {
            range = points[i];
            center = (range.leftBottom + range.rightTop) / 2f;
            size = range.rightTop - range.leftBottom;
            constructableArea.Add(Instantiate(rectangle, center, Quaternion.identity));
            constructableArea[i].GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.5f);
            constructableArea[i].transform.localScale = size;
        }

        turretPoint = Instantiate(turretPointPrefab);
    }

    bool isFixed = false;

    private void Update()
    {
        Vector2 mousePosition = FindPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition), out Range needDelete);

        transform.position = mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            if (isFixed)
            {
                ObjectPoolManager.instance[turret.kind].weaponIndex = turretIndex;
                GameObject temp = turret.Build(transform.position);
            
                if (temp.TryGetComponent(out SummonTurret summonTurret))
                {
                    summonTurret.myPosition = needDelete;
                }

                if (points.Contains(needDelete)) points.Remove(needDelete);
                
                Destroy(turretPoint);
                Destroy(notConstructableArea);
                for (int i = 0; i < constructableArea.Count; i++)
                {
                    Destroy(constructableArea[i]);
                }
                Destroy(gameObject);
            }
            else if (TechTreeUnlock.isPortableTurret)
            {
                ObjectPoolManager temp = ObjectPoolManager.instance[turret.kind];

                if (temp.summonPrefab[turretIndex].TryGetComponent(out BoxCollider2D col)
                    && !IsOverlap(mousePosition, temp.summonPrefab[turretIndex].transform.localScale * col.size, area.constraints.GetRange(1, area.constraints.Count - 1)))
                {
                    Vector2 size = temp.summonPrefab[turretIndex].transform.localScale * col.size;

                    needDelete = new Range(mousePosition + col.offset - size / 2f, mousePosition + col.offset + size / 2f);

                    temp.weaponIndex = turretIndex;
                    GameObject temp2 = turret.Build(transform.position);

                    if (temp2.TryGetComponent(out SummonTurret summonTurret))
                    {
                        summonTurret.myPosition = needDelete;
                    }

                    area.constraints.Add(needDelete);

                    Destroy(gameObject);
                }
            }
        }
    }

    private bool IsOverlap(Vector3 point, Vector3 size, List<Range> constraints)
    {
        foreach (Range range in constraints)
        {
            if (point.x >= range.leftBottom.x - size.x / 2f && point.x <= range.rightTop.x + size.x / 2f && point.y >= range.leftBottom.y - size.y / 2f && point.y <= range.rightTop.y + size.y / 2f)
            {
                return true;
            }
        }
        return false;
    }

    private Vector3 FindPosition(Vector3 position, out Range needDelete)
    {
        foreach (Range range in points)
        {
            if (range.leftBottom.x < position.x && range.rightTop.x > position.x && range.leftBottom.y < position.y && range.rightTop.y > position.y)
            {
                isFixed = true;
                needDelete = range;
                return (range.leftBottom + range.rightTop) / 2f;
            }
        }

        isFixed = false;

        needDelete = new Range();
        return position;
    }

    private void OnDestroy()
    {
        Destroy(turretPoint);
        if (notConstructableArea != null) Destroy(notConstructableArea);
        for (int i = 0; i < constructableArea.Count; i++)
        {
            Destroy(constructableArea[i]);
        }
    }
}
