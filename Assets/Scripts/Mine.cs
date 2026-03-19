using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace Weapons
{
    /// <summary>
    /// 지뢰들을 관장하는 클래스
    /// </summary>
    public class Mine : Weapon
    {
        public static int currentCount = 0;
        [SerializeField] private int maxCount = 5;
        [SerializeField] private TextMeshProUGUI mineCount;

        public override void Attack()
        {
            if (canAttack && currentCount < maxCount + TechTreeUnlock.additionalMineCount && WorkingManager.mineCount > 0)
            {
                canAttack = false;
                isAttacking = !canAttack;

                StartCoroutine(WaitAction.wait(() => isAttackTimimg, () =>
                {
                    WorkingManager.mineCount--;

                    currentCount++;
                    GameObject temp = poolManager.Pool.Get();

                    temp.transform.parent = attackPivot;
                    temp.transform.localPosition = new Vector3(0f, 0f, 0f);
                    temp.transform.parent = null;

                    if (temp.TryGetComponent(out SummonMine mine))
                    {
                        mine.pool = poolManager.Pool;
                    }
                }));

                StartCoroutine(WaitAction.wait(coolTime[poolManager.weaponIndex] / (Personal_resource.hpPercentage <= 20 ? TechTreeUnlock.lowHpAttackSpeed : 1)
                    / TechTreeUnlock.attackSpeed, () =>
                {
                    canAttack = true;
                    isAttacking = !canAttack;
                }));
            }
            else if (canAttack && WorkingManager.mineCount > 0)
            {
                Notion.Warning($"지뢰는 한 번에 {maxCount + TechTreeUnlock.additionalMineCount}개 까지만 설치할 수 있습니다");
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (!global::Weapon.weaponList.ContainsKey(gameObject)) global::Weapon.weaponList[gameObject] = new List<Weapon>();
            global::Weapon.weaponList[gameObject].Add(this);

            currentCount = 0;
        }

        private void OnEnable()
        {
            mineCount.gameObject.SetActive(true);

            weaponRack.SetActive(true);
        }

        private void OnDisable()
        {
            attackAnimation.Rewind();
            mineCount.gameObject.SetActive(false);
        }

        protected override void Update()
        {
            base.Update();

            if (!ItemOwnManager.ownWeapon[kind][poolManager.weaponIndex])
            {
                weaponRack.SetActive(false);
                enabled = false;
            }

            if (attackPivot.eulerAngles.z < 180 && !priDirection)
            {
                weaponRenderer.flipX = true;
            }
            else if (attackPivot.eulerAngles.z > 180 && priDirection)
            {
                weaponRenderer.flipX = false;
            }

            priDirection = weaponRenderer.flipX;

            mineCount.text = $"{WorkingManager.mineCount}";
        }

        protected override void Play()
        {
            if (canAttack && WorkingManager.mineCount > 0)
            {
                attackAnimation = DOTween.Sequence();
                attackAnimation.Append(grandChild.DOLocalRotate(-new Vector3(0, 0, 30 * (weaponRenderer.flipX ? 1 : -1)), coolTime[poolManager.weaponIndex] / 5f))
                    .Append(grandChild.DOLocalRotate(new Vector3(0, 0, 30 * (weaponRenderer.flipX ? 1 : -1)), coolTime[poolManager.weaponIndex] / 5f))
                    .AppendCallback(() =>
                    {
                        isAttackTimimg = true;
                        StartCoroutine(WaitAction.waitOneFrame(() => isAttackTimimg = false));
                    })
                    .Append(grandChild.DOLocalRotate(Vector3.zero, coolTime[poolManager.weaponIndex] / 3f));
            }
        }
    }
}
