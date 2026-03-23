using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct Range
{
    public Vector2 leftBottom;
    public Vector2 rightTop;

    public Range(Vector2 leftBottom, Vector2 rightTop)
    {
        this.leftBottom = leftBottom;
        this.rightTop = rightTop;
    }
}

public class ResourceSpawner : MonoBehaviour
{
    public static int resourceSpawnCount = 2;

    public Range range;

    public List<Range> constraints = new List<Range>();

    private ObjectPoolManager woodenPoolManager;
    private ObjectPoolManager steelPoolManager;

    private void Start()
    {
        woodenPoolManager = ObjectPoolManager.instance[Kind.Wooden];
        steelPoolManager = ObjectPoolManager.instance[Kind.Steel];

        Spawn();
    }

    public void Spawn()
    {
        GameObject[] woodens = new GameObject[resourceSpawnCount];
        GameObject[] steels = new GameObject[resourceSpawnCount];

        for (int i = 0; i < resourceSpawnCount; i++)
        {
            woodens[i] = woodenPoolManager.Pool.Get();
            steels[i] = steelPoolManager.Pool.Get();

            if (woodens[i].TryGetComponent(out ResourceObject resource))
            {
                resource.pool = woodenPoolManager.Pool;
            }
            if (steels[i].TryGetComponent(out ResourceObject resourceObject))
            {
                resourceObject.pool = steelPoolManager.Pool;
            }
        }

        int index = 0;

        while (index < resourceSpawnCount)
        {
            Vector3 tempWooden = new Vector3
                (
                    Random.Range(range.leftBottom.x + woodens[index].transform.localScale.x / 2f, range.rightTop.x - woodens[index].transform.localScale.x / 2f),
                    Random.Range(range.leftBottom.y + woodens[index].transform.localScale.y / 2f, range.rightTop.y - woodens[index].transform.localScale.y / 2f)
                );
            Vector3 tempSteel = new Vector3
                (
                    Random.Range(range.leftBottom.x + steels[index].transform.localScale.x / 2f, range.rightTop.x - steels[index].transform.localScale.x / 2f),
                    Random.Range(range.leftBottom.y + steels[index].transform.localScale.y / 2f, range.rightTop.y - steels[index].transform.localScale.y / 2f)
                );

            if (IsOverlap(tempWooden, woodens[index].transform.localScale, constraints)) continue;

            if (IsOverlap(tempSteel, steels[index].transform.localScale, constraints)) continue;

            Range woodenRange = new Range(tempWooden - woodens[index].transform.localScale / 2f, tempWooden + woodens[index].transform.localScale / 2f);

            if (IsOverlap(tempSteel, steels[index].transform.localScale, new List<Range>() { woodenRange })) continue;

            Range steelRange = new Range(tempSteel - steels[index].transform.localScale / 2f, tempSteel + steels[index].transform.localScale / 2f);

            woodens[index].transform.position = tempWooden;
            steels[index].transform.position = tempSteel;

            constraints.Add(woodenRange);
            constraints.Add(steelRange);

            index++;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(range.leftBottom, new Vector3(range.rightTop.x, range.leftBottom.y));
        Gizmos.DrawLine(range.rightTop, new Vector3(range.rightTop.x, range.leftBottom.y));
        Gizmos.DrawLine(range.leftBottom, new Vector3(range.leftBottom.x, range.rightTop.y));
        Gizmos.DrawLine(range.rightTop, new Vector3(range.leftBottom.x, range.rightTop.y));

        if (constraints != null && constraints.Count > 0)
            foreach (Range range in constraints)
            {
                Vector3[] verts = new Vector3[4]
                {
                    (Vector3)range.rightTop,
                    new Vector3(range.rightTop.x, range.leftBottom.y),
                    (Vector3)range.leftBottom,
                    new Vector3(range.leftBottom.x, range.rightTop.y)
                };

#if UNITY_EDITOR
                Handles.DrawSolidRectangleWithOutline(verts, new Color(1, 0, 0, 0.2f), Color.red);
#endif
            }
    }
}
