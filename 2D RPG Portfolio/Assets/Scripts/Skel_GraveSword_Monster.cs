using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skel_GraveSword_Monster : SkelManager
{
    public Animator secondHandsAnim; // 두 번째 손 애니메이터
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 플레이어 위치 할당
        targetTransform = GameObject.FindWithTag("Player").transform;

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
        if (isAttack && (WeaponAnim.GetCurrentAnimatorStateInfo(0).IsName("Skel_GraveSword_Attack") && WeaponAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f))
        {
            // 공격
            Attack();
        }
    }

    public override void Move()
    {
        base.Move();
    }

    public override void Attack()
    {
        base.Attack();

        // 공격 사운드 재생
        SoundManager.instance.PlaySound("GraveSwordSkel_Attack");

        Collider2D[] players = Physics2D.OverlapCircleAll(attackTransform[0].position, attackRadius[0], LayerMask.GetMask("Player"));

        if (players != null)
        {
            foreach (Collider2D player in players)
            {
                GameManager.instance.failCause = "그레이브소드 스켈레톤에게 패배"; // 사망 이유

                attackPos = transform;
                player.GetComponent<PlayerManager>().TakeDamage(attackDamage, attackPos, false); // 마지막 인수 = 몬스터는 크리티컬 공격 없음
            }

            nextAttackTime = Time.time + attackCoolDown;

            isAttack = false; // 공격 상태를 false로 변경
        }
    }

    public override void TakeDamage(int damage, Transform Pos, bool isCritical)
    {
        base.TakeDamage(damage, Pos, isCritical);

        secondHandsAnim.SetTrigger("Hit"); // 두 번째 손 맞는 애니메이션
    }

    public override void Die()
    {
        base.Die();
    }

    public override void MonsterAI()
    {
        base.MonsterAI();

        if (Vector2.Distance(transform.position, targetTransform.position) < attackDistance)  // 플레이어가 공격 범위 안에 있다.
        {
            // 이동 애니메이션 끄기
            skelAnim.SetBool("isMove", false);

            // 공격 애니메이션이 진행 중이지 않을 때
            if (!WeaponAnim.GetCurrentAnimatorStateInfo(0).IsName("Skel_GraveSword_Attack"))
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
                handsAnim.SetTrigger("Attack");
                secondHandsAnim.SetTrigger("Attack");
                WeaponAnim.SetTrigger("Attack");

                isAttack = true; // 공격 상태를 true로 변경

                nextAttackTime = Time.time + attackCoolDown;
            }
        }
        // 플레이어가 공격 범위 밖에 있다.
        else
        {
            // 맞는 애니메이션이 진행 중일 때
            if (skelAnim.GetCurrentAnimatorStateInfo(0).IsName("Skel_Hit"))
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

                    nextThinkTime = Random.Range(2, 4); // 다음 행동을 정하기까지 걸리는 시간 받기
                }
            }
        }
    }

    public override void PlatformCheck()
    {
        base.PlatformCheck();
    }

    public override void DropItem()
    {
        base.DropItem();
    }

    public override void Hide_HpBar()
    {
        base.Hide_HpBar();
    }
}