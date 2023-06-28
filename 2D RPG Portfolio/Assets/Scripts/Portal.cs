using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public int portal_ID; // 어떤 포탈인지 나타내는 ID

    public Animator anim; // 포탈 애니메이터
    public GameObject keyUI_F; // F키 UI

    public bool isPortalOpen; // 포탈이 열린 상태인지 체크
    public bool isTrigger; // 포탈 트리거가 발동 중인가 (포탈 근처인가)

    private void Awake()
    {
        // 변수 초기화
        isTrigger = false;
    }

    private void Update()
    {
        // 포탈 열기
        if (isPortalOpen) // 포탈이 열렸다면
        {
            // 포탈 애니메이션 작동
            anim.SetBool("OpenPortal", isPortalOpen);

            // 포탈 위 F버튼 UI
            keyUI_F.SetActive(isTrigger);

            // 포탈 탑승
            if (isTrigger && anim.GetCurrentAnimatorStateInfo(0).IsName("Open") && !GameManager.instance.dontMove) // 포탈 트리거를 발동 중이고, 포탈 오픈 애니메이션이 실행 중이고 움직임 제어 중이 아닐 때
            {
                // F키를 누르면
                if (Input.GetKeyDown(KeyCode.F))
                {
                    // 게임매니저의 포탈 탑승 함수 실행
                    GameManager.instance.GetIntoPortal();

                    keyUI_F.SetActive(false); // 포탈 위 UI 끄기
                }
            }
        }
        else
        {
            // 포탈 애니메이션 작동
            anim.SetBool("OpenPortal", isPortalOpen);
        }
    }

    // 트리거가 처음 발동될 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 트리거를 발동시킨 오브젝트의 태그가 플레이어일 때
        if (collision.gameObject.CompareTag("Player"))
        {
            // 트리거 발동
            isTrigger = true;
        }
    }

    // 트리거의 발동이 꺼질 때
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 트리거를 발동시킨 오브젝트의 태그가 플레이어일 때
        if (collision.gameObject.CompareTag("Player"))
        {
            // 트리거 꺼짐
            isTrigger = false;
        }
    }
}
