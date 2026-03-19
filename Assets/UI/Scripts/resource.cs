using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Resource : MonoBehaviour, ICenter, IEnemyAttackable
{
    public static int public_wooden = 0;
    public static int public_steel = 0;
    public static int public_metal = 0;
    public static int public_watt = 0;

    public float basemaxhp = 1000.0f;
    public float basenowhp = 1000.0f;
    public float baseShildMaxHp = 1000f;
    public float baseShildNowHp = 1000f;
    public float baseShildResistance = 0.9f;

    [Header("UI 텍스트")]
    public TextMeshProUGUI wattText;
    public TextMeshProUGUI basehptext;
    public TextMeshProUGUI baseShildHpText;

    [Header("슬라이더")]
    public Image basehpslider;
    public Image baseShildHpBar;

    [Header("UI 위치 조정용")]
    public RectTransform basehpUI;
    public Transform baseTarget;

    [Header("UI 위치 옵션")]
    public Vector2 basehpPositionBackpackOn = new(600, 288);
    public Vector3 baseTargetOffset = new(0, 1.5f, 0);
    public Vector3 baseShildTargetOffset = new(5, 1.5f, 0);

    public GameObject shildUI;

    public GameObject backpack;

    public void Damage(float amount)
    {
        if (!shildUI.activeSelf)
        {
            basenowhp = Mathf.Clamp(basenowhp - (amount / TechTreeUnlock.basecampDefence), 0, basemaxhp);
        }
        else
        {
            float temp = (baseShildNowHp * TechTreeUnlock.basecampDefence / baseShildResistance) - amount;

            baseShildNowHp = Mathf.Clamp(baseShildNowHp - (amount / TechTreeUnlock.basecampDefence * baseShildResistance), 0, baseShildMaxHp);

            if (temp > 0)
            {
                basenowhp = Mathf.Clamp(basenowhp - (temp / TechTreeUnlock.basecampDefence), 0, basemaxhp);
            }
        }

        if (basenowhp == 0)
        {
            Personal_resource.isDead = true;
            SceneChanger.BG("DeathScene");
        }
    }

    public void Recovery(float amount)
    {
        if (!shildUI.activeSelf)
        {
            basenowhp = Mathf.Clamp(basenowhp + amount, 0, basemaxhp);
        }
        else
        {
            float temp = (basenowhp + amount) - basemaxhp;

            basenowhp = Mathf.Clamp(basenowhp + amount, 0, basemaxhp);

            if (temp > 0)
            {
                baseShildNowHp = Mathf.Clamp(baseShildNowHp + amount, 0, baseShildMaxHp);
            }
        }
    }

    private void Awake()
    {
        public_wooden = 0;
        public_steel = 0;
        public_metal = 0;
        public_watt = 0;
}

    void Update()
    {
        UpdateUI();
        UpdateUIPosition();
    }

    void UpdateUI()
    {
        wattText.text = public_watt.ToString("0") + "w";

        float basehpPercent = (basenowhp / basemaxhp) * 100f;
        basenowhp = Mathf.Clamp(basenowhp, 0, basemaxhp);
        basehptext.text = basehpPercent.ToString("0") + "%";
        basehpslider.fillAmount = basenowhp / basemaxhp;

        if (shildUI.activeSelf)
        {
            float baseShildHp = baseShildNowHp / baseShildMaxHp;
            baseShildHpText.text = (baseShildHp * 100).ToString("0") + "%";
            baseShildHpBar.fillAmount = baseShildHp;
        }
    }

    void UpdateUIPosition()
    {
        if (!backpack.activeSelf)
        {
            if (baseTarget != null)
            {
                Vector3 baseScreenPos = Camera.main.WorldToScreenPoint(baseTarget.position + baseTargetOffset);
                basehpUI.position = baseScreenPos;
                baseScreenPos = Camera.main.WorldToScreenPoint(baseTarget.position + baseShildTargetOffset);

                shildUI.transform.position = baseScreenPos;
            }
        }
        else
        {
            basehpUI.localPosition = basehpPositionBackpackOn;
        }
    }
}
