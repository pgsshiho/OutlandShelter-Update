using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infecter : BasicZombie
{

    protected override void Attack(Transform target)
    {
        base.Attack(target);
        Collider2D[] hits = Physics2D.OverlapCircleAll(Position, range);

        bool inRange = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.transform == target)
            {
                inRange = true;
                break;
            }
        }
        if (inRange)
        {
            ApplyInfectionEffect(target);
        }
    }
    public void ApplyInfectionEffect(Transform target)
    {
        if(target.TryGetComponent(out PlayerMove playerMove))
        {
            int Infectrandom = Random.Range(0, 10);
            if(Infectrandom > 8) Personal_resource.instance.isInfect = true;
        }
    }
}
