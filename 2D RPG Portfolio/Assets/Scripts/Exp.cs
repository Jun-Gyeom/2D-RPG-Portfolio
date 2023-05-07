using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp : MonoBehaviour
{
    Rigidbody2D rb; // 경험치의 리지드바디
    CircleCollider2D cc; // 경험치의 트리거인 사이클 콜라이더 

    public Transform hudPos; // 허드텍스트 생성 위치

    void Awake()
    {
        // 컴포넌트 할당
        rb = gameObject.GetComponent<Rigidbody2D>();
        cc = gameObject.GetComponent<CircleCollider2D>();
    }

    // 오브젝트가 활성화 되었을 때 호출
    private void OnEnable()
    {
        StartCoroutine(TriggerDelayTime()); // 트리거 딜레이 타임 코루틴을 실행해서 경험치가 생성되자마자 획득하는 것을 방지

        // 튕겨나갈 힘에 사용할 랜덤 값
        float randomForce_X = Random.Range(-7f, 7f);
        float randomForce_Y = Random.Range(4f, 6f);

        // 랜덤한 방향으로 튕겨나감
        Vector2 dropForce = new Vector2(randomForce_X, randomForce_Y);
        rb.AddForce(dropForce, ForceMode2D.Impulse);
    }

    // 트리거에 닿았을 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 트리거를 발동시킨 오브젝트의 태그가 플레이어이고 플레이어가 죽지 않았을 때
        if (collision.gameObject.CompareTag("Player") && !GameManager.instance.isDie)
        {
            GameManager.instance.exp += 10f; // 경험치 10 획득

            // 허드 텍스트 생성
            GameObject getExp_HudText = ObjectPoolingManager.instance.GetObject("HudText_GetExp");
            getExp_HudText.transform.position = hudPos.position; // 허드 텍스트 위치 변경
            getExp_HudText.transform.GetChild(0).GetComponent<GetExp_HudText>().ShowGetExpText(10); // 허드 텍스트가 표시 할 경험치량 전달 (그림자)
            getExp_HudText.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<GetExpHudText_Shadow>().ShowGetExpText(10); // 허드 텍스트가 표시 할 경험치량 전달 (텍스트)

            cc.enabled = false; // 다시 트리거 비활성화

            gameObject.SetActive(false); // 오브젝트 풀에 오브젝트 반환
        }
    }

    // 경험치가 생성되자마자 획득되는 일을 방지하는 딜레이 코루틴
    IEnumerator TriggerDelayTime()
    {
        yield return new WaitForSeconds(0.5f);
        cc.enabled = true;
    }
}
