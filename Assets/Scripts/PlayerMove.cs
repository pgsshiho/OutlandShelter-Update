using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer grandChild;
    private Animator anim;
    private Animator Anim
    {
        get
        {
            if (anim.runtimeAnimatorController != controller[(int)gender]) anim.runtimeAnimatorController = controller[(int)gender];
            return anim;
        }
        set
        {
            anim = value;
        }
    }
    public RuntimeAnimatorController[] controller;

    public enum Gender
    {
        Man = 0,
        Woman = 1
    }

    public Gender gender;

    [SerializeField] private float moveSpeed = 7f;
    public static Vector2 moveDirection;
    public static bool canMove = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        grandChild = transform.Find("AttackPivot").GetChild(0).GetComponent<SpriteRenderer>();
        Anim = GetComponent<Animator>();
        canMove = true;
        moveDirection = Vector2.zero;
        gender = (MainmenuManager.isMan ? Gender.Man : Gender.Woman);
    }

    private void Start()
    {
        GetComponent<ChangeWeapon>().indexes[(int)Kind.Melee] = (int)gender;
    }

    private void FixedUpdate()
    {
        if (!(Guide.isEnable || UIOpen.isEnable.ContainsValue(true)))
        {
            bool isEquipedArmor = ItemOwnManager.ownWeapon[Kind.Armor].Contains(true);

            if (!SceneChanger.isFading && !Personal_resource.isDead) moveDirection = canMove && !Personal_resource.isStiffen ? new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized : Vector2.zero;
            if (!PlayerAvoidSkill.isDash)
                rb.linearVelocity = moveDirection * (moveSpeed * (TechTreeUnlock.duringAttackingMoveSpeedFixed || !Weapons.Weapon.isAttacking ? 1 : 0.7f)
                    * TechTreeUnlock.playerMoveSpeed * BasicZombie.increaseSpeed * TechTreeUnlock.moveSpeed
                    * (isEquipedArmor ? Armor.armorStats[ObjectPoolManager.instance[Kind.Armor].weaponIndex].speed : 1));

            spriteRenderer.flipX = moveDirection.x <= 0 && (moveDirection.x < 0 || spriteRenderer.flipX);
            Anim.SetBool("IsMove", moveDirection != Vector2.zero);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            Anim.SetBool("IsMove", false);
        }
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 1000f);
    }

    private void OnDestroy()
    {
        Weapon.weaponList.Remove(gameObject);
    }
}
