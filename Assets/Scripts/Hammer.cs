using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Weapons
{
    public class Hammer : Weapon
    {
        public override void Attack()
        {
            if (canAttack)
            {
                canAttack = false;
                isAttacking = !canAttack;

                StartCoroutine(WaitAction.wait(() => isAttackTimimg, () =>
                {
                    SoundManager.SFX.PlayOneShot(SFXReference.Instance.hammer);
                    GameObject temp = poolManager.Pool.Get();

                    temp.transform.parent = attackPivot;
                    temp.transform.localPosition = new Vector3(0, distanceBetweenPlayer[poolManager.weaponIndex], 0);
                    temp.transform.localEulerAngles = Vector3.zero;
                    temp.transform.parent = null;
                    if (temp.TryGetComponent(out SummonHammer hammer) && hammer.pr == null)
                    {
                        hammer.pr = GetComponent<Personal_resource>();
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
                StartCoroutine(WaitAction.wait(coolTime[poolManager.weaponIndex] / (Personal_resource.hpPercentage <= 20 ? TechTreeUnlock.lowHpAttackSpeed : 1), () =>
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
                // 이미 실행 중인 애니메이션이 있다면 멈추고 파괴
                if (attackAnimation != null && attackAnimation.IsActive())
                {
                    attackAnimation.Kill();
                }

                // 방향 결정 (실시간)
                if (attackPivot.eulerAngles.z < 180 && priDirection)
                {
                    weaponRenderer.flipX = false;
                }
                else if (attackPivot.eulerAngles.z > 180 && !priDirection)
                {
                    weaponRenderer.flipX = true;
                }
                priDirection = weaponRenderer.flipX;

                // 무기 거치대 활성화
                weaponRack.SetActive(true);

                // 회전 각도 계산 (실시간 계산)
                float angle = 45f * (weaponRenderer.flipX ? 1 : -1);
                float duration = coolTime[poolManager.weaponIndex] / 5f;

                // 시퀀스 동적 생성
                attackAnimation = DOTween.Sequence();

                attackAnimation.Append(grandChild.DOLocalRotate(new Vector3(0, 0, angle), duration))
                               .Append(grandChild.DOLocalRotate(new Vector3(0, 0, -angle), duration))
                               .AppendCallback(() =>
                               {
                                   isAttackTimimg = true;
                                   StartCoroutine(WaitAction.waitOneFrame(() => isAttackTimimg = false));
                               })
                               .Append(grandChild.DOLocalRotate(Vector3.zero, duration).SetEase(Ease.InOutElastic))
                               .OnComplete(() => weaponRack.SetActive(false));
            }
        }
    }
}
