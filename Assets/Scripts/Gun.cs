using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Weapons
{
    public class Gun : Weapon
    {
        [Header("Gun Setting")]
        [SerializeField]
        private SO_Gun[] gun;
        private SO_Gun CurrentGun => gun[poolManager.weaponIndex];

        private readonly Dictionary<SO_Gun, int> loadedBullet = new();

        private Rigidbody2D rb;
        public bool IsMoving => rb.linearVelocity != Vector2.zero;

        private bool isReloding = false;

        [SerializeField]
        private TextMeshProUGUI bullet;

        [SerializeField]
        private GameObject loadingGuide;

        public override void Attack()
        {
            int index = poolManager.weaponIndex;
            if (!(canAttack && !isReloding))
                return;

            if (loadedBullet[CurrentGun] == 0)
            {
                if (!TechTreeUnlock.isRelodingSkip)
                    return;
                else if (CurrentGun.UsingBullet == 0)
                    return;
            }

            canAttack = false;
            isAttacking = !canAttack;

            // 탄약 감소 로직
            if (!TechTreeUnlock.isRelodingSkip)
                loadedBullet[CurrentGun]--;
            else
                CurrentGun.UsingBullet--;

            CurrentGun.Shoot(this);

            StartCoroutine(
                WaitAction.wait(
                    coolTime[poolManager.weaponIndex]
                        / GunStatManager.instance[(GunKind)poolManager.weaponIndex].attackSpeed
                        / (
                            Personal_resource.hpPercentage <= 20
                                ? TechTreeUnlock.lowHpAttackSpeed
                                : 1
                        )
                        / TechTreeUnlock.attackSpeed,
                    () =>
                    {
                        canAttack = true;
                        isAttacking = !canAttack;
                    }
                )
            );
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.R) && !TechTreeUnlock.isRelodingSkip && !isReloding)
            {
                StartCoroutine(Reloding());
            }

            // 탄약 UI 업데이트
            if (!TechTreeUnlock.isRelodingSkip)
                bullet.text = $"{loadedBullet[CurrentGun]}/{CurrentGun.UsingBullet}";
            else
                bullet.text = CurrentGun.UsingBullet.ToString();

            if (AttackPivot.eulerAngles.z < 180 && !priDirection)
                weaponRenderer.flipX = true;
            else if (AttackPivot.eulerAngles.z > 180 && priDirection)
                weaponRenderer.flipX = false;

            priDirection = weaponRenderer.flipX;
        }

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody2D>();

            if (!global::Weapon.weaponList.ContainsKey(gameObject))
                global::Weapon.weaponList[gameObject] = new List<Weapon>();
            global::Weapon.weaponList[gameObject].Add(this);

            for (int i = 0; i < gun.Length; i++)
            {
                loadedBullet[gun[i]] = 0;
            }

            StartCoroutine(
                WaitAction.wait(
                    () => TechTreeUnlock.isRelodingSkip,
                    () =>
                    {
                        int index = poolManager.weaponIndex;

                        CurrentGun.UsingBullet += loadedBullet[CurrentGun];

                        loadedBullet[CurrentGun] = 0;
                    }
                )
            );
        }

        IEnumerator Reloding()
        {
            int index = poolManager.weaponIndex;
            float time = gun[index].relodingTime;
            isReloding = true;

            int capacity = Mathf.FloorToInt(
                gun[index].oneMagazine * TechTreeUnlock.magazineCapacity
            );
            if (capacity - loadedBullet[CurrentGun] > 0)
            {
                // index 4(샷건)가 아닌 경우들 (권총, 소총, RPG 등)
                yield return StartCoroutine(
                    CurrentGun.Reload(
                        this,
                        () => loadedBullet[CurrentGun],
                        x => loadedBullet[CurrentGun] = x,
                        capacity,
                        time
                    )
                );
            }
            isReloding = false;
        }

        private void OnEnable()
        {
            bullet.gameObject.SetActive(true);
            loadingGuide.SetActive(true);
            weaponRack.SetActive(true);
        }

        private void OnDisable()
        {
            attackAnimation?.Rewind();
            bullet.gameObject.SetActive(false);
            loadingGuide.SetActive(false);
        }

        private void OnDestroy()
        {
            foreach (SO_Gun gun in this.gun)
                if (gun)
                    gun.Clear();
        }

        protected override void Play()
        {
            if (canAttack && !isReloding && loadedBullet[CurrentGun] > 0)
            {
                attackAnimation = DOTween.Sequence();
                attackAnimation
                    .Append(
                        grandChild.DOLocalMove(
                            (weaponRenderer.flipX ? 1 : -1)
                                * coolTime[poolManager.weaponIndex]
                                * new Vector3(0.5f, -0.3f),
                            coolTime[poolManager.weaponIndex] / 3f
                        )
                    )
                    .Join(
                        grandChild.DOLocalRotate(
                            -new Vector3(
                                0,
                                0,
                                (30 * coolTime[poolManager.weaponIndex])
                                    * (weaponRenderer.flipX ? 1 : -1)
                            ),
                            coolTime[poolManager.weaponIndex] / 3f
                        )
                    )
                    .Append(
                        grandChild.DOLocalMove(Vector3.zero, coolTime[poolManager.weaponIndex] / 3f)
                    )
                    .Join(
                        grandChild.DOLocalRotate(
                            Vector3.zero,
                            coolTime[poolManager.weaponIndex] / 3f
                        )
                    );
            }
        }
    }
}
