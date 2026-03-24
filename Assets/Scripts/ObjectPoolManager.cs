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
            if (!pool.ContainsKey(weaponIndex)) Init();
            return pool[weaponIndex];
        }
        private set
        {
            pool[weaponIndex] = value;
        }
    }

    private void Awake()
    {
        instance[key] = this;

        Init();
    }

    private void Init()
    {
        Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
        OnDestroyPoolObject, true, defaultCapacity[weaponIndex], maxPoolSize[weaponIndex]);

        if (!clones.ContainsKey(weaponIndex)) clones[weaponIndex] = new List<GameObject>();

        if (clones[weaponIndex].Count < defaultCapacity[weaponIndex])
        {
            // �̸� ������Ʈ ���� �س���
            for (int i = 0; i < defaultCapacity[weaponIndex]; i++)
            {
                Pool.Release(CreatePooledItem());
            }
        }
    }

    // ����
    private GameObject CreatePooledItem()
    {
        int index = clones[weaponIndex].Count;
        clones[weaponIndex].Add(Instantiate(summonPrefab[weaponIndex]));
        return clones[weaponIndex][index];
    }

    // ���
    private void OnTakeFromPool(GameObject poolGo)
    {
        poolGo.SetActive(true);
    }

    // ��ȯ
    private void OnReturnedToPool(GameObject poolGo)
    {
        poolGo.SetActive(false);
    }

    // ����
    private void OnDestroyPoolObject(GameObject poolGo)
    {
        Destroy(poolGo);
    }
}