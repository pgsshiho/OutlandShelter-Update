using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;


public class TechTreeUnlock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string skillCode;
    [SerializeField] private string skillName;
    [SerializeField] private string skillDescription;

    [HideInInspector] public bool isUnlocked = false;

    public TextMeshProUGUI myText;
    public Image blocker;

    public static int skillPoint = 0;

    public static Dictionary<string, Action> effect = new Dictionary<string, Action>
    {
        {"S01", () => playerHP += 0.1f},
        {"S02", () => playerReceiveDamage -= 0.05f},
        {"S03", () => playerReceiveDamage -= 0.1f},
        {"S04", () => haveAvoidSkill = true},
        {"S05", () => haveIgnoreKnockBack = true},
        {"S06", () => additionalInvincibilityTime += 0.1f},
        {"S07", () => isMedikitOpen = true},
        {"S08", () => avoidSkillCoolTime -= 0.5f},
        {"S09", () => stiffenTime -= 0.3f},
        {"S10", () => duringAttackingReceiveDamage -= 0.5f},
        {"S11", () => lowHpReceiveDamage -= 0.15f},
        {"S12", () => continuousReceiveDamage -= 0.3f},
        {"S13", () => avoidSpeed += 0.1f},
        {"S14", () => duringMovingAccuracyFixed = true},
        {"S15", () => duringAttackingMoveSpeedFixed = true},
        {"S16", () => playerReceiveDamageOnBack -= 0.2f},
        {"S17", () => finalResistance = true},
        {"S18", () => lowHpAttackSpeed += 0.1f},
        {"S19", () => additionalInvincibilityTime += 0.15f},
        {"S20", () => afterAvoidDamage += 0.1f},
        {"S21", () => avoidProbability += 0.1f},
        {"S22", () => continuousIncreaseMoveSpeed += 0.03f},
        {"S23", () =>
            {
                float temp = 0.05f;
                survivalTreeAbilityI += temp;
                temp++;
                playerHP *= temp;
                playerReceiveDamage /= temp;
                additionalInvincibilityTime *= temp;
                avoidSkillCoolTime /= temp;
                stiffenTime /= temp;
                duringAttackingReceiveDamage /= temp;
                lowHpReceiveDamage /= temp;
                continuousReceiveDamage /= temp;
                avoidSpeed *= temp;
                playerReceiveDamageOnBack /= temp;
                playerMoveSpeed *= temp;
                lowHpAttackSpeed *= temp;
                afterAvoidDamage *= temp;
                avoidProbability *= temp;
                continuousIncreaseMoveSpeed *= temp;
            }
        },
        {"S24", () =>
            {
                float temp = 0.1f;
                survivalTreeAbilityII += temp;
                temp++;
                playerHP *= temp;
                playerReceiveDamage /= temp;
                additionalInvincibilityTime *= temp;
                avoidSkillCoolTime /= temp;
                stiffenTime /= temp;
                duringAttackingReceiveDamage /= temp;
                lowHpReceiveDamage /= temp;
                continuousReceiveDamage /= temp;
                avoidSpeed *= temp;
                playerReceiveDamageOnBack /= temp;
                playerMoveSpeed *= temp;
                lowHpAttackSpeed *= temp;
                afterAvoidDamage *= temp;
                avoidProbability *= temp;
                continuousIncreaseMoveSpeed *= temp;
            }
        },
        {"S25", () => additionalAvoidAbleTiming = true},
        {"S26", () => afterEndWaveRecoverHP += 0.05f * survivalTreeAbilityI * survivalTreeAbilityII},
        {"S27", () => closeAttackReceiveDamage -= 0.1f * survivalTreeAbilityI * survivalTreeAbilityII},
        {"S28", () => explosiveAttackReceiveDamage -= 0.1f * survivalTreeAbilityI * survivalTreeAbilityII},
        {"S29", () => duringNoDamageIncreaseDefence += 0.05f * survivalTreeAbilityI * survivalTreeAbilityII},
        {"S30", () => { playerHP += 0.15f * survivalTreeAbilityI * survivalTreeAbilityII; playerReceiveDamage -= 0.1f * survivalTreeAbilityI * survivalTreeAbilityII; avoidSkillCoolTime *= 0.85f / survivalTreeAbilityI / survivalTreeAbilityII; } },
        {"C01", () => useElectric -= 0.1f},
        {"C02", () => resourceSpending -= 0.05f},
        {"C03", () => facilityHP += 0.1f},
        {"C04", () => turretDamage += 0.1f},
        {"C05", () => healthFacilityRecoverySpeed += 0.05f},
        {"C06", () => fixing += 0.1f},
        {"C07", () => turretRange += 0.1f},
        {"C08", () => destroyedConstructionFixedSpendResource -= 0.2f},
        {"C09", () => facilityHP += 0.2f},
        {"C10", () => useElectric -= 0.1f},
        {"C11", () => turretDamage += 0.1f},
        {"C12", () => healthFacilityRecoverySpeed += 0.1f},
        {"C13", () => resourceSpending -= 0.05f},
        {"C14", () => fixing += 0.1f},
        {"C15", () =>
            {
                Resource resource = FindAnyObjectByType<Resource>();
                resource.baseShildTargetOffset.x = 3.25f;
                resource.baseTargetOffset.x = -3.25f;
                resource.shildUI.SetActive(true);
            }
        },
        {"C16", () => turretRange += 0.1f},
        {"C17", () => mineDamage += 0.2f},
        {"C18", () => healthFacilityRecoveryRange += 0.5f},
        {"C19", () => turretAttackSpeed += 0.3f},
        {"C20", () => capacity += 0.3f},
        {"C21", () => basecampDefence += 0.1f},
        {"C22", () => basecampDefence += 0.15f},
        {"C23", () => hammerRange += 0.3f},
        {"C24", () => turretRange += 0.1f},
        {"C25", () => healthFacilityRecoverySpeed += 0.1f},
        {"C26", () => useElectric -= 0.1f},
        {"C27", () => turretDamage += 0.1f},
        {"C28", () => openAutoFix = true},
        {"C29", () =>
            {
                constructionAbility += 0.05f;
                useElectric /= constructionAbility;
                resourceSpending /= constructionAbility;
                facilityHP *= constructionAbility;
                turretDamage *= constructionAbility;
                healthFacilityRecoverySpeed *= constructionAbility;
                fixing *= constructionAbility;
                turretRange *= constructionAbility;
                destroyedConstructionFixedSpendResource /= constructionAbility;
                mineDamage *= constructionAbility;
                healthFacilityRecoveryRange *= constructionAbility;
                turretAttackSpeed *= constructionAbility;
                capacity *= constructionAbility;
                basecampDefence *= constructionAbility;
                hammerRange *= constructionAbility;
            }
        },
        {"C30", () => infectionTreat = true},
        {"W01", () => meleeDamage += 0.05f},
        {"W02", () => meleeDamage += 0.1f},
        {"W03", () => attackSpeed += 0.05f},
        {"W04", () => attackSpeed += 0.1f},
        {"W05", () => reloadingTime -= 0.1f},
        {"W06", () => reloadingTime -= 0.2f},
        {"W07", () => attackSpeed += 0.15f},
        {"W08", () => shotSpeed += 0.2f},
        {"W09", () => throwRange += 0.2f},
        {"W10", () => throwCoolTime -= 0.15f},
        {"W11", () => additionalMineCount++},
        {"W12", () => mineDamage += 0.2f},
        {"W13", () => isPortableTurret = true},
        {"W14", () => turretDamage += 0.15f},
        {"W15", () =>
            {
                foreach (var temp in GunStatManager.instance)
                {
                    foreach (var temp2 in temp.Value.partsUnEquipEffect.Values)
                    {
                        temp2(temp.Key);
                    }
                }
                partsAbility += 0.15f;
                foreach (var temp in GunStatManager.instance)
                {
                    foreach (var temp2 in temp.Value.partsEffect.Values)
                    {
                        temp2(temp.Key);
                    }
                }
            }
        },
        {"W16", () => autoGunDamage += 0.1f},
        {"W17", () => meleeDamage += 0.15f},
        {"W18", () => magazineCapacity += 0.1f},
        {"W19", () =>
            {
                foreach (var temp in GunStatManager.instance)
                {
                    foreach (var temp2 in temp.Value.partsUnEquipEffect.Values)
                    {
                        temp2(temp.Key);
                    }
                }
                partsAbility += 0.15f;
                foreach (var temp in GunStatManager.instance)
                {
                    foreach (var temp2 in temp.Value.partsEffect.Values)
                    {
                        temp2(temp.Key);
                    }
                }
            }
        },
        {"W20", () =>
            {
                foreach (var temp in GunStatManager.instance)
                {
                    foreach (var temp2 in temp.Value.partsUnEquipEffect.Values)
                    {
                        temp2(temp.Key);
                    }
                }
                partsAbility += 0.15f;
                foreach (var temp in GunStatManager.instance)
                {
                    foreach (var temp2 in temp.Value.partsEffect.Values)
                    {
                        temp2(temp.Key);
                    }
                }
            }
        },
        {"W21", () => grenadeRange += 0.2f},
        {"W22", () => increaseMoveSpeedProbability += 0.1f},
        {"W23", () => comboDamageIncrease += 0.1f},
        {"W24", () => gunRange += 0.1f},
        {"W25", () => shotSpeed += 0.2f},
        {"W26", () => weaponDamage += 0.05f},
        {"W27", () => weaponDamage += 0.1f},
        {"W28", () => throwScale += 0.2f},
        {"W29", () =>
            {
                weaponAbility += 0.05f;
                meleeDamage *= weaponAbility;
                attackSpeed *= weaponAbility;
                reloadingTime /= weaponAbility;
                shotSpeed *= weaponAbility;
                throwRange *= weaponAbility;
                throwCoolTime /= weaponAbility;
                foreach (var temp in GunStatManager.instance)
                {
                    foreach (var temp2 in temp.Value.partsUnEquipEffect.Values)
                    {
                        temp2(temp.Key);
                    }
                }
                partsAbility *= weaponAbility;
                foreach (var temp in GunStatManager.instance)
                {
                    foreach (var temp2 in temp.Value.partsEffect.Values)
                    {
                        temp2(temp.Key);
                    }
                }
                autoGunDamage *= weaponAbility;
                magazineCapacity *= weaponAbility;
                grenadeRange *= weaponAbility;
                increaseMoveSpeedProbability *= weaponAbility;
                comboDamageIncrease *= weaponAbility;
                gunRange *= weaponAbility;
                weaponDamage *= weaponAbility;
                throwScale *= weaponAbility;
            }
        },
        {"W30", () => isRelodingSkip = true}
    };

    public static float playerHP = 1;
    public static float playerReceiveDamage = 1;
    public static bool haveAvoidSkill = false;
    public static bool haveIgnoreKnockBack = false;
    public static float additionalInvincibilityTime = 0;
    public static float avoidSkillCoolTime = 3;
    public static float stiffenTime = 1;
    public static float duringAttackingReceiveDamage = 1;
    public static float lowHpReceiveDamage = 1;
    public static float continuousReceiveDamage = 1;
    public static float avoidSpeed = 1;
    public static bool duringMovingAccuracyFixed = false;
    public static bool duringAttackingMoveSpeedFixed = false;
    public static float playerReceiveDamageOnBack = 1;
    public static float playerMoveSpeed = 1;
    public static bool finalResistance = false;
    public static float lowHpAttackSpeed = 1;
    public static float afterAvoidDamage = 1;
    public static float avoidProbability = 0;
    public static float continuousIncreaseMoveSpeed = 0;
    public const int S22MAXOVERWRAP = 5;
    public static float survivalTreeAbilityI = 1;
    public static float survivalTreeAbilityII = 1;
    public static bool additionalAvoidAbleTiming = false;
    public static float afterEndWaveRecoverHP = 0;
    public static float closeAttackReceiveDamage = 1;
    public static float explosiveAttackReceiveDamage = 1;
    public static float duringNoDamageIncreaseDefence = 0;
    public const int S29MAXOVERWRAP = 3;
    public static float useElectric = 1;
    public static float resourceSpending = 1;
    public static float facilityHP = 1;
    public static float turretDamage = 1;
    public static float healthFacilityRecoverySpeed = 1;
    public static float fixing = 1;
    public static float turretRange = 1;
    public static float destroyedConstructionFixedSpendResource = 1;
    public static float mineDamage = 1;
    public static float healthFacilityRecoveryRange = 1;
    public static float turretAttackSpeed = 1;
    public static float capacity = 1;
    public static float basecampDefence = 1;
    public static float hammerRange = 1;
    public static bool openAutoFix = false;
    public static float constructionAbility = 1;
    public static bool infectionTreat = false;
    public static float meleeDamage = 1;
    public static float attackSpeed = 1;
    public static float reloadingTime = 1;
    public static float shotSpeed = 1;
    public static float throwRange = 1;
    public static float throwCoolTime = 1;
    public static int additionalMineCount = 0;
    public static bool isPortableTurret = false;
    public static float partsAbility = 1;
    public static float autoGunDamage = 1;
    public static float magazineCapacity = 1;
    public static float grenadeRange = 1;
    public static float increaseMoveSpeedProbability = 0;
    public static float moveSpeed = 1;
    public static float comboDamageIncrease = 0;
    public static float gunRange = 1;
    public static float weaponDamage = 1;
    public static float throwScale = 1;
    public static float weaponAbility = 1;
    public static bool isRelodingSkip = false;
    public static bool isMedikitOpen = false;
    [HideInInspector] public string originalText;

    private void Awake()
    {
        originalText = myText.text;
    }

    public void Unlock()
    {
        if (!isUnlocked && skillPoint > 0)
        {
            skillPoint--;
            isUnlocked = true;
            myText.text = "Unlocked".Localize("skills");

            blocker.color = new Color(0, 1, 0, 0.4f);
            blocker.gameObject.SetActive(true);
            blocker.raycastTarget = true;

            if (effect.ContainsKey(skillCode))
            {
                effect[skillCode]();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isUnlocked)
        {
            blocker.color = new Color(0, 0, 1, 0.4f);
            blocker.gameObject.SetActive(true);

            myText.text = skillName.Localize("skills");
            Notion.ToolTip(skillDescription.Localize("skills"), true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isUnlocked)
        {
            myText.text = originalText;
            blocker.gameObject.SetActive(false);
            Notion.ToolTip("", false);
        }
    }

    private void OnDisable()
    {
        OnPointerExit(null);
    }
}