using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    PortalManager pm;
    private GameObject pmObject; // 포탈매니저를 할당할 오브젝트
    public int portal_ID; // 어떤 포탈인지 나타내는 ID

    private void Awake()
    {
        // 포탈매니저 할당
        pmObject = GameObject.Find("PortalManager");
        pm = pmObject.GetComponent<PortalManager>();

        // 변수 초기화
        pm.isTrigger = false;
    }
    // 트리거가 처음 발동될 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 트리거를 발동시킨 오브젝트의 태그가 플레이어일 때
        if (collision.gameObject.CompareTag("Player"))
        {
            // 트리거 발동
            pm.isTrigger = true;
        }
    }

    // 트리거의 발동이 꺼질 때
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 트리거를 발동시킨 오브젝트의 태그가 플레이어일 때
        if (collision.gameObject.CompareTag("Player"))
        {
            // 트리거 꺼짐
            pm.isTrigger = false;
        }
    }
}
