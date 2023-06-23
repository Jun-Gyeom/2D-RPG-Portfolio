using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonsterManager
{
    public Rigidbody2D rb; // 박쥐 리지드바디
    public Animator anim; // 박쥐 애니메이터

    public Animator dieAnim; // 사망 시 이펙트 애니메이터
    public Transform attackPos; // 공격받았을 때 상대가 공격한 위치

    public int nextMoveY; // 몬스터의 Y축 이동 방향 (-1, 0, 1)

    private void Start()
    {
        // 플레이어 위치 할당
        targetTransform = GameObject.FindWithTag("Player").transform;

        hp_Bar.SetActive(false); // HP 바 가리기
    }

    private void FixedUpdate()
    {
        // 선형보간 HP 바
        hp_fill_Lerp.fillAmount = Mathf.Lerp(hp_fill_Lerp.fillAmount, hp_fill.fillAmount, Time.deltaTime * 7.5f);

        // 몬스터가 죽었다면 리턴
        if (isDie)
        {
            return;
        }

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
        else if (targetTransform.position.x > transform.position.x)
        {
            // 오른쪽
            Vector3 scale = transform.localScale; // 몬스터의 스케일 값
            scale.x = Mathf.Abs(scale.x); // 스케일의 x값의 절댓값
            transform.localScale = scale; // 몬스터의 스케일 값을 재정의

            Vector3 hpScale = hp_Bar.transform.localScale;
            hpScale.x = Mathf.Abs(hpScale.x);
            hp_Bar.transform.localScale = hpScale; // HP 바는 항상 오른쪽을 봄
        }

        MonsterAI();
    }

    // 이동 함수
    public virtual void Move()
    {
        // 몬스터가 죽었다면 이동 금지
        if (isDie)
        {
            return;
        }

        // 이동
        rb.velocity = new Vector2(nextMove * moveSpeed, nextMoveY * moveSpeed);
    }
    // 몬스터 행동 AI 함수
    public override void MonsterAI()
    {
        // 피격이 진행 중이거나 공격 중일 때
        if (isHitMonster || isAttack)
        {
            return;
        }

        // 이동
        Move();

        thinkTime += Time.deltaTime;

        // 다음 행동 결정 시작
        if (thinkTime >= nextThinkTime)
        {
            thinkTime = 0;

            nextMove = Random.Range(-1, 2); // 랜덤 -1 ~ 1까지 숫자 받기
            nextMoveY = Random.Range(-1, 2); // Y축 랜덤 -1 ~ 1까지 숫자 받기

            nextThinkTime = Random.Range(1f, 2.5f); // 다음 행동을 정하기까지 걸리는 시간 받기
        }
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
}
