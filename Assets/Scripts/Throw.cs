using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace Weapons
{
    /// <summary>
    /// ����ü���� �����ϴ� Ŭ����
    /// </summary>
    public class Throw : Weapon
    {
        [SerializeField] private float throwForce = 10f;
        [SerializeField] private TextMeshProUGUI throwText;

        public override void Attack()
        {
            if (canAttack && WorkingManager.throwCounts[poolManager.weaponIndex] > 0)
            {
                canAttack = false;
                isAttacking = !canAttack;
                WorkingManager.throwCounts[poolManager.weaponIndex]--;

                StartCoroutine(WaitAction.wait(() => isAttackTimimg, () =>
                {
                    GameObject temp = poolManager.Pool.Get();

                    temp.transform.parent = attackPivot;
                    temp.transform.localPosition = new Vector3(0, distanceBetweenPlayer[poolManager.weaponIndex], 0);
                    temp.transform.localEulerAngles = Vector3.zero;
                    temp.transform.parent = null;
                    temp.GetComponent<Rigidbody2D>().linearVelocity = temp.transform.up * throwForce * TechTreeUnlock.throwRange;

                    if (temp.TryGetComponent(out SummonThrow temp2))
                    {
                        temp2.pool = poolManager.Pool;
                    }

                    isAttacking = false;
                }));

                StartCoroutine(WaitAction.wait(coolTime[poolManager.weaponIndex]
                    / (Personal_resource.hpPercentage <= 20 ? TechTreeUnlock.lowHpAttackSpeed : 1) / TechTreeUnlock.attackSpeed * TechTreeUnlock.throwCoolTime, () =>
                {
                    canAttack = true;
                }));
            }
        }

        private void OnEnable()
        {
            throwText.gameObject.SetActive(true);

            weaponRack.SetActive(true);
        }

        private void OnDisable()
        {
            attackAnimation.Rewind();
            throwText.gameObject.SetActive(false);
        }

        protected override void Awake()
        {
            base.Awake();

            if (!global::Weapon.weaponList.ContainsKey(gameObject)) global::Weapon.weaponList[gameObject] = new List<Weapon>();
            global::Weapon.weaponList[gameObject].Add(this);
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

            throwText.text = $"{WorkingManager.throwCounts[poolManager.weaponIndex]}";
        }

        protected override void Play()
        {
            if (canAttack && WorkingManager.throwCounts[poolManager.weaponIndex] > 0)
            {
                attackAnimation = DOTween.Sequence();
                attackAnimation.Append(grandChild.DOLocalRotate(-new Vector3(0, 0, 30 * (weaponRenderer.flipX ? 1 : -1)), 0.3f))
                    .Append(grandChild.DOLocalRotate(new Vector3(0, 0, 30 * (weaponRenderer.flipX ? 1 : -1)), 0.1f))
                    .AppendCallback(() =>
                    {
                        isAttackTimimg = true;
                        StartCoroutine(WaitAction.waitOneFrame(() => isAttackTimimg = false));
                    })
                    .Append(grandChild.DOLocalRotate(Vector3.zero, 0.3f));
            }
        }
    }
}
