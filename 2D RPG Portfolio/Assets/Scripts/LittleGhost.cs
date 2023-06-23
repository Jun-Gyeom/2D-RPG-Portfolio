using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleGhost : MonsterManager
{
    public Rigidbody2D rb; // 꼬마유령 리지드바디
    public Animator anim; // 꼬마유령 애니메이터

    public Animator dieAnim; // 사망 시 이펙트 애니메이터
    public Transform attackPos; // 공격받았을 때 상대가 공격한 위치

    Vector2 derection; // 플레이어가 있는 방향
    public float attackMoveSpeed; // 공격 시 이동 속도
    public bool isDash; // 현재 대쉬공격 중인지 체크

    private void Start()
    {
        // 플레이어 위치 할당
        targetTransform = GameObject.FindWithTag("Player").transform;

        hp_Bar.SetActive(false); // HP 바 가리기
    }

    private void Update()
    {
        // 공격 상태라면
        if (isAttack)
        {
            Collider2D[] players = Physics2D.OverlapCircleAll(attackTransform[0].position, attackRadius[0], LayerMask.GetMask("Player"));

            // 플레이어가 피격 범위 안에 들어왔다면 대미지 적용
            if (players != null)
            {
                foreach (Collider2D player in players)
                {
                    GameManager.instance.failCause = "꼬마유령에게 패배"; // 사망 이유

                    attackPos = transform;
                    player.GetComponent<PlayerManager>().TakeDamage(attackDamage, attackPos, false); // 마지막 인수 = 몬스터는 크리티컬 공격 없음

                    isAttack = false; // 공격 상태를 false로 변경
                }

                nextAttackTime = Time.time + attackCoolDown;
            }
        }
    }

    private void FixedUpdate()
    {
        // 선형보간 HP 바
        hp_fill_Lerp.fillAmount = Mathf.Lerp(hp_fill_Lerp.fillAmount, hp_fill.fillAmount, Time.deltaTime * 7.5f);

        // 플레이어가 있는 방향 계산
        derection = new Vector2(targetTransform.position.x - transform.position.x, targetTransform.position.y - transform.position.y);

        // 몬스터가 죽었다면 리턴
        if (isDie)
        {
            return;
        }

        MonsterAI();
    }

    // 이동 함수
    public virtual void Move()
    {
        // 몬스터가 죽었거나 대쉬공격 중이라면 이동 금지
        if (isDie || isDash)
        {
            return;
        }

        // 거리가 너무 가깝지 않을 때만 이동
        if (Vector2.Distance(transform.position, targetTransform.position) > 0.5f)
        {
            rb.velocity = derection.normalized * moveSpeed; // 이동
        }
        else
        {
            rb.velocity = Vector2.zero; // 너무 가까우면 이동하지 않음
        }

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
    }

    // 꼬마유령 대쉬 공격 함수
    public void Attack()
    {
        isAttack = true; // 현재 공격 중임
        isDash = true; // 현재 대쉬공격 중임
        anim.SetBool("isAttack", isDash); // 공격 애니메이션 

        rb.velocity = derection.normalized * attackMoveSpeed; // 대쉬 공격

        Invoke("EndAttack", 1.5f); // 1.5초 후 isAttack = false로 변경
    }

    // 몬스터 행동 AI 함수
    public override void MonsterAI()
    {
        // 피격이 진행 중이거나 대쉬공격 중일 때
        if (isHitMonster || isDash)
        {
            return;
        }

        // 플레이어 위치 벡터 값
        Vector3 playerPos = targetTransform.position;

        // 플레이어의 x위치가 몬스터의 x위치보다 작으면
        if (playerPos.x < transform.position.x - 0.5f)
        {
            // 왼쪽으로 이동
            nextMove = -1;
        }
        // 플레이어의 x위치가 몬스터의 x위치보다 크면
        else if (playerPos.x > transform.position.x + 0.5f)
        {
            // 오른쪽으로 이동
            nextMove = 1;
        }
        else
        {
            // 가만히
            nextMove = 0;
        }

        // 대쉬공격 중이지 않을 때
        if (!isDash)
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

        // 플레이어가 공격 범위 안에 있다.
        if (Vector2.Distance(transform.position, targetTransform.position) < attackDistance)
        {
            // 공격 쿨타임이 아닐 때
            if (Time.time >= nextAttackTime)
            {
                Attack(); // 공격

                nextAttackTime = Time.time + attackCoolDown; // 쿨타임
            }
        }

        // 이동
        Move();
    }

    // 대미지 적용 함수
    public override void TakeDamage(int damage, Transform Pos, bool isCritical)
    {
        // 죽었다면 리턴
        if (isDie)
        {
            return;
        }

        hp_Bar.SetActive(true); // HP 바 활성화

        CancelInvoke("Hide_HpBar"); // 초기화
        Invoke("Hide_HpBar", 2f); // 2초 후 HP 바 비활성화

        EndAttack(); // 대쉬공격 끊기

        base.TakeDamage(damage, Pos, isCritical); // 대미지 적용

        // 크리티컬이라면
        if (isCritical)
        {
            // 크리티컬 대미지 허드 텍스트 생성
            GameObject damage_HudText = ObjectPoolingManager.instance.GetObject("HudText_CriticalDam");
            damage_HudText.transform.position = hudPos.position; // 허드 텍스트 위치 변경
            damage_HudText.transform.GetChild(0).GetComponent<Damage_HudText>().ShowDamageText(Mathf.RoundToInt(damage * (GameManager.instance.critical_Value + GameManager.instance.increased_CriticalValue))); // 허드 텍스트가 표시 할 대미지 전달 (그림자)
            damage_HudText.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<DamageHudText_Shadow>().ShowDamageText(Mathf.RoundToInt(damage * (GameManager.instance.critical_Value + GameManager.instance.increased_CriticalValue))); // 허드 텍스트가 표시 할 대미지 전달 (텍스트)

            // 넉백
            float x = transform.position.x - Pos.position.x; // 밀려날 방향
            if (x > 0)
            {
                rb.velocity = new Vector2(5f, rb.velocity.y); // 오른쪽으로 5만큼 넉백 
            }
            else if (x < 0)
            {
                rb.velocity = new Vector2(-5f, rb.velocity.y); // 왼쪽으로 5만큼 넉백 
            }
        }
        else
        {
            // 대미지 허드 텍스트 생성
            GameObject damage_HudText = ObjectPoolingManager.instance.GetObject("HudText_Damage");
            damage_HudText.transform.position = hudPos.position; // 허드 텍스트 위치 변경
            damage_HudText.transform.GetChild(0).GetComponent<Damage_HudText>().ShowDamageText(damage); // 허드 텍스트가 표시 할 대미지 전달 (그림자)
            damage_HudText.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<DamageHudText_Shadow>().ShowDamageText(damage); // 허드 텍스트가 표시 할 대미지 전달 (텍스트)

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
        }

        hp_fill.fillAmount = health / maxHealth; // 체력바 조절
    }

    // 사망 함수
    public override void Die()
    {
        base.Die();

        anim.SetTrigger("Die"); // 죽는 애니메이션
        hp_Bar.gameObject.SetActive(false); // HP 바 비활성화
    }

    // 아이템 드롭
    public override void DropItem()
    {
        base.DropItem();

        // 드롭할 아이템 갯수 랜덤 값
        int rand_dropAmount = Random.Range(minItemDrop, maxItemDrop);

        // 드롭할 아이템 갯수만큼 for문 실행
        for (int i = 0; i < rand_dropAmount; i++)
        {
            float rand_dropItem = Random.Range(0f, 1f);

            if (rand_dropItem < 0.55f) // 경험치 (55%) 
            {
                Debug.Log("경험치 드롭");
                GameObject exp = ObjectPoolingManager.instance.GetObject("Item_Exp"); // 오브젝트 풀에서 경험치 대여
                exp.transform.position = this.transform.position; // 위치 초기화
            }
            else if (rand_dropItem < 0.95f) // 코인 (40%) 
            {
                Debug.Log("코인 드롭");
                GameObject coin = ObjectPoolingManager.instance.GetObject("Item_Coin"); // 오브젝트 풀에서 코인 대여
                coin.transform.position = this.transform.position; // 위치 초기화
            }
            else if (rand_dropItem < 1.0f) // 금괴 (5%) 
            {
                Debug.Log("금괴 드롭");
                GameObject bullion = ObjectPoolingManager.instance.GetObject("Item_Bullion"); // 오브젝트 풀에서 금괴 대여
                bullion.transform.position = this.transform.position; // 위치 초기화
            }
        }
    }

    // HP바를 숨기는 함수
    public override void Hide_HpBar()
    {
        base.Hide_HpBar();
    }

    // 대쉬 공격 체크 값을 해제하는 함수
    public void EndAttack()
    {
        isAttack = false; // 현재 공격 중이 아님
        isDash = false; // 현재 대쉬공격 중이 아님
        anim.SetBool("isAttack", isDash); // 공격 애니메이션
    }
}
