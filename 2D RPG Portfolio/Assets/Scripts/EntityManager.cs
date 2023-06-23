using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    [SerializeField]
    public float health; // 현재 체력
    public float maxHealth; // 최대 체력

    public int attackDamage; // 공격력
    public float moveSpeed; // 이동 속도

    public Transform[] attackTransform; // 공격 위치

    public float[] attackRadius; // 공격 범위
    public float attackCoolDown; // 공격 후 지난 시간
    public float nextAttackTime; // 다시 공격이 가능한 시간

    public SpriteRenderer sr; // 스프라이트 렌더러

    public float hitAnimFadeTime; // 피격 애니메이션 페이드 되는 시간
    public bool isHitMonster; // 몬스터 피격 상태인지 체크

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>(); // 스프라이트 렌더러 할당
        health = maxHealth; // 체력 초기화
    }

    // 대미지를 받는 함수
    public virtual void TakeDamage(int damage, Transform Pos, bool isCritical)
    {
        // 대상이 몬스터라면
        if (gameObject.tag == "Monster")
        {
            isHitMonster = true; // 몬스터 피격 상태 체크

            SoundManager.instance.PlaySound("EnemyHit"); // 사운드 재생

            StartCoroutine("HitAnimation_Red"); // 몬스터 피격 애니메이션
        }
        // 대상이 플레이어라면
        else if (gameObject.tag == "Player")
        {
            StartCoroutine("HitAnimation_Alpha"); // 플레이어 피격 애니메이션
        }    

        // 치명타라면
        if (isCritical)
        {
            health -= Mathf.RoundToInt(damage * (GameManager.instance.critical_Value + GameManager.instance.increased_CriticalValue)); // 크리티컬 대미지
        }
        else
        {
            health -= damage;
        }

        if (health <= 0)
        {
            Die();
        }
    }

    // 죽는 함수
    public virtual void Die() { }

    // 기즈모 표시
    public virtual void OnDrawGizmosSelected()
    {
        // 공격 범위 기즈모
        if (attackTransform.Length > 0)
        {
            Color gizmosColor = Gizmos.color;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackTransform[0].position, attackRadius[0]);
            Gizmos.color = gizmosColor;
        }
    }

    // 몬스터 피격 애니메이션 코루틴
    IEnumerator HitAnimation_Red()
    {
        // 엔티티 색상 빨간색으로 변경
        sr.material.color = new Color(1f, 0.15f, 0.15f, 1f);

        // 페이드 되는 시간
        float time = 0;

        Color alpha = sr.material.color;

        while (alpha.g < 1f)
        {
            time += Time.deltaTime / hitAnimFadeTime;

            // 페이드 인
            alpha.g = Mathf.Lerp(0.15f, 1f, time);
            alpha.b = Mathf.Lerp(0.15f, 1f, time);

            // 색상 적용
            sr.material.color = alpha;

            yield return null;
        }

        isHitMonster = false; // 몬스터 피격 상태 해제

        yield return null;
    }

    // 플레이어 피격 애니메이션 코루틴
    IEnumerator HitAnimation_Alpha()
    {
        // 엔티티 색상 반투명색으로 변경
        sr.material.color = new Color(1f, 1f, 1f, 0.45f);

        // 페이드 되는 시간
        float time = 0;

        Color alpha = sr.material.color;

        while (alpha.a < 1f)
        {
            time += Time.deltaTime / hitAnimFadeTime;

            // 페이드 인
            alpha.a = Mathf.Lerp(0.45f, 1f, time);

            // 색상 적용
            sr.material.color = alpha;

            yield return null;
        }
    }
}
