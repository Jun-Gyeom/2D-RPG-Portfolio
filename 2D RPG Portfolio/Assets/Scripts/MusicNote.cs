using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicNote : MonoBehaviour
{
    Rigidbody2D musicNoteRb; // 음표 리지드바디
    Animator anim; // 음표 애니메이터

    public float speed; // 음표 속도
    public float distance; // 음표 사거리

    public Transform shooterPos; // 음표를 발사한 객체의 위치 값

    public Transform aimPos; // 전달받을 음표 발사 방향 값
    public Transform bulletPos; // 전달받을 음표 발사 위치

    public Transform shotRotation; // 실제로 발사할 방향

    public int damage; // 전달받을 공격력
    public string failCause; // 전달받을 사망 사유

    private bool isHit; // 음표가 터졌는지 체크

    private void Awake()
    {
        // 컴포넌트 할당
        musicNoteRb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 음표를 발사한 객체의 위치 계산
        float dis = Vector2.Distance(shooterPos.position, transform.position);

        // 음표의 위치가 발사한 객체 위치랑 사거리보다 더 멀어졌을 때 음표가 삭제되지 않았다면
        if (dis > distance && !isHit)
        {
            ExplosionFireBall(); // 음표 터짐
        }
    }

    private void OnEnable()
    {
        // 음표 터지지 않음 체크
        isHit = false;

        // 음표 기본 애니메이션
        anim.SetBool("isDefault", true);
    }

    // 음표 위치 설정 함수
    public void Setting()
    {
        // 음표의 위치 설정
        transform.position = bulletPos.position;

        // 음표의 방향 설정
        shotRotation.rotation = aimPos.rotation;
    }

    public void Shot()
    {
        // 음표 발사
        if (shooterPos.localScale.x > 0)
        {
            musicNoteRb.velocity = shotRotation.right * speed; // 음표 이동
        }
        else if (shooterPos.localScale.x < 0)
        {
            musicNoteRb.velocity = shotRotation.right * -speed; // 음표 이동
        }
    }

    // 트리거에 닿았을 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 트리거를 발동시킨 오브젝트의 태그가 플레이어이고 플레이어가 죽지 않았고 음표가 터지지 않았을 때
        if (collision.gameObject.CompareTag("Player") && !GameManager.instance.isDie && !isHit)
        {
            // 피격
            GameManager.instance.failCause = failCause; // 사망 이유

            collision.GetComponent<PlayerManager>().TakeDamage(damage, transform, false); // 마지막 인수 = 몬스터는 크리티컬 공격 없음

            ExplosionFireBall(); // 음표 터짐
        }
    }

    // 음표 터짐 함수
    void ExplosionFireBall()
    {
        // 음표 기본 애니메이션 끄기
        anim.SetBool("isDefault", false);

        // 음표 터짐 애니메이션
        anim.SetTrigger("Hit");

        // 음표 멈춤
        musicNoteRb.velocity = Vector2.zero;

        isHit = true; // 음표 터짐 체크

        // 음표 반환 함수가 이미 호출 되었다면 리턴
        if (IsInvoking("RetrunMusicNote"))
        {
            return;
        }

        Invoke("RetrunMusicNote", 1f); // 1초 후 음표 풀에 반환
    }

    void RetrunMusicNote()
    {
        Debug.Log("음표 반환!");
        gameObject.SetActive(false); // 음표 풀에 반환
    }
}
