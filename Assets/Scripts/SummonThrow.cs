using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class SummonThrow : SummonObject
{
    protected Rigidbody2D rb;
    protected Collider2D col;
    public float movingTime = 0.7f;
    public IObjectPool<GameObject> pool;
    protected bool isStop = false;
    protected Animator anim;

    // [추가] 생성되자마자 대기 상태로 들어갈 것인지 체크하는 변수
    protected bool isWaitingForClick = false; 

    protected override void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    protected override void OnEnable()
    {
        col.enabled = false;
        isStop = false;

        // [수정] 대기 상태가 아닐 때만 기존처럼 바로 타이머(날아가기)를 시작합니다.
        if (!isWaitingForClick)
        {
            StartMoving();
        }
    }

    // [추가] 실제로 날아가기 시작하는 로직을 분리
    protected void StartMoving()
    {
        isWaitingForClick = false;
        StartCoroutine(WaitAction.wait(movingTime, Skill));
    }

    protected virtual void Skill()
    {
        isStop = true;
        rb.linearVelocity = Vector3.zero;
        col.enabled = true;
    }

    protected void Update()
    {
        // [수정] 대기 상태가 아니고, 멈추지 않았을 때만 회전합니다.
        if (!isWaitingForClick && !isStop) 
            transform.Rotate(new Vector3(0, 0, 720f * Time.deltaTime));
        else if (isStop) 
            transform.eulerAngles = Vector3.zero;
    }

    protected void OnDisable()
    {
        col.enabled = false;
        isStop = false;
        isWaitingForClick = false; // 리셋
    }
}