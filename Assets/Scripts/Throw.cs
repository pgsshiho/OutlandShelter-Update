using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        [SerializeField]
        private float throwForce = 10f;

        [SerializeField]
        private TextMeshProUGUI throwText;

        public override void Attack()
        {
            // 장착된 무기가 있고 개수가 있을 때
            if (canAttack && WorkingManager.throwCounts[poolManager.weaponIndex] > 0)
            {
                canAttack = false;
                isAttacking = !canAttack;
                WorkingManager.throwCounts[poolManager.weaponIndex]--;

                // ★ [중요] 3번 인덱스가 구급상자라면 애니메이션 코루틴을 절대 타지 않고 즉시 소환!
                if (poolManager.weaponIndex == 3)
                {
                    GameObject temp = poolManager.Pool.Get();

                    // 소환 즉시 플레이어 손 위치에 본드로 붙여버림
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                    {
                        Transform holdPoint = player.transform.Find("HoldPosition");
                        if (holdPoint != null)
                        {
                            temp.transform.parent = holdPoint;
                            temp.transform.localPosition = Vector3.zero;
                            temp.transform.localEulerAngles = Vector3.zero;
                        }
                    }

                    // 물리적으로 절대 날아가지 않게 잠금
                    Rigidbody2D rb = temp.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector2.zero;
                        rb.bodyType = RigidbodyType2D.Kinematic;
                    }

                    isAttacking = false;
                }
                else // 일반 수류탄 등은 원래대로 던지는 타이밍을 기다림
                {
                    StartCoroutine(
                        WaitAction.wait(
                            () => isAttackTimimg,
                            () =>
                            {
                                GameObject temp = poolManager.Pool.Get();
                                temp.transform.parent = AttackPivot;
                                temp.transform.localPosition = new Vector3(0, distanceBetweenPlayer[poolManager.weaponIndex], 0);
                                temp.transform.localEulerAngles = Vector3.zero;
                                temp.transform.parent = null;

                                temp.GetComponent<Rigidbody2D>().linearVelocity =
                                    temp.transform.up * throwForce * TechTreeUnlock.throwRange;

                                if (temp.TryGetComponent(out SummonThrow temp2))
                                {
                                    temp2.pool = poolManager.Pool;
                                }
                                isAttacking = false;
                            }
                        )
                    );
                }

                // 쿨타임 처리는 공통
                StartCoroutine(
                    WaitAction.wait(
                        coolTime[poolManager.weaponIndex] / (Personal_resource.hpPercentage <= 20 ? TechTreeUnlock.lowHpAttackSpeed : 1) / TechTreeUnlock.attackSpeed * TechTreeUnlock.throwCoolTime,
                        () => { canAttack = true; }
                    )
                );
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

            if (!global::Weapon.weaponList.ContainsKey(gameObject))
                global::Weapon.weaponList[gameObject] = new List<Weapon>();
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

            if (AttackPivot.eulerAngles.z < 180 && !priDirection)
            {
                weaponRenderer.flipX = true;
            }
            else if (AttackPivot.eulerAngles.z > 180 && priDirection)
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
                attackAnimation
                    .Append(
                        grandChild.DOLocalRotate(
                            -new Vector3(0, 0, 30 * (weaponRenderer.flipX ? 1 : -1)),
                            0.3f
                        )
                    )
                    .Append(
                        grandChild.DOLocalRotate(
                            new Vector3(0, 0, 30 * (weaponRenderer.flipX ? 1 : -1)),
                            0.1f
                        )
                    )
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
