using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaticleDelete : MonoBehaviour
{
    ParticleSystem particle;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();

        StartCoroutine(WaitAction.waitOneFrame(() =>
        {
            StartCoroutine(WaitAction.wait(() => { return particle.particleCount == 0; }, () =>
            {
                
                Destroy(gameObject);
            }));
        }));
    }
}
