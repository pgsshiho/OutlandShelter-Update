using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tunger : BasicZombie
{
    [SerializeField] private GameObject saliva;
    [SerializeField] private float attackDistance;
    [SerializeField] private float salivaSpeed;

    protected override void Attack(Transform target)
    {
        Vector2 direction = (targetPos - transform.position).normalized;
        
        GameObject temp = Instantiate(saliva, transform.position + (Vector3)(direction * attackDistance), Quaternion.identity);

        if (temp.TryGetComponent(out Saliva sal))
        {
            sal.damage = damage;

            sal.ThrowTo(direction, salivaSpeed);
        }
    }
}
