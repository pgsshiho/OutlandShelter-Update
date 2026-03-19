using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Guide : MonoBehaviour
{
    public GameObject guide;
    protected PlayerGuideUI guideUI;
    public KeyCode keyCode;
    public static bool isEnable = false;

    public void GuideOnOff(PlayerGuideUI playerGuideUI, bool value)
    {
        guideUI = playerGuideUI;
        if (guideUI != null) guideUI.selectGuide.Add(this);
        if (guide != null) guide.SetActive(value);
    }

    protected virtual void Update()
    {
        if (guideUI != null && !UIOpen.isEnable.ContainsValue(true) && !MapManager.isActivePanel)
        {
            if (Input.GetKeyDown(keyCode))
            {
                guideUI.isOpened = !guideUI.isOpened;
                isEnable = guideUI.isOpened;
                if (guideUI.isOpened)
                {
                    Enable();
                }
                else
                {
                    Disable();
                }
            }
            else if (guideUI.isOpened && Input.GetKeyUp(KeyCode.Escape))
            {
                StartCoroutine(WaitAction.waitOneFrame(() =>
                {
                    guideUI.isOpened = false;
                    isEnable = false;
                    Disable();
                }));
            }
        }
    }

    public abstract void Enable();
    public abstract void Disable();
}
