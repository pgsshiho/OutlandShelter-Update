using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Weapons
{
    /// <summary>
    /// �� ���⸦ �����ϴ� Ŭ����
    /// </summary>
    public class Gun : Weapon
    {
        public float[] deviation; // +- �� ���� ����
        public int[] oneMagazine; // źâ �ϳ� �뷮
        private int[] loadedBullet; // ������ �Ѿ�
        public float[] relodingTime; // ������ �ð�
        public int shotSpeed = 20;
        private Rigidbody2D rb;
        private bool isReloding = false;
        [SerializeField] private TextMeshProUGUI bullet;
        [SerializeField] private GameObject loadingGuide;

        public override void Attack()
        {
            if (canAttack && !isReloding && loadedBullet[poolManager.weaponIndex] > 0)
            {
                canAttack = false;
                isAttacking = !canAttack;
                if (!TechTreeUnlock.isRelodingSkip) loadedBullet[poolManager.weaponIndex]--;
                else
                {
                    if (poolManager.weaponIndex == 0 || poolManager.weaponIndex == 1)
                    {
                        BulletManager.pistolBullet--;
                    }
                    else if (poolManager.weaponIndex == 2 || poolManager.weaponIndex == 3)
                    {
                        BulletManager.rifleBullet--;
                    }
                    else
                    {
                        BulletManager.shotGunBullet--;
                    }
                }

                if (poolManager.weaponIndex != 4 && poolManager.weaponIndex != 5)
                {
                    SoundManager.SFX.PlayOneShot(SFXReference.Instance.gun);
                    GameObject temp = poolManager.Pool.Get();

                    if (temp.TryGetComponent(out SummonBullet summonBullet))
                    {
                        summonBullet.pool = poolManager.Pool;
                        summonBullet.isAuto = poolManager.weaponIndex == 3;
                    }

                    float deviation = this.deviation[poolManager.weaponIndex] * (TechTreeUnlock.duringMovingAccuracyFixed || rb.linearVelocity == Vector2.zero ? 1 : 1.2f);

                    temp.transform.parent = attackPivot;
                    temp.transform.localPosition = new Vector3(0, distanceBetweenPlayer[poolManager.weaponIndex], 0);
                    temp.transform.localEulerAngles = new Vector3(0, 0, Random.Range(-deviation / GunStatManager.instance[(GunKind)poolManager.weaponIndex].accuracy, deviation / GunStatManager.instance[(GunKind)poolManager.weaponIndex].accuracy));
                    temp.transform.parent = null;

                    temp.GetComponent<Rigidbody2D>().linearVelocity = shotSpeed * TechTreeUnlock.shotSpeed * temp.transform.up;

                    temp.TryGetComponent(out SummonObject temp2);

                    temp2.StartCoroutine(WaitAction.wait(7f * TechTreeUnlock.gunRange, () =>
                    {
                        poolManager.Pool.Release(temp);
                    }));
                }
                else
                {
                    SoundManager.SFX.PlayOneShot(SFXReference.Instance.shotgun);
                    PlayerMove.canMove = false;

                    GameObject temp = poolManager.Pool.Get();

                    temp.transform.parent = attackPivot;
                    temp.transform.localScale = GunStatManager.instance[(GunKind)poolManager.weaponIndex].range * TechTreeUnlock.gunRange
                        * poolManager.summonPrefab[poolManager.weaponIndex].transform.localScale;
                    temp.transform.localPosition = new Vector3(0, distanceBetweenPlayer[poolManager.weaponIndex] * (temp.transform.localScale.y / 2f), 0);
                    temp.transform.localEulerAngles = Vector3.zero;
                    temp.transform.parent = null;

                    if (temp.TryGetComponent(out SummonShotGunEffect summonShotGunEffect))
                    {
                        summonShotGunEffect.isAuto = poolManager.weaponIndex == 5;
                    }

                    StartCoroutine(WaitAction.waitOneFrame(() =>
                    {
                        if (temp.TryGetComponent(out Animator anim))
                        {
                            StartCoroutine(WaitAction.wait(() => { return !anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"); }, () =>
                            {
                                poolManager.Pool.Release(temp);
                            }));
                        }
                    }));
                    StartCoroutine(WaitAction.wait((coolTime[poolManager.weaponIndex] / GunStatManager.instance[(GunKind)poolManager.weaponIndex].attackSpeed)
                         / TechTreeUnlock.attackSpeed / 2, () =>
                    {
                        PlayerMove.canMove = true;
                    }));
                }

                StartCoroutine(WaitAction.wait(coolTime[poolManager.weaponIndex] / GunStatManager.instance[(GunKind)poolManager.weaponIndex].attackSpeed
                    / (Personal_resource.hpPercentage <= 20 ? TechTreeUnlock.lowHpAttackSpeed : 1) / TechTreeUnlock.attackSpeed, () =>
                {
                    canAttack = true;
                    isAttacking = !canAttack;
                }));
            }
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.R) && !TechTreeUnlock.isRelodingSkip && !isReloding)
            {
                StartCoroutine(Reloding());
            }

            if (!TechTreeUnlock.isRelodingSkip) bullet.text = $"{loadedBullet[poolManager.weaponIndex]}/{(poolManager.weaponIndex == 0 || poolManager.weaponIndex == 1 ? BulletManager.pistolBullet : poolManager.weaponIndex == 2 || poolManager.weaponIndex == 3 ? BulletManager.rifleBullet : BulletManager.shotGunBullet)}";
            else bullet.text = $"{(poolManager.weaponIndex == 0 || poolManager.weaponIndex == 1 ? BulletManager.pistolBullet : poolManager.weaponIndex == 2 || poolManager.weaponIndex == 3 ? BulletManager.rifleBullet : BulletManager.shotGunBullet)}";
            
            if (attackPivot.eulerAngles.z < 180 && !priDirection)
            {
                weaponRenderer.flipX = true;
            }
            else if (attackPivot.eulerAngles.z > 180 && priDirection)
            {
                weaponRenderer.flipX = false;
            }

            priDirection = weaponRenderer.flipX;
        }

        protected override void Awake()
        {
            base.Awake();

            rb = GetComponent<Rigidbody2D>();
            loadedBullet = new int[oneMagazine.Length];

            if (!global::Weapon.weaponList.ContainsKey(gameObject)) global::Weapon.weaponList[gameObject] = new List<Weapon>();
            global::Weapon.weaponList[gameObject].Add(this);

            StartCoroutine(WaitAction.wait(() => TechTreeUnlock.isRelodingSkip, () =>
            {
                int temp = loadedBullet[poolManager.weaponIndex];

                if (poolManager.weaponIndex == 0 || poolManager.weaponIndex == 1)
                {
                    BulletManager.pistolBullet += temp;
                }
                else if (poolManager.weaponIndex == 2 || poolManager.weaponIndex == 3)
                {
                    BulletManager.rifleBullet += temp;
                }
                else
                {
                    BulletManager.shotGunBullet += temp;
                }

                loadedBullet[poolManager.weaponIndex] -= temp;
            }));
        }

        IEnumerator Reloding()
        {
            int index = poolManager.weaponIndex;
            float relodingTime = this.relodingTime[index];

            isReloding = true;

            if (oneMagazine[index] - loadedBullet[index] > 0)
            {
                if (index != 4)
                {
                    yield return new WaitForSeconds(relodingTime * TechTreeUnlock.reloadingTime);
                    int reloadBulletCount;

                    if (index == 0 || index == 1)
                    {
                        reloadBulletCount = Mathf.Clamp(Mathf.FloorToInt(oneMagazine[index] * TechTreeUnlock.magazineCapacity) - loadedBullet[index],
                            0, BulletManager.pistolBullet);

                        BulletManager.pistolBullet -= reloadBulletCount;
                    }
                    else if (index == 2 || index == 3)
                    {
                        reloadBulletCount = Mathf.Clamp(Mathf.FloorToInt(oneMagazine[index] * TechTreeUnlock.magazineCapacity) - loadedBullet[index],
                            0, BulletManager.rifleBullet);

                        BulletManager.rifleBullet -= reloadBulletCount;
                    }
                    else
                    {
                        reloadBulletCount = Mathf.Clamp(Mathf.FloorToInt(oneMagazine[index] * TechTreeUnlock.magazineCapacity) - loadedBullet[index],
                            0, BulletManager.shotGunBullet);

                        BulletManager.shotGunBullet -= reloadBulletCount;
                    }

                    SoundManager.SFX.PlayOneShot(SFXReference.Instance.reload);
                    loadedBullet[index] += reloadBulletCount;
                }
                else
                {
                    int temp = Mathf.Clamp(Mathf.FloorToInt(oneMagazine[index] * TechTreeUnlock.magazineCapacity) - loadedBullet[index], 0, BulletManager.shotGunBullet);

                    for (int i = loadedBullet[index]; i <= temp; i++)
                    {
                        loadedBullet[index] = i;
                        BulletManager.shotGunBullet--;
                        SoundManager.SFX.PlayOneShot(SFXReference.Instance.reload);
                        yield return new WaitForSeconds(relodingTime * TechTreeUnlock.reloadingTime * GunStatManager.instance[GunKind.Shotgun].reloadingTime / temp);
                    }
                }
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

        protected override void Play()
        {
            if (canAttack && !isReloding && loadedBullet[poolManager.weaponIndex] > 0)
            {
                attackAnimation = DOTween.Sequence();
                attackAnimation.Append(grandChild.DOLocalMove((weaponRenderer.flipX ? 1 : -1) * coolTime[poolManager.weaponIndex] * new Vector3(0.5f, -0.3f), coolTime[poolManager.weaponIndex] / 3f))
                    .Join(grandChild.DOLocalRotate(-new Vector3(0, 0, (30 * coolTime[poolManager.weaponIndex]) * (weaponRenderer.flipX ? 1 : -1)), coolTime[poolManager.weaponIndex] / 3f))
                    .Append(grandChild.DOLocalMove(Vector3.zero, coolTime[poolManager.weaponIndex] / 3f))
                    .Join(grandChild.DOLocalRotate(Vector3.zero, coolTime[poolManager.weaponIndex] / 3f));
            }
        }
    }
}
