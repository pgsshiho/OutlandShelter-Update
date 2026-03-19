using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSeed : MonoBehaviour
{
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.SetTrigger("Boom");

        StartCoroutine(WaitAction.waitOneFrame(() =>
        {
            StartCoroutine(WaitAction.wait(() => { return !anim.GetCurrentAnimatorStateInfo(0).IsTag("Boom"); }, () =>
            {
                gameObject.SetActive(false);
            }));
        }));
    }
}
