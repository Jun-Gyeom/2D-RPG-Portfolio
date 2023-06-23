using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkelManager : MonsterManager
{
    public Rigidbody2D rb;
    public Animator skelAnim; // 스켈 애니메이터
    public Animator handsAnim; // 손 애니메이터
    public Animator WeaponAnim; // 무기 애니메이터

    public GameObject handsPosObject; // 손 위치 오브젝트

    public Transform attackPos; // 공격받았을 때 상대가 공격한 위치

    // 이동 함수
    public virtual void Move()
    {
        // 몬스터가 죽었거나 공격 중이라면 이동 금지
        if (isDie || (WeaponAnim.GetCurrentAnimatorStateInfo(0).IsName("Skel_ShortSword_Attack") && (WeaponAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f || WeaponAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)))
        {
            return;
        }

        // 이동
        rb.velocity = new Vector2(nextMove * moveSpeed, rb.velocity.y);

        // 이동 애니메이션
        skelAnim.SetBool("isMove", true);

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
            skelAnim.SetBool("isMove", false);
        }
    }

    // 공격 함수
    public virtual void Attack() { }

    // 대미지 받는 함수
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

        skelAnim.SetTrigger("Hit"); // 맞는 애니메이션
        handsAnim.SetTrigger("Hit"); // 손 맞는 애니메이션

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

        // 공격 끊기 (보류)
        //nextAttackTime = Time.time + attackCoolDown;

        hp_fill.fillAmount = health / maxHealth; // 체력바 조절
    }

    public override void Die()
    {
        base.Die();

        skelAnim.SetTrigger("Die"); // 죽는 애니메이션

        //GetComponent<Collider2D>().enabled = false; // 콜라이더 끄기
        //rb.isKinematic = true; // 위치 고정
        //rb.velocity = new Vector2(0, 0); // 위치 고정

        handsPosObject.SetActive(false); // 손과 무기 비활성화
        hp_Bar.gameObject.SetActive(false); // HP 바 비활성화
    }

    // 몬스터 AI
    public override void MonsterAI()
    {
        base.MonsterAI();
    }

    // 몬스터가 낭떨어지로 가는 것을 막는 함수
    public virtual void PlatformCheck()
    {
        Vector2 frontVec = new Vector2(rb.position.x + nextMove, rb.position.y);

        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            nextMove = 0; // 멈춤
        }
    }

    // 아이템 드롭 함수
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

        // 스켈 머리 오브젝트 드롭
        GameObject skelHead = ObjectPoolingManager.instance.GetObject("Object_SkelHead"); // 오브젝트 풀에서 스켈 머리 대여
        skelHead.transform.position = this.transform.position; // 위치 초기화
    }
}
