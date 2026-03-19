using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;

    Vector3 TargetPos
    {
        get
        {
            return (Vector3)PlayerMove.moveDirection * 0.5f + new Vector3(0, 0, -500);
        }
    }

    [SerializeField] private Vector2 leftBottom;
    [SerializeField] private Vector2 rightTop;

    private void Update()
    {
        Vector3 targetPos = transform.parent.position + TargetPos;

        float size = Camera.main.orthographicSize;

        leftBottom.x += size * (16f / 9f);
        leftBottom.y += size;
        rightTop.x -= size * (16f / 9f);
        rightTop.y -= size;

        targetPos.x = Mathf.Clamp(targetPos.x, leftBottom.x, rightTop.x);
        targetPos.y = Mathf.Clamp(targetPos.y, leftBottom.y, rightTop.y);

        if (targetPos.x > leftBottom.x && targetPos.x < rightTop.x && targetPos.y > leftBottom.y && targetPos.y < rightTop.y) transform.position = Vector3.Lerp(transform.position, targetPos, 10f / 3f * Time.deltaTime);
        else transform.position = targetPos;

        leftBottom.x -= size * (16f / 9f);
        leftBottom.y -= size;
        rightTop.x += size * (16f / 9f);
        rightTop.y += size;
    }

    private void FixedUpdate()
    {
        Vector3 targetPos = transform.parent.position + TargetPos;

        float size = Camera.main.orthographicSize;

        leftBottom.x += size * (16f / 9f);
        leftBottom.y += size;
        rightTop.x -= size * (16f / 9f);
        rightTop.y -= size;

        targetPos.x = Mathf.Clamp(targetPos.x, leftBottom.x, rightTop.x);
        targetPos.y = Mathf.Clamp(targetPos.y, leftBottom.y, rightTop.y);

        if (targetPos.x > leftBottom.x && targetPos.x < rightTop.x && targetPos.y > leftBottom.y && targetPos.y < rightTop.y) transform.position = Vector3.Lerp(transform.position, targetPos, 10f / 3f * Time.deltaTime);
        else transform.position = targetPos;

        leftBottom.x -= size * (16f / 9f);
        leftBottom.y -= size;
        rightTop.x += size * (16f / 9f);
        rightTop.y += size;
    }

    private void LateUpdate()
    {
        Vector3 targetPos = transform.parent.position + TargetPos;

        float size = Camera.main.orthographicSize;

        leftBottom.x += size * (16f / 9f);
        leftBottom.y += size;
        rightTop.x -= size * (16f / 9f);
        rightTop.y -= size;

        targetPos.x = Mathf.Clamp(targetPos.x, leftBottom.x, rightTop.x);
        targetPos.y = Mathf.Clamp(targetPos.y, leftBottom.y, rightTop.y);

        if (targetPos.x > leftBottom.x && targetPos.x < rightTop.x && targetPos.y > leftBottom.y && targetPos.y < rightTop.y) transform.position = Vector3.Lerp(transform.position, targetPos, 10f / 3f * Time.deltaTime);
        else transform.position = targetPos;

        leftBottom.x -= size * (16f / 9f);
        leftBottom.y -= size;
        rightTop.x += size * (16f / 9f);
        rightTop.y += size;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(leftBottom, new Vector3(rightTop.x, leftBottom.y));
        Gizmos.DrawLine(leftBottom, new Vector3(leftBottom.x, rightTop.y));
        Gizmos.DrawLine(rightTop, new Vector3(rightTop.x, leftBottom.y));
        Gizmos.DrawLine(rightTop, new Vector3(leftBottom.x, rightTop.y));
    }
}
