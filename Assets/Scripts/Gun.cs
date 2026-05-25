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

        //[SerializeField]
        //private float[] deviation;

        //[SerializeField]
        //private int[] oneMagazine;

        //[SerializeField]
        //private float[] relodingTime;

        //[SerializeField]
        //private int shotSpeed = 20;

        private readonly Dictionary<SO_Gun, int> loadedBullet = new();

        private Rigidbody2D rb;
        public bool IsMoving => rb.linearVelocity != Vector2.zero;

        private bool isReloding = false;

        [SerializeField]
        private TextMeshProUGUI bullet;

        [SerializeField]
        private GameObject loadingGuide;

        //[Header("위치 보정 (X는 좌우, Height는 높이)")]
        //[SerializeField]
        //private float fireXOffset = 0f; // X축 미세조정용

        //[SerializeField]
        //private float characterHandHeight = 0.7f;

        public override void Attack()
        {
            int index = poolManager.weaponIndex;
            if (!(canAttack && !isReloding && loadedBullet[CurrentGun] > 0))
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

            //// 일반 총 (권총, 소총 등)
            //if (poolManager.weaponIndex != 4 && poolManager.weaponIndex != 5)
            //{
            //    SoundManager.SFX.PlayOneShot(SFXReference.Instance.gun);
            //    GameObject temp = poolManager.Pool.Get();
            //    Camera.main.transform.DOComplete(); // 이전 흔들림 캔슬로 딜레이 방지
            //    Camera.main.transform.DOShakePosition(0.1f, 0.1f, 10, 90, false, true);
            //    if (temp.TryGetComponent(out SummonBullet summonBullet))
            //    {
            //        summonBullet.pool = poolManager.Pool;
            //        summonBullet.isAuto = poolManager.weaponIndex == 3;
            //    }

            //    float currentDeviation =
            //        this.deviation[poolManager.weaponIndex]
            //        * (
            //            TechTreeUnlock.duringMovingAccuracyFixed
            //            || rb.linearVelocity == Vector2.zero
            //                ? 1
            //                : 1.2f
            //        );

            //    temp.transform.parent = attackPivot;

            //    // 수정: X축 오프셋 반영 (기본값 0)
            //    temp.transform.localPosition = new Vector3(
            //        fireXOffset,
            //        characterHandHeight + distanceBetweenPlayer[poolManager.weaponIndex],
            //        0
            //    );

            //    temp.transform.localEulerAngles = new Vector3(
            //        0,
            //        0,
            //        Random.Range(
            //            -currentDeviation
            //                / GunStatManager.instance[(GunKind)poolManager.weaponIndex].accuracy,
            //            currentDeviation
            //                / GunStatManager.instance[(GunKind)poolManager.weaponIndex].accuracy
            //        )
            //    );
            //    temp.transform.parent = null;

            //    temp.GetComponent<Rigidbody2D>().linearVelocity =
            //        shotSpeed * TechTreeUnlock.shotSpeed * temp.transform.up;

            //    temp.TryGetComponent(out SummonObject temp2);
            //    temp2.StartCoroutine(
            //        WaitAction.wait(
            //            7f * TechTreeUnlock.gunRange,
            //            () =>
            //            {
            //                poolManager.Pool.Release(temp);
            //            }
            //        )
            //    );
            //}
            //else if (poolManager.weaponIndex == 6)
            //{
            //    SoundManager.SFX.PlayOneShot(SFXReference.Instance.rpgShot); // RPG 발사음

            //    GameObject temp = poolManager.Pool.Get();
            //    temp.transform.parent = attackPivot;
            //    Camera.main.transform.DOComplete(); // 이전 흔들림 캔슬로 딜레이 방지
            //    Camera.main.transform.DOShakePosition(0.2f, 0.3f, 15, 90, false, true);
            //    // 위치 설정 (기존 총구 오프셋 활용)
            //    temp.transform.localPosition = new Vector3(
            //        fireXOffset,
            //        characterHandHeight + distanceBetweenPlayer[poolManager.weaponIndex],
            //        0
            //    );
            //    temp.transform.localEulerAngles = Vector3.zero;
            //    temp.transform.parent = null;

            //    // RPG 탄두 속도 및 물리 설정
            //    Rigidbody2D rpgRb = temp.GetComponent<Rigidbody2D>();
            //    rpgRb.linearVelocity = shotSpeed * 0.5f * temp.transform.up; // RPG는 보통 탄속이 느림

            //    // RPG 전용 스크립트가 있다면 데이터 전달
            //    if (temp.TryGetComponent(out SummonRPG rpgScript))
            //    {
            //        rpgScript.pool = poolManager.Pool;
            //    }
            //    loadedBullet[poolManager.weaponIndex] = 0;
            //    canAttack = false;

            //    // 무기 소유권 박탈 (내려놓지도 못하게 하려면 인벤토리에서 제거)
            //    ItemOwnManager.ownWeapon[Kind.Gun][6] = false;

            //    // 알림 메시지
            //    Notion.Log("The RPG has been used and is no longer available.");
            //}
            //// 샷건 및 오토 샷건 (4, 5번)
            //else
            //{
            //    SoundManager.SFX.PlayOneShot(SFXReference.Instance.shotgun);
            //    PlayerMove.canMove = false;

            //    GameObject temp = poolManager.Pool.Get();
            //    temp.transform.parent = attackPivot;
            //    Camera.main.transform.DOComplete(); // 이전 흔들림 캔슬로 딜레이 방지
            //    Camera.main.transform.DOShakePosition(0.4f, 0.6f, 20, 90, false, true);
            //    temp.transform.localScale =
            //        GunStatManager.instance[(GunKind)poolManager.weaponIndex].range
            //        * TechTreeUnlock.gunRange
            //        * poolManager.summonPrefab[poolManager.weaponIndex].transform.localScale;

            //    // 수정: 샷건도 X축 오프셋 반영 및 Y축 거리 고정
            //    float shotgunForwardOffset = 0.1f;
            //    temp.transform.localPosition = new Vector3(
            //        fireXOffset,
            //        characterHandHeight
            //            + distanceBetweenPlayer[poolManager.weaponIndex]
            //            + shotgunForwardOffset,
            //        0
            //    );

            //    temp.transform.localEulerAngles = Vector3.zero;
            //    temp.transform.parent = null;

            //    if (temp.TryGetComponent(out SummonShotGunEffect summonShotGunEffect))
            //    {
            //        summonShotGunEffect.isAuto = poolManager.weaponIndex == 5;
            //    }

            //    StartCoroutine(
            //        WaitAction.waitOneFrame(() =>
            //        {
            //            if (temp.TryGetComponent(out Animator anim))
            //            {
            //                StartCoroutine(
            //                    WaitAction.wait(
            //                        () =>
            //                        {
            //                            return !anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
            //                        },
            //                        () =>
            //                        {
            //                            poolManager.Pool.Release(temp);
            //                        }
            //                    )
            //                );
            //            }
            //        })
            //    );

            //    StartCoroutine(
            //        WaitAction.wait(
            //            (
            //                coolTime[poolManager.weaponIndex]
            //                / GunStatManager.instance[(GunKind)poolManager.weaponIndex].attackSpeed
            //            )
            //                / TechTreeUnlock.attackSpeed
            //                / 2,
            //            () =>
            //            {
            //                PlayerMove.canMove = true;
            //            }
            //        )
            //    );
            //}

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
                //if (index != 4)
                //{
                //    yield return new WaitForSeconds(time * TechTreeUnlock.reloadingTime);

                //    // 인벤토리 탄약 결정
                //    int currentInv = gun[index].UsingBullet;

                //    int reloadCount = Mathf.Clamp(capacity - loadedBullet[index], 0, currentInv);

                //    // 인벤토리에서 차감
                //    gun[index].UsingBullet -= reloadCount;

                //    SoundManager.SFX.PlayOneShot(SFXReference.Instance.reload);
                //    loadedBullet[index] += reloadCount;
                //}
                //else
                //{
                //    int temp = Mathf.Clamp(
                //        capacity - loadedBullet[index],
                //        0,
                //        BulletManager.shotGunBullet
                //    );
                //    for (int i = loadedBullet[index]; i <= temp; i++)
                //    {
                //        loadedBullet[index] = i;
                //        BulletManager.shotGunBullet--;
                //        SoundManager.SFX.PlayOneShot(SFXReference.Instance.reload);
                //        yield return new WaitForSeconds(
                //            time
                //                * TechTreeUnlock.reloadingTime
                //                * GunStatManager.instance[GunKind.Shotgun].reloadingTime
                //                / temp
                //        );
                //    }
                //}
            }
            isReloding = false;
        }

        private void OnEnable()
        {
            bullet.gameObject.SetActive(true);
            loadingGuide.SetActive(true);
            weaponRack.SetActive(true);

            StartCoroutine(Reloding());
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
