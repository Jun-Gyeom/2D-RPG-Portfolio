using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skull_Monster : MonsterManager
{
    Rigidbody2D rb;
    Animator anim;
    Transform targetTransform; // 플레이어의 위치

    Transform attackPos; // 공격받았을 때 상대가 공격한 위치

    // 드롭 아이템
    public GameObject coinPrefab; // 코인
    public GameObject expPrefab; // 경험치

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // 플레이어 위치 할당
        targetTransform = GameObject.FindWithTag("Player").transform;

        // HP 바 할당
        hp_Bar = GameObject.Find("Hp_Bar");
        hp_fill_GameObject = GameObject.Find("Hp_fill");
        hp_fill_Lerp_GameObject = GameObject.Find("Hp_fill_Lerp");

        hp_fill = hp_fill_GameObject.GetComponent<Image>(); // HP 바 fill 영역 이미지 할당
        hp_fill_Lerp = hp_fill_Lerp_GameObject.GetComponent<Image>(); // HP 바 부드러운 fill 영역 이미지 할당

        hp_Bar.SetActive(false); // HP 바 가리기


    }

    private void FixedUpdate()
    {
        // 선형보간 HP, EXP 바
        hp_fill_Lerp.fillAmount = Mathf.Lerp(hp_fill_Lerp.fillAmount, hp_fill.fillAmount, Time.deltaTime * 7.5f);

        // 몬스터가 죽었다면 리턴
        if (isDie)
        {
            return;
        }

        MonsterAI();
        PlatformCheck();

        // 공격 상태이고, 공격 애니메이션이 충분히 진행되었다면
        if (isAttack && (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f))
        {
            // 공격
            Attack();
        }
    }
    // 이동 함수
    void Move()
    {
        // 몬스터가 죽었거나 공격 중이라면 이동 금지
        if (anim.GetBool("isDeath") || (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f || anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)))
        {
            return;
        }

        // 이동
        rb.velocity = new Vector2(nextMove * moveSpeed, rb.velocity.y);

        // 이동 애니메이션
        anim.SetBool("isMove", true);

        // 몬스터 보는 방향
        if (nextMove < 0)
        {
            // 왼쪽
            Vector3 scale = transform.localScale; // 몬스터의 스케일 값
            scale.x = -Mathf.Abs(scale.x); // 스케일의 x값의 -절댓값
            transform.localScale = scale; // 몬스터의 스케일 값을 재정의

            Vector3 hpScale = hp_Bar.transform.localScale;
            hpScale.x = -Mathf.Abs(hpScale.x);
            hp_Bar.transform.localScale = hpScale; // HP 바는 항상 오른쪽을 봄
        }
        else if (nextMove > 0)
        {
            // 오른쪽
            Vector3 scale = transform.localScale; // 몬스터의 스케일 값
            scale.x = Mathf.Abs(scale.x); // 스케일의 x값의 절댓값
            transform.localScale = scale; // 몬스터의 스케일 값을 재정의

            Vector3 hpScale = hp_Bar.transform.localScale;
            hpScale.x = Mathf.Abs(hpScale.x);
            hp_Bar.transform.localScale = hpScale; // HP 바는 항상 오른쪽을 봄
        }
        else
        {
            anim.SetBool("isMove", false);
        }
    }

    // 공격 함수
    void Attack()
    {
        Collider2D[] players = Physics2D.OverlapCircleAll(attackTransform[0].position, attackRadius[0], LayerMask.GetMask("Player"));

        if (players != null)
        {
            foreach (Collider2D player in players)
            {
                attackPos = transform;
                player.GetComponent<PlayerManager>().TakeDamage(attackDamage, attackPos);
            }

            nextAttackTime = Time.time + attackCoolDown;

            isAttack = false; // 공격 상태를 false로 변경
        }
    }

    // 몬스터가 대미지를 받는 함수
    public override void TakeDamage(int damage, Transform Pos)
    {
        base.TakeDamage(damage, Pos); // Entity Manager의 TakeDamage함수 실행
        anim.SetTrigger("Hit"); // 맞는 애니메이션

        // 죽지 않았을 때만
        if (!anim.GetBool("isDeath"))
        {
            // 넉백
            float x = transform.position.x - Pos.position.x; // 밀려날 방향
            if (x > 0)
            {
                rb.velocity = new Vector2(3f, rb.velocity.y); // 오른쪽으로 3만큼 넉백 
            }
            else if (x < 0)
            {
                rb.velocity = new Vector2(-3f, rb.velocity.y); // 왼쪽으로 3만큼 넉백 
            }

            hp_Bar.SetActive(true); // HP 바 활성화

            CancelInvoke("Hide_HpBar"); // 초기화
            Invoke("Hide_HpBar", 2f); // 2초 후 HP 바 비활성화
        }

        // 공격 끊기
        nextAttackTime = Time.time + attackCoolDown;

        hp_fill.fillAmount = health / maxHealth; // 체력바 조절
    }

    // 몬스터가 죽는 함수
    public override void Die()
    {
        base.Die();

        anim.SetBool("isDeath", true); // 죽는 애니메이션

        GetComponent<Collider2D>().enabled = false; // 콜라이더 끄기
        rb.isKinematic = true; // 위치 고정
        rb.velocity = new Vector2(0, 0); // 위치 고정

        hp_Bar.gameObject.SetActive(false); // HP 바 끄기
    }

    // 몬스터 인공지능 함수
    public override void MonsterAI()
    {
        if (Vector2.Distance(transform.position, targetTransform.position) < attackDistance)  // 플레이어가 공격 범위 안에 있다.
        {
            // 공격 애니메이션이 진행 중이지 않을 때
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                // 몬스터 보는 방향
                if (targetTransform.position.x < transform.position.x)
                {
                    // 왼쪽
                    Vector3 scale = transform.localScale; // 몬스터의 스케일 값
                    scale.x = -Mathf.Abs(scale.x); // 스케일의 x값의 -절댓값
                    transform.localScale = scale; // 몬스터의 스케일 값을 재정의

                    Vector3 hpScale = hp_Bar.transform.localScale;
                    hpScale.x = -Mathf.Abs(hpScale.x);
                    hp_Bar.transform.localScale = hpScale; // HP 바는 항상 오른쪽을 봄
                }
                if (targetTransform.position.x > transform.position.x)
                {
                    // 오른쪽
                    Vector3 scale = transform.localScale; // 몬스터의 스케일 값
                    scale.x = Mathf.Abs(scale.x); // 스케일의 x값의 절댓값
                    transform.localScale = scale; // 몬스터의 스케일 값을 재정의

                    Vector3 hpScale = hp_Bar.transform.localScale;
                    hpScale.x = Mathf.Abs(hpScale.x);
                    hp_Bar.transform.localScale = hpScale; // HP 바는 항상 오른쪽을 봄
                }
            }

            if (Time.time >= nextAttackTime)
            {
                // 공격 애니메이션
                
                anim.SetTrigger("Attack");

                isAttack = true; // 공격 상태를 true로 변경

                nextAttackTime = Time.time + attackCoolDown;
            }
        }
        // 플레이어가 공격 범위 밖에 있다.
        else
        {
            // 맞는 애니메이션이 진행 중일 때
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            {
                return;
            }

            // 이동
            Move();

            if (Vector2.Distance(transform.position, targetTransform.position) <= followDistance) // 플레이어가 인식 범위 안에 있다.
            {
                // 플레이어 위치 벡터 값
                Vector3 playerPos = targetTransform.position;

                // 플레이어의 x위치가 몬스터의 x위치보다 작으면
                if (playerPos.x < transform.position.x)
                {
                    // 왼쪽으로 이동
                    nextMove = -1;
                }

                // 플레이어의 x위치가 몬스터의 공격 범위의 절반보다 가까우면
                else if ((playerPos.x > transform.position.x - attackDistance / 2) && (playerPos.x < transform.position.x + attackDistance / 2))
                {
                    // 이동 안 함
                    nextMove = 0;
                }

                // 플레이어의 x위치가 몬스터의 x위치보다 크면
                else if (playerPos.x > transform.position.x)
                {
                    // 오른쪽으로 이동
                    nextMove = 1;
                }
            }

            // 플레이어가 인식 범위 밖에 있다.
            else
            {

                thinkTime += Time.deltaTime;

                // 다음 행동 결정 시작
                if (thinkTime >= nextThinkTime)
                {
                    thinkTime = 0;

                    nextMove = Random.Range(-1, 2); // 랜덤 -1 ~ 1까지 숫자 받기

                    nextThinkTime = Random.Range(2, 6); // 다음 행동을 정하기까지 걸리는 시간 받기
                }
            }
        }
    }

    // AI가 낭떨어지로 가는 것을 막는 함수
    public void PlatformCheck()
    {
        Vector2 frontVec = new Vector2(rb.position.x + nextMove, rb.position.y);

        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            nextMove = 0; // 멈춤
        }
    }

    void Hide_HpBar()
    {
        hp_Bar.SetActive(false);
    }
}
