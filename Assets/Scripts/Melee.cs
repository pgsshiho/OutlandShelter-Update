using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Weapons
{
    /// <summary>
    /// 근접무기를 관장하는 클래스
    /// </summary>
    public class Melee : Weapon
    {
        // 칼 추가 후 소환 타이밍 조정 예정
        public override void Attack()
        {
            if (canAttack)
            {
                canAttack = false;
                isAttacking = !canAttack;

                StartCoroutine(WaitAction.wait(() => isAttackTimimg, () =>
                {
                    SoundManager.SFX.PlayOneShot(SFXReference.Instance.melee);
                    GameObject temp = poolManager.Pool.Get();

                    temp.transform.parent = attackPivot;
                    temp.transform.localPosition = new Vector3(0, distanceBetweenPlayer[poolManager.weaponIndex], 0);
                    temp.transform.localEulerAngles = Vector3.zero;
                    temp.transform.parent = null;

                    if (temp.TryGetComponent(out SpriteRenderer spriteRenderer))
                    {
                        spriteRenderer.flipX = weaponRenderer.flipX;
                    }

                    StartCoroutine(WaitAction.waitOneFrame(() =>
                    {
                        if (temp.TryGetComponent<Animator>(out Animator anim))
                        {
                            StartCoroutine(WaitAction.wait(() => !anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"), () =>
                            {
                                poolManager.Pool.Release(temp);
                            }));
                        }
                    }));
                }));
                StartCoroutine(WaitAction.wait(coolTime[poolManager.weaponIndex] / (Personal_resource.hpPercentage <= 20 ? TechTreeUnlock.lowHpAttackSpeed : 1)
                    / TechTreeUnlock.attackSpeed, () =>
                {
                    canAttack = true;
                    isAttacking = !canAttack;
                }));
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (!global::Weapon.weaponList.ContainsKey(gameObject)) global::Weapon.weaponList[gameObject] = new List<Weapon>();

            global::Weapon.weaponList[gameObject].Add(this);
        }

        private void OnEnable()
        {
            weaponRack.SetActive(false);
        }

        private void OnDisable()
        {
            attackAnimation?.Rewind();
        }

        protected override void Play()
        {
            if (canAttack)
            {
                if (attackPivot.eulerAngles.z < 180 && priDirection)
                {
                    weaponRenderer.flipX = false;
                }
                else if (attackPivot.eulerAngles.z > 180 && !priDirection)
                {
                    weaponRenderer.flipX = true;
                }

                priDirection = weaponRenderer.flipX;

                grandChild.Rotate(new Vector3(0, 0, 100 * (weaponRenderer.flipX ? 1 : -1)));

                weaponRack.SetActive(true);

                isAttackTimimg = true;

                StartCoroutine(WaitAction.waitOneFrame(() => isAttackTimimg = false));

                attackAnimation = DOTween.Sequence();
                attackAnimation
                    .Append(grandChild.DOLocalRotate(-new Vector3(0, 0, 70 * (weaponRenderer.flipX ? 1 : -1)), coolTime[poolManager.weaponIndex] / 3f))
                    .AppendCallback(() =>
                    {
                        StartCoroutine(WaitAction.wait(coolTime[poolManager.weaponIndex] / 5f, () =>
                        {
                            grandChild.localEulerAngles = Vector3.zero;
                            weaponRack.SetActive(false);
                            if (poolManager.weaponIndex == 1) weaponRenderer.flipX = !weaponRenderer.flipX;
                        }));
                    });
            }
        }
    }
}
