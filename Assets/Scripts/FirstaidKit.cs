using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstaidKit : SummonThrow
{
    private bool isHeld = true;
    private bool isHealing = false;

    protected override void OnEnable()
    {
        // [중요] 부모의 OnEnable을 타면 타이머가 꼬이므로 base.OnEnable()을 절대 호출하지 않습니다.
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        col.enabled = false;
        isStop = false;
        isHeld = true;
        isHealing = false;

        rb.linearVelocity = Vector2.zero;

        // 플레이어 손 위치에 장착
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Transform holdPoint = player.transform.Find("HoldPosition");
            if (holdPoint != null)
            {
                transform.SetParent(holdPoint);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
        }
    }

    // 부모인 SummonThrow의 Update(회전 로직 등)를 덮어씌워 완전히 무시합니다.
    protected new void Update()
    {
        // 들고 있고, 치료 중이 아닐 때 클릭하면 치료 시작
        if (isHeld && !isHealing && Input.GetMouseButtonDown(0))
        {
            StartHealing();
        }
    }

    private void StartHealing()
    {
        isHeld = false;
        isHealing = true;

        Debug.Log("★ 치료 시작 (로그 찍히는지 확인하세요!)");

        // 애니메이션이 있다면 실행
        if (anim != null) anim.SetTrigger("HealProgress");

        // movingTime 초 만큼 가만히 대기한 후 정확히 Skill()을 호출합니다.
        StartCoroutine(WaitAction.wait(movingTime, Skill));
    }

    protected override void Skill()
    {
        // 대기 시간이 끝났으므로 치료 로직을 실행합니다.
        ApplyHeal();
    }

    private void ApplyHeal()
    {
        Personal_resource player = Personal_resource.instance;
        if (player != null)
        {
            player.heal(20);
            Debug.Log("★ 치료 완료! 체력 20 회복됨");
        }
        else
        {
            Debug.LogError("Personal_resource 싱글톤 인스턴스를 찾을 수 없습니다!");
        }

        // 사용이 끝났으므로 부모 풀로 안전하게 반환
        if (pool != null) pool.Release(gameObject);
        else Destroy(gameObject);
    }

    // =================================================================
    // [핵심] 부모 클래스들(SummonObject)의 슬로우 및 공격 간섭을 원천 차단
    // =================================================================
    protected override void OnTriggerEnter2D(Collider2D other) { }
    protected override void Attack(IEnemyDamage enemy, Vector2 direction) { }
}