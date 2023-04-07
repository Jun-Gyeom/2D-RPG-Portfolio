using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoDownPlatform : MonoBehaviour
{
    PlatformEffector2D pe; // 플랫폼 이펙터 2D

    private bool isGoDown; // 현재 플랫폼 내려가기를 진행 중인지 체크
    private bool canGoDown; // 플랫폼에서 내려갈 수 있는 상황(플랫폼에 닿아있는 상황)인지 체크

    void Start()
    {
        pe = GetComponent<PlatformEffector2D>();
    }

    void Update()
    {
        // 플레이어가 죽었다면 Return
        if (GameManager.instance.isDie)
        {
            return;
        }

        // S키를 누른 상태로 스페이스 바를 누르면 (단, 현재 플랫폼 내려가기를 진행중이지 않고, 플랫폼에 닿아있다면)
        if (Input.GetKey(KeyCode.S) && (Input.GetKeyDown(KeyCode.Space) && !isGoDown && canGoDown))
        {
            // 플랫폼 내려가기 코루틴 실행
            StartCoroutine(GoDownPlatformCoroutine());
        }
    }

    // 플랫폼 내려가기 코루틴
    IEnumerator GoDownPlatformCoroutine()
    {
        isGoDown = true; // 플랫폼 내려가기를 진행 중
        pe.rotationalOffset = 180f; // 단방향 콜라이더 방향 아래쪽으로 변경
        yield return new WaitForSeconds(0.5f); // 0.5초 대기
        pe.rotationalOffset = 0f; // 단방향 콜라이더 방향 위쪽으로 변경
        isGoDown = false; // 플랫폼 내려가기 진행 끝남
    }
    
    // 콜라이더가 닿았을 때 호출
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 플랫폼에서 내려갈 수 있는 상황
        canGoDown = true;
    }

    // 콜라이더가 떨어졌을 때 호출
    private void OnCollisionExit2D(Collision2D collision)
    {
        // 플랫폼에서 내려갈 수 없는 상황
        canGoDown = false;
    }
}
