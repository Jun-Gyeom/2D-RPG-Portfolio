using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData item;

    public Inventory inventory;

    Rigidbody2D rb; // 아이템의 리지드바디
    BoxCollider2D bc; // 아이템의 트리거인 박스 콜라이더 

    private void Awake()
    {
        // 컴포넌트 할당
        inventory = FindObjectOfType<Inventory>();

        rb = gameObject.GetComponent<Rigidbody2D>();
        bc = gameObject.GetComponent<BoxCollider2D>();
    }

    // 오브젝트가 활성화 되었을 때 호출
    private void OnEnable()
    {
        StartCoroutine(TriggerDelayTime()); // 트리거 딜레이 타임 코루틴을 실행해서 아이템이 생성되자마자 획득하는 것을 방지

        // 튕겨나갈 힘에 사용할 랜덤 값
        float randomForce_Y = Random.Range(5f, 7f);

        // 랜덤한 방향으로 튕겨나감
        Vector2 dropForce = new Vector2(0, randomForce_Y);
        rb.AddForce(dropForce, ForceMode2D.Impulse);
    }

    // 트리거에 닿았을 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 트리거를 발동시킨 오브젝트의 태그가 플레이어이고 플레이어가 죽지 않았을 때
        if (collision.gameObject.CompareTag("Player") && !GameManager.instance.isDie)
        {
            inventory.AcquireItem(gameObject);

            bc.enabled = false; // 다시 트리거 비활성화
        }
    }

    // 아이템이 생성되자마자 획득되는 일을 방지하는 딜레이 코루틴
    IEnumerator TriggerDelayTime()
    {
        yield return new WaitForSeconds(0.5f);
        bc.enabled = true;
    }
}
