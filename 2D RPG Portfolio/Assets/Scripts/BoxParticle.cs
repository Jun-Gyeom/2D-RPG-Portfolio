using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxParticle : MonoBehaviour
{
    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        StartCoroutine("DestroyBoxPiece");

        // 튕겨나갈 힘에 사용할 랜덤 값
        float randomForce_X = Random.Range(-7f, 7f);
        float randomForce_Y = Random.Range(6f, 8f);

        // 랜덤한 방향으로 튕겨나감
        Vector2 dropForce = new Vector2(randomForce_X, randomForce_Y);
        rb.AddForce(dropForce, ForceMode2D.Impulse);
    }

    IEnumerator DestroyBoxPiece()
    {
        yield return new WaitForSeconds(5f); // 5초 후
        gameObject.SetActive(false); // 오브젝트 풀에 반환
    }
}
