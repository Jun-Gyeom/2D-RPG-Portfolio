using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TresureChest : MonoBehaviour
{
    public Animator anim; // 보물상자 애니메이터

    public GameObject keyUI_F; // F키 UI

    // 등장 아이템 리스트
    public List<ItemData> common_ItemList; // 일반 등급 아이템 리스트
    public List<ItemData> rare_ItemList; // 레어 등급 아이템 리스트
    public List<ItemData> epic_ItemList; // 에픽 등급 아이템 리스트

    public List<ItemData> targetList; // 드롭 할 아이템 리스트

    private bool isOpen; // 상자 개봉 여부

    private void Update()
    {
        // 트리거 발동 중에 상자가 열리지 않았을 때
        if (keyUI_F.activeSelf && !isOpen)
        {
            // F키를 누르면
            if (Input.GetKeyDown(KeyCode.F))
            {
                ChestOpen(); // 상자 오픈
            }
        }
    }

    // 트리거가 처음 발동될 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 트리거를 발동시킨 오브젝트의 태그가 플레이어이고 상자가 열리지 않았을 때
        if (collision.gameObject.CompareTag("Player") && !isOpen)
        {
            keyUI_F.SetActive(true);
        }
    }

    // 트리거의 발동이 꺼질 때
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 트리거를 발동시킨 오브젝트의 태그가 플레이어이거나 상자가 열렸을 때
        if (collision.gameObject.CompareTag("Player") || isOpen)
        {
            keyUI_F.SetActive(false);
        }
    }

    // 상자 오픈 함수
    private void ChestOpen()
    {
        isOpen = true; // 상자 열림 처리

        keyUI_F.SetActive(false);

        anim.SetBool("isOpen", true); // 상자 열기 애니메이션 재생

        float rand_ItemClass = Random.Range(0f, 1f);

        if (rand_ItemClass < 0.6f) // (60%) 
        {
            Debug.Log("일반 등급");
            targetList = common_ItemList; // 타겟 리스트에 일반 등급 대입
        }
        else if (rand_ItemClass < 0.95f) // 레어 (35%) 
        {
            Debug.Log("레어 등급");
            targetList = rare_ItemList; // 타겟 리스트에 레어 등급 대입
        }
        else if (rand_ItemClass < 1.0f) // 에픽 (5%) 
        {
            Debug.Log("에픽 등급");
            targetList = epic_ItemList; // 타겟 리스트에 에픽 등급 대입
        }

        int rand_Item = Random.Range(0, targetList.Count); // 타겟 등급 리스트의 크기보다 작은 난수 구하기

        Debug.Log(rand_Item);

        GameObject dropItem = ObjectPoolingManager.instance.GetObject(targetList[rand_Item].objectPoolName);
        dropItem.transform.position = transform.position; // 위치 변경
    }
}
