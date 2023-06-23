using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fairy : MonoBehaviour
{
    public enum FairySize
    {
        Size_S,
        Size_M,
        Size_L,
        Size_XL,
    }

    CircleCollider2D cc; // 페어리의 트리거인 사이클 콜라이더 
    public Transform hudPos; // 허드텍스트 생성 위치

    PlayerManager pm; // 플레이어 매니저

    public FairySize fairySize; // 페어리 사이즈

    private int getHealthAmount; // HP 획득량

    private void Awake()
    {
        // 컴포넌트 할당
        cc = gameObject.GetComponent<CircleCollider2D>();
    }

    // 오브젝트가 활성화 되었을 때 호출
    private void OnEnable()
    {
        StartCoroutine(TriggerDelayTime()); // 트리거 딜레이 타임 코루틴을 실행해서 페어리가 생성되자마자 획득하는 것을 방지

        pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>(); // 플레이어 매니저 할당

        // 사이즈에 따른 HP 획득량 설정
        switch (fairySize)
        {
            // 사이즈 S
            case FairySize.Size_S:
                getHealthAmount = 5; // HP 획득량 5로 설정
                break;
            // 사이즈 M
            case FairySize.Size_M:
                getHealthAmount = 15; // HP 획득량 15로 설정
                break;
            // 사이즈 L
            case FairySize.Size_L:
                getHealthAmount = 25; // HP 획득량 25로 설정
                break;
            // 사이즈 XL
            case FairySize.Size_XL:
                getHealthAmount = 50; // HP 획득량 50으로 설정
                break;
        }
    }

    // 트리거에 닿았을 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 트리거를 발동시킨 오브젝트의 태그가 플레이어이고 플레이어가 죽지 않았을 때
        if (collision.gameObject.CompareTag("Player") && !GameManager.instance.isDie)
        {
            pm.health += getHealthAmount; // HP 획득량 만큼 체력 얻음

            // 허드 텍스트 생성
            GameObject getHp_HudText = ObjectPoolingManager.instance.GetObject("HudText_GetHp");
            getHp_HudText.transform.position = hudPos.position; // 허드 텍스트 위치 변경
            getHp_HudText.transform.GetChild(0).GetComponent<GetHealth_HudText>().ShowGetHealthText(getHealthAmount); // 허드 텍스트가 표시 할 체력 회복량 전달 (그림자)
            getHp_HudText.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<GetHealthHudText_Shadow>().ShowGetHealthText(getHealthAmount); // 허드 텍스트가 표시 할 체력 회복량 전달 (텍스트)

            // 획득 사운드 재생
            SoundManager.instance.PlaySound("GetFairy");

            cc.enabled = false; // 다시 트리거 비활성화
            gameObject.SetActive(false); // 오브젝트 풀에 오브젝트 반환
        }
    }

    // 페어리가 생성되자마자 획득되는 일을 방지하는 딜레이 코루틴
    IEnumerator TriggerDelayTime()
    {
        yield return new WaitForSeconds(0.5f);
        cc.enabled = true;
    }
}
