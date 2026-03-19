using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
    }

    private void LateUpdate()
    {
        transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
    }

    private void FixedUpdate()
    {
        transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
    }
}
