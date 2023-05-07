using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkelHead : MonoBehaviour
{
    Rigidbody2D rb; // 스켈 머리의 리지드바디
    SpriteRenderer sr; // 스켈 머리의 스프라이트 렌더러
    void Awake()
    {
        // 컴포넌트 할당
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    // 오브젝트가 활성화 되었을 때 호출
    private void OnEnable()
    {
        Invoke("DestroyHead", 5f); // 5초 후 오브젝트 반환 처리

        // 튕겨나갈 힘에 사용할 랜덤 값
        float randomForce_X = Random.Range(-5f, 5f);
        float randomForce_Y = Random.Range(4f, 5f);

        // 랜덤한 방향으로 튕겨나감
        Vector2 dropForce = new Vector2(randomForce_X, randomForce_Y);
        rb.AddForce(dropForce, ForceMode2D.Impulse);

        // 튕겨나간 방향에 따른 스프라이트 플립
        if (randomForce_X < 0)
        {
            sr.flipX = true;
        }
    }

    private void DestroyHead()
    {
        gameObject.SetActive(false); // 오브젝트 풀에 반환
    }
}
