using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    Rigidbody2D fireBallRb; // 파이어볼 리지드바디
    Animator anim; // 파이어볼 애니메이터

    public float speed; // 파이어볼 속도
    public float distance; // 파이어볼 사거리

    public Transform shooterPos; // 파이어볼을 발사한 객체의 위치 값

    public Transform aimPos; // 전달받을 파이어볼 발사 방향 값
    public Transform bulletPos; // 전달받을 파이어볼 발사 위치

    public int damage; // 전달받을 공격력
    public string failCause; // 전달받을 사망 사유

    private bool isHit; // 파이어볼이 터졌는지 체크

    private void Awake()
    {
        // 컴포넌트 할당
        fireBallRb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 파이어볼을 발사한 객체의 위치 계산
        float dis = Vector2.Distance(shooterPos.position, transform.position);

        // 파이어볼의 위치가 발사한 객체 위치랑 사거리보다 더 멀어졌을 때 파이어볼이 삭제되지 않았다면
        if (dis > distance && !isHit)
        {
            ExplosionFireBall(); // 파이어볼 터짐
        }
    }

    private void OnEnable()
    {
        // 파이어볼 터지지 않음 체크
        isHit = false;

        // 파이어볼 기본 애니메이션
        anim.SetBool("isDefault", true);
    }

    // 파이어볼 위치, 방향 설정 함수
    public void Setting()
    {
        // 파이어볼의 위치 설정
        transform.position = bulletPos.position;

        // 파이어볼의 방향 설정
        if (shooterPos.localScale.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, aimPos.rotation.eulerAngles.z - 90);
        }
        else if (shooterPos.localScale.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, aimPos.rotation.eulerAngles.z + 90);
        }
    }

    public void Shot()
    {
        // 파이어볼 발사
        if (shooterPos.localScale.x > 0)
        {
            fireBallRb.velocity = aimPos.right * speed; // 파이어볼 이동
        }
        else if (shooterPos.localScale.x < 0)
        {
            fireBallRb.velocity = aimPos.right * -speed; // 파이어볼 이동
        }
    }

    // 트리거에 닿았을 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 트리거를 발동시킨 오브젝트의 태그가 플레이어이고 플레이어가 죽지 않았고 파이어볼이 터지지 않았을 때
        if (collision.gameObject.CompareTag("Player") && !GameManager.instance.isDie && !isHit)
        {
            // 피격
            GameManager.instance.failCause = failCause; // 사망 이유

            collision.GetComponent<PlayerManager>().TakeDamage(damage, transform, false); // 마지막 인수 = 몬스터는 크리티컬 공격 없음

            ExplosionFireBall(); // 파이어볼 터짐
        }
        // 트리거를 발동시킨 오브젝트의 태그가 플랫폼일 때
        else if (collision.gameObject.CompareTag("Platform"))
        {
            ExplosionFireBall(); // 파이어볼 터짐
        }
    }

    // 파이어볼 터짐 함수
    void ExplosionFireBall()
    {
        // 파이어볼 기본 애니메이션 끄기
        anim.SetBool("isDefault", false);

        // 파이어볼 터짐 애니메이션
        anim.SetTrigger("Hit");

        // 파이어볼 멈춤
        fireBallRb.velocity = Vector2.zero;

        isHit = true; // 파이어볼 터짐 체크

        // 파이어볼 반환 함수가 이미 호출 되었다면 리턴
        if (IsInvoking("RetrunFireBall"))
        {
            return;
        }

        Invoke("RetrunFireBall", 1f); // 1초 후 파이어볼 풀에 반환
    }

    void RetrunFireBall()
    {
        Debug.Log("파이어볼 반환!");
        gameObject.SetActive(false); // 파이어볼 풀에 반환
    }
}
