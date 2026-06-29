using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType
{
    Default,
    Generator,
    Workbench,
    WeaponWorkbench,
}

public class BuildExampleImage : MonoBehaviour
{
    [SerializeField] private BuildingType currentBuildingType = BuildingType.Default;
    private ResourceSpawner area;

    private Vector3 center;
    private Vector3 size;

    private GameObject constructableArea;
    private readonly List<GameObject> notConstructableArea = new List<GameObject>();

    [SerializeField] private GameObject rectangle;

    [SerializeField] private GameObject building;          // 일반 모드에서 설치될 건물 프리패브
    [SerializeField] private GameObject tutoBuilding;      // 💡 튜토리얼 모드에서 설치될 건물 프리패브

    [SerializeField] private Vector2 buildingSize;

    public Buildings.Resource price;

    [SerializeField] private GameObject turretPointPrefab;
    private GameObject turretPoint;

    private void Awake()
    {
        area = FindAnyObjectByType<ResourceSpawner>();

        turretPoint = Instantiate(turretPointPrefab);

        GameObject targetPrefab = (Tutorial.instance != null && Tutorial.instance.isTutorial) ? tutoBuilding : building;

        // 혹시 튜토리얼용 프리패브를 깜빡하고 안 넣었을 때를 대비한 안전장치
        if (targetPrefab == null) targetPrefab = building;

        BoxCollider2D field = targetPrefab.GetComponent<BoxCollider2D>();

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
                // 💡 [핵심 변환] 튜토리얼 중이라면 tutoBuilding을 소환하고, 아니면 일반 building을 소환합니다.
                GameObject prefabToSpawn = (Tutorial.instance != null && Tutorial.instance.isTutorial) ? tutoBuilding : building;
                if (prefabToSpawn == null) prefabToSpawn = building; // 방어 코드

                GameObject temp = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);

                if (SFXReference.Instance.construct != null)
                {
                    SoundManager.SFX.PlayOneShot(SFXReference.Instance.construct, 0.7f);
                }

                // 건물 유형 카운팅 및 로그 정상화
                switch (currentBuildingType)
                {
                    case BuildingType.Generator:
                        if (Tutorial.instance != null) Tutorial.instance.buildings[0]++;
                        Debug.Log("발전기 설치 완료!");
                        break;

                    case BuildingType.Workbench:
                        if (Tutorial.instance != null) Tutorial.instance.buildings[1]++;
                        Debug.Log("작업대 설치 완료!");
                        break;

                    case BuildingType.WeaponWorkbench:
                        if (Tutorial.instance != null) Tutorial.instance.buildings[2]++;
                        Debug.Log("무기 작업대 설치 완료!");
                        break;

                    default:
                        Debug.Log("일반 건물 설치 완료!");
                        break;
                }

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
                if (turretPoint != null) Destroy(turretPoint);
                Destroy(gameObject);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Resource.public_watt += price.watt;
            Personal_resource resource = Personal_resource.instance;
            resource.Wooden = Mathf.Clamp(resource.Wooden + price.wooden, 0, resource.bag.capacity);
            resource.Steel = Mathf.Clamp(resource.Steel + price.steel, 0, resource.bag.capacity);

            Notion.Log("Cancle building".Localize());

            for (int i = 0; i < notConstructableArea.Count; i++)
            {
                Destroy(notConstructableArea[i]);
            }
            Destroy(constructableArea);
            if (turretPoint != null) Destroy(turretPoint);
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