using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        {"S07", () => avoidSkillCoolTime -= 0.5f},
        {"S08", () => stiffenTime -= 0.3f},
        {"S09", () => duringAttackingReceiveDamage -= 0.5f},
        {"S10", () => lowHpReceiveDamage -= 0.15f},
        {"S11", () => continuousReceiveDamage -= 0.3f},
        {"S12", () => avoidSpeed += 0.1f},
        {"S13", () => duringMovingAccuracyFixed = true},
        {"S14", () => duringAttackingMoveSpeedFixed = true},
        {"S15", () => playerReceiveDamageOnBack -= 0.2f},
        {"S16", () => playerMoveSpeed += 0.2f},
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

    #region 강화 변수
    /// <summary>
    /// 플레이어 체력 배수
    /// </summary>
    public static float playerHP = 1; // 적용

    /// <summary>
    /// 플레이어 받는 피해 배수
    /// </summary>
    public static float playerReceiveDamage = 1; // 적용

    /// <summary>
    /// 회피 스킬 보유 유무
    /// </summary>
    public static bool haveAvoidSkill = false; // 적용

    /// <summary>
    /// 넉백 면역 보유 유무
    /// </summary>
    public static bool haveIgnoreKnockBack = false; // 적용

    /// <summary>
    /// 회피 스킬 추가 지속시간
    /// </summary>
    public static float additionalInvincibilityTime = 0; // 적용

    /// <summary>
    /// 회피 스킬 쿨타임
    /// </summary>
    public static float avoidSkillCoolTime = 3; // 적용

    /// <summary>
    /// 피격 시 경직 시간 배수
    /// </summary>
    public static float stiffenTime = 1; // 적용

    /// <summary>
    /// 공격 중 피격 데미지 배수
    /// </summary>
    public static float duringAttackingReceiveDamage = 1; // 적용

    /// <summary>
    /// 체력이 10%이하 시 받는 피해 배수
    /// </summary>
    public static float lowHpReceiveDamage = 1; // 적용

    /// <summary>
    /// 연속 피격 시 받는 피해 배수
    /// </summary>
    public static float continuousReceiveDamage = 1; // 적용

    /// <summary>
    /// 회피 스킬 회피 속도 배율
    /// </summary>
    public static float avoidSpeed = 1; // 적용

    /// <summary>
    /// 이동 중 명중률이 고정되는 지 유무
    /// </summary>
    public static bool duringMovingAccuracyFixed = false; // 적용

    /// <summary>
    /// 공격 중 이동속도 고정 유무
    /// </summary>
    public static bool duringAttackingMoveSpeedFixed = false; // 적용

    /// <summary>
    /// 후방에서 공격 받았을 때의 피해 배수
    /// </summary>
    public static float playerReceiveDamageOnBack = 1; // 적용

    /// <summary>
    /// 플레이어 이동 속도 배수
    /// </summary>
    public static float playerMoveSpeed = 1; // 적용

    /// <summary>
    /// 불굴 스킬을 찍었는 지 유무
    /// </summary>
    public static bool finalResistance = false; // 적용

    /// <summary>
    /// 체력이 20%이하일 때의 공격 속도 배수
    /// </summary>
    public static float lowHpAttackSpeed = 1; // 적용

    /// <summary>
    /// 회피 후 3초간 증가하는 공격력 배수
    /// </summary>
    public static float afterAvoidDamage = 1; // 적용

    /// <summary>
    /// 피격 시 회피 스킬을 발동할 확률(0~1)
    /// </summary>
    public static float avoidProbability = 0; // 적용

    /// <summary>
    /// 연속 처치 시 증가하는 이동속도 배수
    /// </summary>
    public static float continuousIncreaseMoveSpeed = 0; // 적용
    /// <summary>
    /// continuousIncreaseMoveSpeed 최대 중첩 수
    /// </summary>
    public const int S22MAXOVERWRAP = 5; // 적용

    /// <summary>
    /// 생존 트리 능력 배수(<- 이게 제일 뭣같음)
    /// </summary>
    public static float survivalTreeAbilityI = 1; // 적용
    public static float survivalTreeAbilityII = 1; // 적용

    /// <summary>
    /// 회피 가능 타이밍이 늘어났는지 유무
    /// </summary>
    public static bool additionalAvoidAbleTiming = false; // 적용

    /// <summary>
    /// 웨이브가 끝나고 회복되는 체력 퍼센테이지
    /// </summary>
    public static float afterEndWaveRecoverHP = 0; // 적용

    /// <summary>
    /// 근접 공격 피해 배수
    /// </summary>
    public static float closeAttackReceiveDamage = 1; // 적용

    /// <summary>
    /// 폭발 범위 피해 배수
    /// </summary>
    public static float explosiveAttackReceiveDamage = 1; // 적용

    /// <summary>
    /// 5초간 피해가 없을 시 증가할 방어력 퍼센테이지
    /// </summary>
    public static float duringNoDamageIncreaseDefence = 0; // 적용
    /// <summary>
    /// S29 최대 중첩 횟수
    /// </summary>
    public const int S29MAXOVERWRAP = 3; // 적용

    /// <summary>
    /// 구조물 소모 전기 배율
    /// </summary>
    public static float useElectric = 1; // 적용

    /// <summary>
    /// 건설 시 자원 소비량 배수
    /// </summary>
    public static float resourceSpending = 1; // 적용

    /// <summary>
    /// 모든 구조물 체력 배수
    /// </summary>
    public static float facilityHP = 1; // 적용

    /// <summary>
    /// 포탑 공격력 배수
    /// </summary>
    public static float turretDamage = 1; // 적용

    /// <summary>
    /// 회복 시설의 회복 속도 배수
    /// </summary>
    public static float healthFacilityRecoverySpeed = 1; // 적용

    /// <summary>
    /// 수리 효율 배수
    /// </summary>
    public static float fixing = 1; // 적용

    /// <summary>
    /// 포탑 사정거리 배수
    /// </summary>
    public static float turretRange = 1; // 적용

    /// <summary>
    /// 파괴된 건물 복원 시의 자원 소비 배수
    /// </summary>
    public static float destroyedConstructionFixedSpendResource = 1; // 적용

    /// <summary>
    /// 지뢰 공격력 배율
    /// </summary>
    public static float mineDamage = 1; // 적용

    /// <summary>
    /// 회복 시설 범위 배율
    /// </summary>
    public static float healthFacilityRecoveryRange = 1; // 적용

    /// <summary>
    /// 터렛 공격 속도 배율
    /// </summary>
    public static float turretAttackSpeed = 1; // 적용

    /// <summary>
    /// 창고 저장량 배율
    /// </summary>
    public static float capacity = 1; // 적용

    /// <summary>
    /// 거점 방어력 배율
    /// </summary>
    public static float basecampDefence = 1; // 적용

    /// <summary>
    /// 망치 타격 범위 배율
    /// </summary>
    public static float hammerRange = 1; // 적용

    /// <summary>
    /// 거점 자동 수리 유무
    /// </summary>
    public static bool openAutoFix = false; // 적용

    /// <summary>
    /// 구조물 모든 능력치 배수(<- 이거 진짜 악마임)
    /// </summary>
    public static float constructionAbility = 1; // 적용

    /// <summary>
    /// 감염 치료 해금 여부
    /// </summary>
    public static bool infectionTreat = false; // 적용

    /// <summary>
    /// 근접 무기 공격력 배율
    /// </summary>
    public static float meleeDamage = 1; // 적용

    /// <summary>
    /// 공격 속도 배율
    /// </summary>
    public static float attackSpeed = 1; // 적용

    /// <summary>
    /// 재장전 시간 배율
    /// </summary>
    public static float reloadingTime = 1; // 적용

    /// <summary>
    /// 탄속 배율
    /// </summary>
    public static float shotSpeed = 1; // 적용

    /// <summary>
    /// 투척류 사거리 배율
    /// </summary>
    public static float throwRange = 1; // 적용

    /// <summary>
    /// 투척류 쿨타임 배율
    /// </summary>
    public static float throwCoolTime = 1; // 적용

    /// <summary>
    /// 추가 지뢰 설치 수
    /// </summary>
    public static int additionalMineCount = 0; // 적용

    /// <summary>
    /// 포탑 휴대 해금 여부
    /// </summary>
    public static bool isPortableTurret = false; // 적용

    /// <summary>
    /// 파츠 능력치 배율
    /// </summary>
    public static float partsAbility = 1; // 적용

    /// <summary>
    /// 자동 무기 공격력 배율
    /// </summary>
    public static float autoGunDamage = 1; // 적용

    /// <summary>
    /// 탄창 크기 배율
    /// </summary>
    public static float magazineCapacity = 1; // 적용

    /// <summary>
    /// 수류탄 범위 배율
    /// </summary>
    public static float grenadeRange = 1; // 적용

    /// <summary>
    /// 공격 성공 시 이동 속도가 증가할 확률 (0~1)
    /// </summary>
    public static float increaseMoveSpeedProbability = 0; // 적용
    /// <summary>
    /// 이동 속도 배수
    /// </summary>
    public static float moveSpeed = 1; // 적용

    /// <summary>
    /// 콤보 공격 시 공격력 증가율
    /// </summary>
    public static float comboDamageIncrease = 0; // 적용

    /// <summary>
    /// 총기류 사정거리 배율
    /// </summary>
    public static float gunRange = 1; // 적용

    /// <summary>
    /// 무기 공격력 배율
    /// </summary>
    public static float weaponDamage = 1; // 적용

    /// <summary>
    /// 투척류 범위 배율
    /// </summary>
    public static float throwScale = 1; // 적용

    /// <summary>
    /// 무기 관련 모든 능력치 배율(<- 제발)
    /// </summary>
    public static float weaponAbility = 1; // 적용

    /// <summary>
    /// 총기 재장전 스킵 여부
    /// </summary>
    public static bool isRelodingSkip = false; // 적용
    #endregion 

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
            myText.text = "습득함";

            blocker.color = new Color(0, 1, 0, 0.4f);
            blocker.gameObject.SetActive(true);
            blocker.raycastTarget = true;

            effect[skillCode]();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isUnlocked)
        {
            blocker.color = new Color(0, 0, 1, 0.4f);
            blocker.gameObject.SetActive(true);

            myText.text = skillName;
            Notion.ToolTip(skillDescription, true);
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
