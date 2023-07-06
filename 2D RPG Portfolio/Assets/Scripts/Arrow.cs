using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Skel_Bow_Monster skel;

    private void Update()
    {
        // 화살과 스켈 궁수의 위치 계산
        float distance = Vector2.Distance(skel.transform.position, transform.position);

        // 화살의 위치가 스켈 궁수 위치랑 사거리보다 더 멀어졌을 때 화살이 삭제되지 않았다면
        if (distance > skel.arrowDistance)
        {
            // 화살 제거
            DestroyArrow();
        }
    }

    // 트리거에 닿았을 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 트리거를 발동시킨 오브젝트의 태그가 플레이어이고 플레이어가 죽지 않았을 때
        if (collision.gameObject.CompareTag("Player") && !GameManager.instance.isDie)
        {
            // 피격
            GameManager.instance.failCause = "스켈레톤 궁수에게 패배"; // 사망 이유

            skel.attackPos = transform;
            collision.GetComponent<PlayerManager>().TakeDamage(skel.attackDamage, skel.attackPos, false); // 마지막 인수 = 몬스터는 크리티컬 공격 없음

            // 화살 제거
            DestroyArrow();
        }
        // 트리거를 발동시킨 오브젝트의 태그가 플랫폼일 때
        else if (collision.gameObject.CompareTag("Platform"))
        {
            // 화살 제거
            DestroyArrow();
        }
    }

    public void DestroyArrow()
    {
        // 이펙트 생성
        GameObject hitEffect = ObjectPoolingManager.instance.GetObject("Effect_ArrowHit"); // 이펙트 오브젝트 풀에서 대여
        hitEffect.transform.position = transform.position; // 위치 변경
        hitEffect.transform.rotation = transform.rotation; // 방향 변경

        skel.arrowHitFXAnim = hitEffect.GetComponent<Animator>();

        // 애니메이션 트리거 활성화
        skel.arrowHitFXAnim.SetTrigger("Hit_Arrow");

        // 화살 오브젝트 풀에 반환
        gameObject.SetActive(false);
    }
}
