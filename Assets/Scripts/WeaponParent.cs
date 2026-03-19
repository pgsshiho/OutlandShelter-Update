using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        public abstract void Attack();
        protected abstract void Play();
        protected Sequence attackAnimation;
        public float[] coolTime;
        protected bool canAttack = true;
        protected Transform attackPivot;
        public Kind kind;
        public float[] distanceBetweenPlayer;

        public static bool isAttacking = false;
        protected static bool isAttackTimimg = false;

        public bool[] isFlip;

        public Vector2[] offset;
        public Sprite[] weapons;

        [HideInInspector] public ObjectPoolManager poolManager;
        protected Transform grandChild;

        protected GameObject weaponRack;
        protected SpriteRenderer weaponRenderer;
        protected bool priDirection = false;

        protected virtual void Awake()
        {
            attackPivot = transform.Find("AttackPivot");
            if (!global::Weapon.weaponList.ContainsKey(gameObject)) global::Weapon.weaponList[gameObject] = new List<Weapon>();
            global::Weapon.weaponList[gameObject].Add(this);

            isAttacking = false;
            isAttackTimimg = false;

            grandChild = attackPivot.GetChild(0);

            weaponRack = grandChild.GetChild(0).gameObject;
            weaponRenderer = weaponRack.GetComponent<SpriteRenderer>();
            weaponRack.SetActive(false);
        }

        protected void Start()
        {
            poolManager = ObjectPoolManager.instance[kind];
        }

        protected virtual void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Play();
                Attack();
            }
        }
    }
}
