using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkellBossLaser : MonoBehaviour
{
    public Animator[] anims; // 레이저 애니메이션 배열
    public bool isAttack; // 공격 중인지 체크

    public Transform attackPos; // 전달할 공격한 위치
    public int attackDamage; // 공격력

    public CapsuleCollider2D hitCollider; // 피격 판정 콜라이더


    // 트리거에 접촉 중일 때 호출
    private void OnTriggerStay2D(Collider2D player)
    {
        // 공격 중일 때
        if (isAttack)
        {
            // 태그가 플레이어라면
            if (player.gameObject.tag == "Player")
            {
                GameManager.instance.failCause = "델리알에게 패배"; // 사망 이유

                attackPos = transform;
                player.gameObject.GetComponent<PlayerManager>().TakeDamage(attackDamage, attackPos, false); // 마지막 인수 = 몬스터는 크리티컬 공격 없음

                isAttack = false; // 공격 상태 해제
            }
        }
    }

    // 레이저 발사 함수
    public void LaserShot()
    {
        isAttack = true; // 현재 공격 중임

        hitCollider.enabled = true; // 피격 판정 활성화

        SoundManager.instance.PlaySound("LaserShot"); // 사운드 재생

        // 모든 레이저 발사 애니메이션 재생
        for (int i = 0; i < anims.Length; i++)
        {            
            anims[i].SetBool("isShot", true);
        }

        Invoke("LaserOff", 0.75f);
    }

    // 레이저 발사 종료 함수
    public void LaserOff()
    {
        // 모든 레이저 꺼짐 애니메이션 재생
        for (int i = 0; i < anims.Length; i++)
        {
            anims[i].SetBool("isShot", false);
        }

        Invoke("AttackOff", 0.25f);
    }

    // 공격 상태 해제 함수
    public void AttackOff()
    {
        isAttack = false; // 현재 공격 중 아님

        hitCollider.enabled = false; // 피격 판정 비활성화
    }
}
