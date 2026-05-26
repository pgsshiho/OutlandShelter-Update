using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    Vector3 TargetPos
    {
        get { return (Vector3)PlayerMove.moveDirection * 0.5f + new Vector3(0, 0, -500); }
    }

    [SerializeField]
    private Vector2 leftBottom;

    [SerializeField]
    private Vector2 rightTop;

    private Camera main;

    private void Awake()
    {
        main = Camera.main;
    }

    private void LateUpdate()
    {
        Vector3 targetPos = player.position + TargetPos;

        float size = main.orthographicSize;

        float leftBottomX = leftBottom.x + size * main.aspect;
        float leftBottomY = leftBottom.y + size;
        float rightTopX = rightTop.x - size * main.aspect;
        float rightTopY = rightTop.y - size;

        targetPos.x = Mathf.Clamp(targetPos.x, leftBottomX, rightTopX);
        targetPos.y = Mathf.Clamp(targetPos.y, leftBottomY, rightTopY);

        Vector3 temp = Vector3.MoveTowards(transform.position, targetPos, 5 * Time.deltaTime);

        temp.x = Mathf.Clamp(temp.x, leftBottomX, rightTopX);
        temp.y = Mathf.Clamp(temp.y, leftBottomY, rightTopY);

        transform.position = temp;
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
