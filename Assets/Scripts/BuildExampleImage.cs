using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildExampleImage : MonoBehaviour
{
    private ResourceSpawner area;

    private Vector3 center;
    private Vector3 size;

    private GameObject constructableArea;
    private readonly List<GameObject> notConstructableArea = new List<GameObject>();

    [SerializeField] private GameObject rectangle;

    [SerializeField] private GameObject building;

    [SerializeField] private Vector2 buildingSize;

    public Buildings.Resource price;

    [SerializeField] private GameObject turretPointPrefab;
    private GameObject turretPoint;

    private void Awake()
    {
        area = FindAnyObjectByType<ResourceSpawner>();

        turretPoint = Instantiate(turretPointPrefab);

        BoxCollider2D field = building.GetComponent<BoxCollider2D>();

        GameObject child = Instantiate(rectangle, (Vector2)transform.position + field.offset, Quaternion.identity);
        child.transform.localScale = transform.localScale * field.size;

        child.transform.parent = transform;
        child.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.4f);

        Range range;

        range = area.range;
        center = (range.leftBottom + range.rightTop) / 2f;
        size = range.rightTop - range.leftBottom;
        notConstructableArea.Add(Instantiate(rectangle, center, Quaternion.identity));
        notConstructableArea[0].GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.2f);
        notConstructableArea[0].transform.localScale = size;

        for (int i = 1; i < area.constraints.Count; i++)
        {
            range = area.constraints[i];
            center = (range.leftBottom + range.rightTop) / 2f;
            size = range.rightTop - range.leftBottom;
            notConstructableArea.Add(Instantiate(rectangle, center, Quaternion.identity));
            notConstructableArea[i].GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.2f);
            notConstructableArea[i].transform.localScale = size;
        }

        range = area.constraints[0];
        center = (range.leftBottom + range.rightTop) / 2f;
        size = range.rightTop - range.leftBottom;
        constructableArea = Instantiate(rectangle, center, Quaternion.identity);
        constructableArea.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.4f);
        constructableArea.transform.localScale = size;
    }

    private void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);

        if (Input.GetMouseButtonDown(0))
        {
            if (IsOverlap(transform.position, transform.localScale, new List<Range>() { area.constraints[0] })
                && !IsOverlap(transform.position, transform.localScale, area.constraints.GetRange(1, area.constraints.Count - 1)))
            {
                GameObject temp = Instantiate(building, transform.position, Quaternion.identity);

                if (temp.TryGetComponent(out ResourceReturn _return))
                {
                    _return.returnResources.wooden = price.wooden / 3;
                    _return.returnResources.steel = price.steel / 3;
                    _return.returnResources.watt = price.watt;
                }

                area.constraints.Add(new Range((Vector2)transform.position + _return.boxCollider.offset - (Vector2)transform.localScale * _return.boxCollider.size / 2f,
                    (Vector2)transform.position + _return.boxCollider.offset + (Vector2)transform.localScale * _return.boxCollider.size / 2f));

                for (int i = 0; i < notConstructableArea.Count; i++)
                {
                    Destroy(notConstructableArea[i]);
                }
                Destroy(constructableArea);
                Destroy(turretPoint);
                Destroy(gameObject);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Resource.public_watt += price.watt;
            Personal_resource resource = Personal_resource.instance;
            resource.Wooden = Mathf.Clamp(resource.Wooden + price.wooden, 0, resource.bag.capacity);
            resource.Steel = Mathf.Clamp(resource.Steel + price.steel, 0, resource.bag.capacity);

            Notion.Log("건물 설치를 취소했습니다.");

            for (int i = 0; i < notConstructableArea.Count; i++)
            {
                Destroy(notConstructableArea[i]);
            }
            Destroy(constructableArea);
            Destroy(turretPoint);
            Destroy(gameObject);
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
}
