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
        if (!isStop) transform.Rotate(new Vector3(0, 0, 720f * Time.deltaTime));
        else transform.eulerAngles = Vector3.zero;
    }

    protected void OnDisable()
    {
        col.enabled = false;
        isStop = false;
    }
}
