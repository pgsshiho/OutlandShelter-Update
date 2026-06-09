using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FirstaidKit : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;

    [Header("치료 설정")]
    public float movingTime = 0.7f; // 5번: 대기 시간
    public int healAmount = 20;     // 6번: 치료량

    public IObjectPool<GameObject> pool;

    private bool isHeld = false;
    private bool isHealing = false;
    private bool canStartHeal = false; // 마우스 연타로 인한 타이밍 버그 방지용

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // 부모의 기능을 완전히 무력화 (슬로우 및 물리 날아감 방지)
        if (col != null) col.enabled = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        isHeld = true;
        isHealing = false;
        canStartHeal = false;

        // 장착하는 순간 무기 발사기 클릭과 겹쳐서 바로 치료가 시작되는 것을 막기 위해 1프레임 대기
        StartCoroutine(EnableClickRoutine());
    }

    private IEnumerator EnableClickRoutine()
    {
        yield return null; // 딱 1프레임 쉬고
        canStartHeal = true; // 이제부터 진짜 사용(4번) 입력을 받음
    }

    private void Update()
    {
        if (isHeld && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // 4번: 손에 잘 들려있고, 치료 중이 아닐 때 "다시 한번 마우스를 클릭하면" 치료 시작!
        if (isHeld && !isHealing && canStartHeal && Input.GetMouseButtonDown(0))
        {
            StartHealing();
        }
    }

    private void StartHealing()
    {
        isHeld = false;
        isHealing = true;

        // 🌟 [추가] 치료 시작 시 플레이어 이동 불가 처리
        PlayerMove.canMove = false;

        Debug.Log("★ 4번 성공: 구급상자 사용 시작! 플레이어가 제자리에 멈춥니다.");

        if (anim != null) anim.SetTrigger("HealProgress");

        // 5번: 사용 시간(movingTime)만큼 얌전히 가만히 기다린 후 정확히 치료(ApplyHeal) 호출
        StartCoroutine(WaitAction.wait(movingTime, ApplyHeal));
    }

    private void ApplyHeal()
    {
        // 6번: 치료 실행
        Personal_resource player = Personal_resource.instance;
        if (player != null)
        {
            player.heal(healAmount);
            Debug.Log($"★ 6번 성공: 치료 완료! 체력 {healAmount} 회복됨.");
        }

        // 🌟 [추가] 치료 완료 후 플레이어 이동 기능 다시 복구
        PlayerMove.canMove = true;

        transform.SetParent(null); // 손에서 해제

        // 사용 완료 후 풀로 정상 반환
        if (pool != null) pool.Release(gameObject);
        else Destroy(gameObject);
    }
}