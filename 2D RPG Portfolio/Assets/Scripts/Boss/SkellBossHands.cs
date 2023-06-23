using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkellBossHands : MonoBehaviour
{
    public Animator anim; // 손 애니메이터

    public SkellBossLaser laser; // 레이저 스크립트

    private Transform targetTransform; // 플레이어 위치 

    public float moveSpeed; // 손 이동속도
    public float handPosY; // 손이 이동할 y위치
    private void Start()
    {
        // 플레이어 위치 할당
        targetTransform = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        MoveHand();
    }
    public void MoveHand()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector2(transform.position.x, handPosY), moveSpeed);
    }

    public void SetHandPos()
    {
        // 플레이어 y위치에 따른 손 이동 위치
        if (targetTransform.position.y > 11f) // 가장 위
        {
            handPosY = 12f;
        }
        else if (targetTransform.position.y > 9f)
        {
            handPosY = 10f;
        }
        else if (targetTransform.position.y > 7f)
        {
            handPosY = 8f;
        }
        else if (targetTransform.position.y > 5f)
        {
            handPosY = 6f;
        }
        else if (targetTransform.position.y > 3f)
        {
            handPosY = 4f;
        }
        else if (targetTransform.position.y > 1f)
        {
            handPosY = 2f;
        }
        else // 가장 아래
        {
            handPosY = 0f;
        }
    }
}
