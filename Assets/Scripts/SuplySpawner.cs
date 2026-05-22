using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class SuplySpawn : MonoBehaviour
{
    public static int resourceSpawnCount = 2;

    public Range range;

    public List<Range> constraints = new List<Range>();

    private ObjectPoolManager SuplyPoolManagger;

    private void Start()
    {
        SuplyPoolManagger = ObjectPoolManager.instance[Kind.Suply];

        Spawn();
    }

    public void Spawn()
    {
        int random = Random.Range(1, 101);
        if(random > 25)
        {
            return;
        }
        GameObject[] Suply = new GameObject[resourceSpawnCount];

        for (int i = 0; i < resourceSpawnCount; i++)
        {
            Suply[i] = SuplyPoolManagger.Pool.Get();

            if (Suply[i].TryGetComponent(out ResourceObject resource))
            {
                resource.pool = SuplyPoolManagger.Pool;
            }
        }

        int index = 0;

        while (index < resourceSpawnCount)
        {
            Vector3 tempSuply = new Vector3
                (
                    Random.Range(range.leftBottom.x + Suply[index].transform.localScale.x / 2f, range.rightTop.x - Suply[index].transform.localScale.x / 2f),
                    Random.Range(range.leftBottom.y + Suply[index].transform.localScale.y / 2f, range.rightTop.y - Suply[index].transform.localScale.y / 2f)
                );

            if (IsOverlap(tempSuply, Suply[index].transform.localScale, constraints)) continue;

            Range woodenRange = new Range(tempSuply - Suply[index].transform.localScale / 2f, tempSuply + Suply[index].transform.localScale / 2f);

            Suply[index].transform.position = tempSuply;

            constraints.Add(woodenRange);

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
