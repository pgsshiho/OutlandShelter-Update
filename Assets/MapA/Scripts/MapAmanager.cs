using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Threading;

public class MapManager : MonoBehaviour
{
    public static int waveCount = 0;
    public float waveTimerLimit = 120f;
    public float restTimerLimit = 60f;

    private float waveTimer;
    private float restTimer;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI wavecount;
    public TextMeshProUGUI leftzombie;

    public bool isWave = false;
    public int zombieCount = 0;
    private bool waveEnded = false;
    public GameObject restskip;
    public GameObject settingpanel;

    private ResourceSpawner spawner;

    public static int currentZombieCount = 0;

    [System.Serializable]
    public struct Wave
    {
        public GameObject[] summonZombie;
    }

    public Wave[] waves;

    public Transform[] spawners;

    private ObjectPoolManager poolManager;
    public static bool isActivePanel = false;

    private void Awake()
    {
        waveCount = 0;
        currentZombieCount = 0;
        isActivePanel = false;
        StartRest();
    }
    
    private void Start()
    {
        spawner = FindAnyObjectByType<ResourceSpawner>();
        poolManager = ObjectPoolManager.instance[Kind.Zombie];
    }

    private void Update()
    {
        if (!(Guide.isEnable || UIOpen.isEnable.ContainsValue(true)) && Input.GetKeyUp(KeyCode.Escape))
        {
            settingpanel.SetActive(!settingpanel.activeSelf);
            isActivePanel = settingpanel.activeSelf;
        }

        if (isWave)
        {
            if (waveCount != waves.Length)
            {
                if (waveTimer > 0)
                {
                    waveTimer = Mathf.Clamp(waveTimer - Time.deltaTime, 0, waveTimerLimit);
                    UpdateTimerUI(waveTimer);

                    if (currentZombieCount <= 0)
                    {
                        waveEnded = true;
                        Notion.Log("ZombieAllkill".Localize());
                        StartRest();
                    }
                }
                else
                {
                    if (!waveEnded)
                    {
                        Notion.Log("TimerDone".Localize());
                        StartRest();
                    }
                }
            }
            else
            {
                timerText.text = "00:00";
                wavecount.text = $"Wave {waveCount}";
                leftzombie.text = $"{currentZombieCount}";

                if (currentZombieCount <= 0 && !Personal_resource.isDead)
                {
                    Ending();
                }
            }
        }
        else
        {
            if (restTimer > 0)
            {
                restTimer = Mathf.Clamp(restTimer - Time.deltaTime, 0, restTimerLimit);
                UpdateTimerUI(restTimer);
            }
            else
            {
                StartWave();
            }
        }
    }

    private void UpdateTimerUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        wavecount.text = $"Wave {waveCount}";
        leftzombie.text = $"{currentZombieCount}";
    }

    public void StartWave()
    {
        Personal_resource.NowHP += Personal_resource.MaxHP * TechTreeUnlock.afterEndWaveRecoverHP;
        isWave = true;
        waveEnded = false;
        waveCount++;
        waveTimer = waveTimerLimit;
        restskip.SetActive(false);

        spawner.Spawn();

        SetZombieCount(waveCount * 9);
        
        if (waveCount != waves.Length) for (int i = 0; i < spawners.Length; i++) StartCoroutine(SpawnZombie(zombieCount, spawners[i], waves[waveCount - 1].summonZombie.Length, waveCount));
        else StartCoroutine(SpawnZombie(1, spawners[Random.Range(0, spawners.Length)], waves[waveCount - 1].summonZombie.Length, waveCount));

        Notion.Log("WaveStart".Localize("En",waveCount));
    }

    private IEnumerator SpawnZombie(int count, Transform spawner, int zombieIndex, int waveCount)
    {
        if (count != 1)
            for (int i = 0; i < count / spawners.Length; i++)
            {
                GameObject summonZombie = waves[waveCount - 1].summonZombie[Random.Range(0, zombieIndex)];

                for (int j = 0; j < poolManager.summonPrefab.Length; j++)
                {
                    if (poolManager.summonPrefab[j].name == summonZombie.name)
                    {
                        poolManager.weaponIndex = j;
                    }
                }

                GameObject zombie = poolManager.Pool.Get();

                currentZombieCount++;

                zombie.transform.position = spawner.position;

                yield return new WaitForSeconds(5f / (count / spawners.Length));
            }
        else
        {
            GameObject summonZombie = waves[waveCount - 1].summonZombie[0];

            poolManager.weaponIndex = poolManager.summonPrefab.ToList().IndexOf(summonZombie);

            GameObject zombie = poolManager.Pool.Get();

            currentZombieCount++;

            zombie.transform.position = spawner.position;

            yield return new WaitForSeconds(5f / (count / spawners.Length));
        }
    }

    public void StartRest()
    {
        isWave = false;
        waveEnded = false;
        restTimer = restTimerLimit;
        restskip.SetActive(true);
        Notion.Log("maintenance".Localize());
    }

    public void SetZombieCount(int count)
    {
        zombieCount = count;
    }

    public void SkipRest()
    {
        if (!isWave)
        {
            StartWave();
        }
    }

    public void Goback()
    {
        SceneChanger.BG("Mainmenu");
    }

    private void Ending()
    {
        SceneChanger.BG("EndingScene");
    }
}
