using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SummonTurretBullet : MonoBehaviour
{
    private Animator anim;
    public IObjectPool<GameObject> pool;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        anim.SetTrigger("Attack");

        StartCoroutine(WaitAction.waitOneFrame(() =>
        {
            StartCoroutine(WaitAction.wait(() => { return !anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"); }, () =>
            {
                pool.Release(gameObject);
            }));
        }));
    }
}
