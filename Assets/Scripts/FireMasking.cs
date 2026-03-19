using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMasking : MonoBehaviour
{
    [SerializeField] private float doTime = 0.2f;

    private void Awake()
    {
        transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), doTime);
    }
}
