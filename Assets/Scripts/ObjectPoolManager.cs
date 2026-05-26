using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static Dictionary<Kind, ObjectPoolManager> instance = new();
    public int[] defaultCapacity;
    public int[] maxPoolSize;
    public GameObject[] summonPrefab;
    public int weaponIndex = 0;
    public Kind key;
    public Dictionary<int, List<GameObject>> clones = new();

    private readonly Dictionary<int, IObjectPool<GameObject>> pool = new();

    public IObjectPool<GameObject> Pool
    {
        get
        {
            if (!pool.ContainsKey(weaponIndex))
                Init();
            return pool[weaponIndex];
        }
        private set { pool[weaponIndex] = value; }
    }

    private void Awake()
    {
        instance[key] = this;

        Init();
    }

    private void Init()
    {
        Pool = new ObjectPool<GameObject>(
            CreatePooledItem,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            true,
            defaultCapacity[weaponIndex],
            maxPoolSize[weaponIndex]
        );

        if (!clones.ContainsKey(weaponIndex))
            clones[weaponIndex] = new List<GameObject>();

        if (clones[weaponIndex].Count < defaultCapacity[weaponIndex])
        {
            // 초기화
            for (int i = 0; i < defaultCapacity[weaponIndex]; i++)
            {
                Pool.Release(CreatePooledItem());
            }
        }
    }

    // Pool 생성
    private GameObject CreatePooledItem()
    {
        GameObject go = Instantiate(summonPrefab[weaponIndex]);

        // 2. 생성된 오브젝트에 "너의 주인(Pool)은 얘야"라고 알려줍니다. (가장 중요!)
        if (go.TryGetComponent(out SummonRPG rpg))
        {
            rpg.pool = pool[weaponIndex];
        }

        // 3. 리스트에 기록하고 오브젝트를 반환합니다.
        if (!clones.ContainsKey(weaponIndex))
            clones[weaponIndex] = new List<GameObject>();
        clones[weaponIndex].Add(go);

        return go;
    }

    // Get
    private void OnTakeFromPool(GameObject poolGo)
    {
        poolGo.SetActive(true);
    }

    // Release
    private void OnReturnedToPool(GameObject poolGo)
    {
        poolGo.SetActive(false);
    }

    // Destroy
    private void OnDestroyPoolObject(GameObject poolGo)
    {
        Destroy(poolGo);
    }
}
