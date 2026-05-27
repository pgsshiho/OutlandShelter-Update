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

    [SerializeField]
    private bool usePooling = true; // 풀링 사용 여부를 Inspector에서 설정할 수 있도록 추가

    private void Awake()
    {
        instance[key] = this;

        Init();
    }

    private void Init()
    {
        if (usePooling)
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
                for (int i = 0; i < defaultCapacity[weaponIndex]; i++)
                {
                    Pool.Release(CreatePooledItem());
                }
            }
        }
        else
        {
            Pool = new InstantiateDestroyPool(CreateTransientItem, OnTakeFromPool);
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

    // 풀링을 사용하지 않을 때, 매번 새로 생성하는 함수
    private GameObject CreateTransientItem()
    {
        GameObject go = Instantiate(summonPrefab[weaponIndex]);

        if (go.TryGetComponent(out SummonRPG rpg))
        {
            rpg.pool = pool[weaponIndex];
        }

        return go;
    }

    private sealed class InstantiateDestroyPool : IObjectPool<GameObject>
    {
        private readonly System.Func<GameObject> createFunc;
        private readonly System.Action<GameObject> actionOnGet;

        public int CountInactive => 0;

        public InstantiateDestroyPool(
            System.Func<GameObject> createFunc,
            System.Action<GameObject> actionOnGet
        )
        {
            this.createFunc = createFunc;
            this.actionOnGet = actionOnGet;
        }

        public GameObject Get()
        {
            GameObject go = createFunc();
            actionOnGet?.Invoke(go);
            return go;
        }

        public PooledObject<GameObject> Get(out GameObject v)
        {
            v = Get();
            return new PooledObject<GameObject>(v, this);
        }

        public void Release(GameObject element)
        {
            if (element != null)
                Destroy(element);
        }

        public void Clear() { }
    }
}
