using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    Animator anim; // 포탈 애니메이터
    public Portal portal; // 포탈 스크립트

    public GameObject portalObject; // 포탈매니저를 할당할 오브젝트
    public GameObject keyUI_F; // F키 UI
    public GameObject animObj; // 애니메이터가 들어가 있는 오브젝트

    public bool isPortalOpen; // 포탈이 열린 상태인지 체크
    public bool isTrigger; // 포탈 트리거가 발동 중인가 (포탈 근처인가)


    void Update()
    {
        // 할당되지 않은 컴포넌트가 있다면 할당
        if ((keyUI_F == null) || (anim == null) || (portal == null))
        {
            SetComponent();
        }

        // 포탈 열기
        if (isPortalOpen) // 포탈이 열렸다면
        {
            // 포탈 애니메이션 작동
            anim.SetBool("OpenPortal", isPortalOpen);

            // 포탈 위 F버튼 UI
            keyUI_F.SetActive(isTrigger);

            // 포탈 탑승
            if (isTrigger && anim.GetCurrentAnimatorStateInfo(0).IsName("Open")) // 포탈 트리거를 발동 중이고, 포탈 오픈 애니메이션이 실행 중일 때
            {
                // F키를 누르면
                if (Input.GetKeyDown(KeyCode.F))
                {
                    // 게임매니저의 포탈 탑승 함수 실행
                    GameManager.instance.GetIntoPortal();
                }
            }
        }
        else
        {
            // 포탈 애니메이션 작동
            anim.SetBool("OpenPortal", isPortalOpen);
        }
    }

    // 컴포넌트 할당과 몬스터 수 체크
    public void SetComponent()
    {
        portalObject = GameObject.Find("Portal");
        keyUI_F = GameObject.Find("F");
        animObj = GameObject.Find("PortalAnim");

        anim = animObj.GetComponent<Animator>();
        portal = portalObject.GetComponent<Portal>();

        GameManager.instance.StageMonsterCheck(); // 게임매니저의 스테이지 몬스터 체크 함수 실행
    }
}
