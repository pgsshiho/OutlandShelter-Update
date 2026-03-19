using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOpen : MonoBehaviour
{
    public GameObject ui;
    public KeyCode keyCode;
    public static Dictionary<KeyCode, bool> isEnable = new();
    public static bool isBlocking = false;

    protected void Awake()
    {
        isEnable[keyCode] = false;
        isBlocking = false;
    }

    protected virtual void Update()
    {
        if (!Guide.isEnable && !MapManager.isActivePanel)
        {
            bool alreadyEnable = false;

            foreach (var temp in isEnable)
            {
                if (temp.Key != keyCode && temp.Value)
                {
                    alreadyEnable = true;
                }
            }

            if (Input.GetKeyUp(keyCode) && !alreadyEnable && !isBlocking)
            {
                if (!ui.activeSelf)
                {
                    ui.SetActive(true);
                    isEnable[keyCode] = true;
                }
                else
                {
                    ui.SetActive(false);
                    isEnable[keyCode] = false;
                }
            }

            if (Input.GetKeyUp(KeyCode.Escape) && !alreadyEnable && ui.activeSelf && !isBlocking)
            {
                StartCoroutine(WaitAction.waitOneFrame(() =>
                {
                    ui.SetActive(false);
                    isEnable[keyCode] = false;
                }));
            }
        }
    }
}
