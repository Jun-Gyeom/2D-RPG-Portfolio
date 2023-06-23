using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSword : MonoBehaviour
{
    Rigidbody2D BossSwordRB; // 검 리지드바디

    public Animator anim; // 검 애니메이터
    public Animator chargeAnim; // 검 충전 효과 애니메이터
    public Animator createAnim; // 검 생성 효과 애니메이터
    public Animator hitAnim; // 검 충돌 효과 애니메이터

    public float speed; // 검 속도
    public float distance; // 검 사거리

    public Transform shooterPos; // 검을 발사한 객체의 위치 값

    public Transform aimPos; // 전달받을 검 발사 방향 값
    public Transform bulletPos; // 전달받을 검 발사 위치

    public Transform targetTransform; // 전달받을 플레이어 위치 값

    public int damage; // 전달받을 공격력
    public string failCause; // 전달받을 사망 사유

    private bool isShot; // 검을 발사했는지 체크
    private bool isHit; // 검이 터졌는지 체크
    private bool canHit; // 플레이어를 공격할 수 있는 상태인지 체크

    GameObject bossSwordHitFX; // 검 충돌 애니메이션을 재생할 오브젝트

    public Transform headTransform; // 검의 머리 부분 위치

    RaycastHit2D hitInfo; // 레이 충돌 정보

    private void Awake()
    {
        // 컴포넌트 할당
        BossSwordRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 검을 발사한 객체의 위치 계산
        float dis = Vector2.Distance(shooterPos.position, transform.position);

        // 검의 위치가 발사한 객체 위치랑 사거리보다 더 멀어졌을 때 검이 삭제되지 않았다면
        if (dis > distance && !isHit)
        {
            ExplosionBossSword(); // 검 터짐
        }

        AimPlayer(); // 플레이어 조준

        Debug.DrawRay(bulletPos.position, (headTransform.position - transform.position) * 10f, Color.red);
    }

    private void OnEnable()
    {
        // 검 발사하지 않음 체크
        isShot = false;

        // 검 터지지 않음 체크
        isHit = false;

        // 검 기본 애니메이션
        anim.SetBool("isShot", false);
    }

    // 검 위치 설정 함수
    public void Setting()
    {
        // 검 위치 설정
        transform.position = bulletPos.position;
    }

    public void Shot()
    {
        // 검 발사
        if (shooterPos.localScale.x > 0)
        {
            BossSwordRB.velocity = aimPos.up * speed; // 검 이동
        }
        else if (shooterPos.localScale.x < 0)
        {
            BossSwordRB.velocity = aimPos.up * -speed; // 검 이동
        }

        isShot = true; // 발사 체크
        canHit = true; // 발사를 했으므로 공격 가능
    }

    // 트리거에 닿았을 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 트리거를 발동시킨 오브젝트의 태그가 플레이어이고 플레이어가 죽지 않았고 검이 터지지 않았고 공격 가능할 때
        if (collision.gameObject.CompareTag("Player") && !GameManager.instance.isDie && !isHit && canHit)
        {
            // 피격
            GameManager.instance.failCause = failCause; // 사망 이유

            canHit = false; // 한번 피격 당하면 공격 능력 상실

            collision.GetComponent<PlayerManager>().TakeDamage(damage, transform, false); // 마지막 인수 = 몬스터는 크리티컬 공격 없음
        }
        // 트리거를 발동시킨 오브젝트의 태그가 플랫폼일 때
        else if (collision.gameObject.CompareTag("Platform"))
        {
            ExplosionBossSword();
        }
    }

    // 검 터짐 함수
    void ExplosionBossSword()
    {
        // 검 샷 애니메이션 끄기
        anim.SetBool("isShot", false);

        // 검 멈춤
        BossSwordRB.velocity = Vector2.zero;

        // 검 히트 이펙트 애니메이션을 재생할 오브젝트 풀에서 대여
        bossSwordHitFX = ObjectPoolingManager.instance.GetObject("Effect_BossSwordHit");
        hitAnim = bossSwordHitFX.GetComponent<Animator>();

        // 레이 발사
        hitInfo = Physics2D.Raycast(bulletPos.position, (headTransform.position - transform.position) * 10f);

        if (hitInfo)
        {
            // 위치 변경
            bossSwordHitFX.transform.position = transform.position;

            // 방향 변경 ( 보류 됨 ㅜ ) ---------------------------------------------
            //bossSwordHitFX.transform.rotation = new Quaternion(0, 0, Quaternion.LookRotation(headTransform.position - transform.position, Vector3.right).z, bossSwordHitFX.transform.rotation.w);
            Debug.Log(Quaternion.LookRotation(hitInfo.normal, Vector2.right));
            Debug.Log(Quaternion.LookRotation(hitInfo.normal));
        }

        // 검 히트 이펙트 애니메이션 재생
        hitAnim.SetTrigger("Hit");

        isHit = true; // 검 터짐 체크

        // 검 반환 함수가 이미 호출 되었다면 리턴
        if (IsInvoking("RetrunBossSword"))
        {
            return;
        }

        Invoke("RetrunBossSword", 1f); // 1초 후 검 풀에 반환
    }

    // 플레이어 조준 함수
    public void AimPlayer()
    {
        // 검을 발사했다면 조준 안함
        if (isShot)
        {
            return;
        }

        Vector3 dir = targetTransform.position - transform.position;

        // 방향에 따라 다른 조준 (정확히 위나 아래를 조준할 때 생기는 버그를 막기 위함)
        if (transform.localScale.x > 0)
        {
            aimPos.transform.right = dir.normalized;
        }
        else if (transform.localScale.x < 0)
        {
            aimPos.transform.right = -dir.normalized;
        }

        // 검 방향 설정
        if (shooterPos.localScale.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, aimPos.rotation.eulerAngles.z - 90);
        }
        else if (shooterPos.localScale.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, aimPos.rotation.eulerAngles.z + 90);
        }
    }

    void RetrunBossSword()
    {
        Debug.Log("검 반환!");
        gameObject.SetActive(false); // 검 풀에 반환
        bossSwordHitFX.SetActive(false); // 충돌 이펙트 풀에 반환
    }
}
