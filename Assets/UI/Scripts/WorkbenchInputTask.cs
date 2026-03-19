using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkbenchInputTask : MonoBehaviour
{
    public float spendTime;
    public float StartTime
    {
        get; private set;
    }
    public Image taskBar;
    public TextMeshProUGUI timeCheck;
    [SerializeField] private TextMeshProUGUI[] material;
    [SerializeField] private float[] prices;

    public string itemName;
    [HideInInspector] public bool isComplete = true;

    private PlayerBag bag;

    public static Queue<TextMeshProUGUI> taskQueue = new Queue<TextMeshProUGUI>();

    private void Awake()
    {
        bag = FindAnyObjectByType<PlayerBag>();
        material[0].text = ": " + prices[0];
        material[1].text = ": " + prices[1];
        material[2].text = ": " + prices[2];
        material[3].text = ": " + prices[3];
    }

    private void Update()
    {
        if (spendTime != -1)
        {
            if (!isComplete)
            {
                taskBar.fillAmount = (Time.time - StartTime) / spendTime;
                timeCheck.text = $"대기 순서 : {taskQueue.ToList().IndexOf(timeCheck)}\n남은 시간 : {(int)(WorkingManager.remainingTime[this] / 60f)}분 {(int)(WorkingManager.remainingTime[this] % 60)}초";
            }
            else
            {
                timeCheck.text = $"대기 순서 : {taskQueue.ToList().IndexOf(timeCheck)}\n남은 시간 : {(int)(spendTime / 60f)}분 {(int)(spendTime % 60)}초";
            }
        }
    }

    public void StartTask()
    {
        int tempWooden = (int)(prices[(int)Backpack.ResourceKind.Wooden] * TechTreeUnlock.resourceSpending);
        int tempSteel = (int)(prices[(int)Backpack.ResourceKind.Steel] * TechTreeUnlock.resourceSpending);
        int tempMetal = (int)(prices[(int)Backpack.ResourceKind.Metal] * TechTreeUnlock.resourceSpending);
        int tempWatt = (int)(prices[3] * TechTreeUnlock.useElectric);

        if (bag.resources[(int)Backpack.ResourceKind.Wooden] >= tempWooden
            && bag.resources[(int)Backpack.ResourceKind.Steel] >= tempSteel
            && bag.resources[(int)Backpack.ResourceKind.Metal] >= tempMetal
            && Resource.public_watt >= tempWatt)
        {
            bag.resources[(int)Backpack.ResourceKind.Wooden] -= tempWooden;
            bag.resources[(int)Backpack.ResourceKind.Steel] -= tempSteel;
            bag.resources[(int)Backpack.ResourceKind.Metal] -= tempMetal;
            Resource.public_watt -= tempWatt;

            taskQueue.Enqueue(timeCheck);
            StartCoroutine(WaitAction.wait(() => { return taskQueue.Peek() == timeCheck; }, () =>
            {
                StartTime = Time.time;
                isComplete = false;
            }));
            WorkingManager.remainingTime[this] = spendTime;
            taskBar.fillAmount = 0;
            taskBar.transform.parent.gameObject.SetActive(true);
            timeCheck.text = $"대기 순서 : {taskQueue.ToList().IndexOf(timeCheck)}\n남은 시간 : {(int)(WorkingManager.remainingTime[this] / 60f)}분 {(int)(WorkingManager.remainingTime[this] % 60)}초";
        }
        else
        {
            Notion.Warning("자원이 부족합니다!!!");
        }
    }
}
