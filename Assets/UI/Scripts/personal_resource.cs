using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Personal_resource : MonoBehaviour, IDamageable, IEnemyAttackable
{
    [HideInInspector] public PlayerBag bag;
    public static Personal_resource instance;

    public int Wooden
    {
        get
        {
            return bag.resources[(int)PlayerBag.ResourceKind.Wooden];
        }
        set
        {
            bag.resources[(int)PlayerBag.ResourceKind.Wooden] = value;
        }
    }
    public int Steel
    {
        get
        {
            return bag.resources[(int)PlayerBag.ResourceKind.Steel];
        }
        set
        {
            bag.resources[(int)PlayerBag.ResourceKind.Steel] = value;
        }
    }
    public int Metal
    {
        get
        {
            return bag.resources[(int)PlayerBag.ResourceKind.Metal];
        }
        set
        {
            bag.resources[(int)PlayerBag.ResourceKind.Metal] = value;
        }
    }

    private readonly static float maxhp = 100.0f;
    private static float nowhp = 100.0f;

    public static float MaxHP
    {
        get
        {
            return maxhp * TechTreeUnlock.playerHP;
        }
    }
    public static float NowHP
    {
        get
        {
            return nowhp * TechTreeUnlock.playerHP;
        }
        set
        {
            nowhp = Mathf.Clamp(value, 0, maxhp);
        }
    }

    private static int needExp = 100;
    private static int curExp = 0;
    private static int level = 1;

    public static int CurExp
    {
        get
        {
            return curExp;
        }
        set
        {
            curExp = value;

            if (curExp >= needExp)
            {
                TechTreeUnlock.skillPoint++;
                level++;

                curExp -= needExp;

                Notion.Log("레벨업!!");

                needExp += 100;
            }
        }
    }

    [Header("UI 텍스트")]
    public TextMeshProUGUI woodenText;
    public TextMeshProUGUI steelText;
    public TextMeshProUGUI metalText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI levelText;

    [Header("슬라이더")]
    public Image hpSlider;
    public Image expSlider;

    [Header("UI 위치 조정용")]
    public RectTransform hpSliderUI;

    [Header("UI 위치 옵션")]
    public Vector2 hpSliderPositionBackpackOn = new(600, 391);

    [Header("고정 좌표 설정 (백팩 꺼짐 시)")]
    public Vector2 fixedHPPositionWhenBackpackOff = new(600, -500);

    private Rigidbody2D rb;

    public static bool isStiffen = false;

    private int continousHitStack = 0;

    public static float hpPercentage;

    public GameObject backpack;

    public void Damage(float damage, Vector2 knockBackDirection = new Vector2(), AttackType type = AttackType.None, float stiffenTime = 0.2f)
    {
        bool isEquipedArmor = ItemOwnManager.ownWeapon[Kind.Armor][ObjectPoolManager.instance[Kind.Armor].weaponIndex];

        NowHP = nowhp - (damage / TechTreeUnlock.playerHP * TechTreeUnlock.playerReceiveDamage
            * (Weapons.Weapon.isAttacking ? TechTreeUnlock.duringAttackingReceiveDamage : 1) * (NowHP / MaxHP <= 0.1f ? TechTreeUnlock.lowHpReceiveDamage : 1)
            * (continousHitStack > 0 ? TechTreeUnlock.continuousReceiveDamage : 1)
            * (Vector2.Dot(rb.linearVelocity.normalized, knockBackDirection.normalized) >= Mathf.Cos(45 * Mathf.Deg2Rad) ? TechTreeUnlock.playerReceiveDamageOnBack : 1)
            * (type == AttackType.Close ? TechTreeUnlock.closeAttackReceiveDamage : (type == AttackType.Explosion ? TechTreeUnlock.explosiveAttackReceiveDamage : 1))
            / (1 + TechTreeUnlock.duringNoDamageIncreaseDefence * Mathf.Clamp((int)(NoDamageTimer / 5), 0, TechTreeUnlock.S29MAXOVERWRAP)))
            * (isEquipedArmor ? Armor.armorStats[ObjectPoolManager.instance[Kind.Armor].weaponIndex].defence : 1);

        if (!TechTreeUnlock.haveIgnoreKnockBack) rb.AddForce(knockBackDirection, ForceMode2D.Impulse);

        if (TechTreeUnlock.finalResistance && nowhp < 1)
        {
            Notion.Log("불굴 발동!!!");
            nowhp = 1;
            TechTreeUnlock.finalResistance = false;
        }

        if (damage > 0)
        {
            isStiffen = true;
            StartCoroutine(WaitAction.wait(stiffenTime * TechTreeUnlock.stiffenTime, () =>
            {
                isStiffen = false;
            }));

            continousHitStack++;
            StartCoroutine(WaitAction.wait(3f, () =>
            {
                continousHitStack--;
            }));
        }

        if (NowHP <= 0)
        {
            Death();
        }
    }

    public static bool isDead = false;

    private void Death()
    {
        isDead = true;

        SceneChanger.BG("DeathScene");
    }

    private void Awake()
    {
        bag = GetComponent<PlayerBag>();
        rb = GetComponent<Rigidbody2D>();
        isDead = false;
        nowhp = maxhp;
        needExp = 100;
        curExp = 0;
        level = 1;
        isStiffen = false;
        BasicZombie.deathCount = 0;
        BasicZombie.increaseSpeed = 1;
        BasicZombie.stack = 0;
        GunStatManager.Awake();
        TechTreeUnlock.skillPoint = 0;
        TechTreeUnlock.playerHP = 1;
        TechTreeUnlock.playerReceiveDamage = 1;
        TechTreeUnlock.haveAvoidSkill = false;
        TechTreeUnlock.haveIgnoreKnockBack = false;
        TechTreeUnlock.additionalInvincibilityTime = 0;
        TechTreeUnlock.avoidSkillCoolTime = 3;
        TechTreeUnlock.stiffenTime = 1;
        TechTreeUnlock.duringAttackingReceiveDamage = 1;
        TechTreeUnlock.lowHpReceiveDamage = 1;
        TechTreeUnlock.continuousReceiveDamage = 1;
        TechTreeUnlock.avoidSpeed = 1;
        TechTreeUnlock.duringMovingAccuracyFixed = false;
        TechTreeUnlock.duringAttackingMoveSpeedFixed = false;
        TechTreeUnlock.playerReceiveDamageOnBack = 1;
        TechTreeUnlock.playerMoveSpeed = 1;
        TechTreeUnlock.finalResistance = false;
        TechTreeUnlock.lowHpAttackSpeed = 1;
        TechTreeUnlock.afterAvoidDamage = 1;
        TechTreeUnlock.avoidProbability = 0;
        TechTreeUnlock.continuousIncreaseMoveSpeed = 0;
        TechTreeUnlock.survivalTreeAbilityI = 1;
        TechTreeUnlock.survivalTreeAbilityII = 1;
        TechTreeUnlock.additionalAvoidAbleTiming = false;
        TechTreeUnlock.afterEndWaveRecoverHP = 0;
        TechTreeUnlock.closeAttackReceiveDamage = 1;
        TechTreeUnlock.explosiveAttackReceiveDamage = 1;
        TechTreeUnlock.duringNoDamageIncreaseDefence = 0;
        TechTreeUnlock.useElectric = 1;
        TechTreeUnlock.resourceSpending = 1;
        TechTreeUnlock.facilityHP = 1;
        TechTreeUnlock.turretDamage = 1;
        TechTreeUnlock.healthFacilityRecoverySpeed = 1;
        TechTreeUnlock.fixing = 1;
        TechTreeUnlock.turretRange = 1;
        TechTreeUnlock.destroyedConstructionFixedSpendResource = 1;
        TechTreeUnlock.mineDamage = 1;
        TechTreeUnlock.healthFacilityRecoveryRange = 1;
        TechTreeUnlock.turretAttackSpeed = 1;
        TechTreeUnlock.capacity = 1;
        TechTreeUnlock.basecampDefence = 1;
        TechTreeUnlock.hammerRange = 1;
        TechTreeUnlock.openAutoFix = false;
        TechTreeUnlock.constructionAbility = 1;
        TechTreeUnlock.infectionTreat = false;
        TechTreeUnlock.meleeDamage = 1;
        TechTreeUnlock.attackSpeed = 1;
        TechTreeUnlock.reloadingTime = 1;
        TechTreeUnlock.shotSpeed = 1;
        TechTreeUnlock.throwRange = 1;
        TechTreeUnlock.throwCoolTime = 1;
        TechTreeUnlock.additionalMineCount = 0;
        TechTreeUnlock.isPortableTurret = false;
        TechTreeUnlock.partsAbility = 1;
        TechTreeUnlock.autoGunDamage = 1;
        TechTreeUnlock.magazineCapacity = 1;
        TechTreeUnlock.grenadeRange = 1;
        TechTreeUnlock.increaseMoveSpeedProbability = 1;
        TechTreeUnlock.moveSpeed = 1;
        TechTreeUnlock.comboDamageIncrease = 0;
        TechTreeUnlock.gunRange = 1;
        TechTreeUnlock.weaponDamage = 1;
        TechTreeUnlock.throwScale = 1;
        TechTreeUnlock.weaponAbility = 1;
        TechTreeUnlock.isRelodingSkip = false;
        SummonObject.overWrap = 0;
        ConstructionTurret.points = null;

        instance = this;
    }

    float NoDamageTimer = 0;

    void Update()
    {
        UpdateUI();
        UpdateUIPosition();

        woodenText.text = Wooden.ToString();
        steelText.text = Steel.ToString();
        metalText.text = Metal.ToString();

        if (GunStatManager.instance[(GunKind)ObjectPoolManager.instance[Kind.Gun].weaponIndex].isLight)
        {
            if (Camera.main.orthographicSize != 8) Camera.main.orthographicSize = 8;
        }
        else
        {
            if (Camera.main.orthographicSize != 5) Camera.main.orthographicSize = 5;
        }

        NoDamageTimer += Time.deltaTime;
    }

    void UpdateUI()
    {
        woodenText.text = Wooden.ToString();
        steelText.text = Steel.ToString();
        metalText.text = Metal.ToString();

        hpPercentage = NowHP / MaxHP * 100f;

        float hpPercent = hpPercentage;
        hpText.text = hpPercent.ToString("0") + "%";
        hpSlider.fillAmount = NowHP / MaxHP;

        expText.text = $"{CurExp}/{needExp}";
        expSlider.fillAmount = (float)CurExp / (float)needExp;

        levelText.text = $"LV.{level}";
    }

    void UpdateUIPosition()
    {
        if (!backpack.activeSelf)
        {
            hpSliderUI.localPosition = fixedHPPositionWhenBackpackOff;
        }
        else
        {
            hpSliderUI.localPosition = hpSliderPositionBackpackOn;
        }
    }
}
