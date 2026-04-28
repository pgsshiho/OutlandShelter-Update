using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eater : BasicZombie
{
    public GameObject melten;
    public override void Death()
    {
        if (isDead) return;
        int i = Random.Range(2, 4);
        for (int a = 0; a < i; a++){
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
            
            Instantiate(melten, spawnPos, Quaternion.identity); }
        base.Death();
    }
}
