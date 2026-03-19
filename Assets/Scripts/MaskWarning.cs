using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskWarning : MonoBehaviour
{
    [SerializeField] private float doTime = 0.3f;

    private void Awake()
    {
        StartCoroutine(Warning(Vector3.one));
    }

    IEnumerator Warning(Vector3 endValue)
    {
        while (true)
        {
            transform.localScale = Vector3.zero;

            var tweenerCore = transform.DOScale(endValue, doTime);

            yield return tweenerCore.WaitForCompletion();
        }
    }
}
