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

    [SerializeField] public float moveSpeed = 7f;

    // 외부(GasCloud 등)에서 접근할 수 있도록 프로퍼티 추가
    public float BaseMoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    // 🌟 상태이상 및 디버프로 인한 속도 배율 (기본값 1, 슬로우 시 0.5 등으로 감소)
    public float CurrentSpeedMultiplier { get; set; } = 1f;

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

            if (!SceneChanger.isFading && !Personal_resource.isDead)
                moveDirection = canMove && !Personal_resource.isStiffen ? new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized : Vector2.zero;

            if (!PlayerAvoidSkill.isDash)
            {
                // 기본 무기 공격 시 감속 비율 계산
                float attackSpeedFactor = TechTreeUnlock.duringAttackingMoveSpeedFixed || !Weapons.Weapon.isAttacking ? 1f : 0.7f;

                // 아머 장착 시 속도 계산
                float armorSpeedFactor = isEquipedArmor ? Armor.armorStats[ObjectPoolManager.instance[Kind.Armor].weaponIndex].speed : 1f;

                // 🌟 전체 최종 속도 계산 (수식을 분리하여 가독성 확보 + CurrentSpeedMultiplier 반영)
                float finalSpeed = moveSpeed
                                   * attackSpeedFactor
                                   * TechTreeUnlock.playerMoveSpeed
                                   * BasicZombie.increaseSpeed
                                   * TechTreeUnlock.moveSpeed
                                   * armorSpeedFactor
                                   * CurrentSpeedMultiplier; // <- 가스 장판 등의 슬로우 디버프가 여기에 계산됨

                rb.linearVelocity = moveDirection * finalSpeed;
            }

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
        // 2D Y-Sorting
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 1000f);
    }

    private void OnDestroy()
    {
        Weapon.weaponList.Remove(gameObject);
    }
    private void OnEnable()
    {
        WorkingManager.OnShoesCrafted += OnSpeedBoostTriggered;
    }

    private void OnDisable()
    {
        WorkingManager.OnShoesCrafted -= OnSpeedBoostTriggered;
    }

    // IShoesSpeed 인터페이스 구현
    public void OnSpeedBoostTriggered(int percentage)
    {
        // 내 속도는 내가 계산한다!
        moveSpeed *= (1f + (percentage / 100f));
    }
}