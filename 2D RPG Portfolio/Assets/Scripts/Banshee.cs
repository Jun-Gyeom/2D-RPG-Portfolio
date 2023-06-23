using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banshee : MonsterManager
{
    public Rigidbody2D rb; // 붉은 거대 박쥐 리지드바디
    public Animator anim; // 붉은 거대 박쥐 애니메이터

    public Animator dieAnim; // 사망 시 이펙트 애니메이터

    // 발사체 관련
    MusicNote[] mn; // 음표 스크립트 배열

    public int shotMusicNoteNum; // 발사할 음표 개수
    public float musicNoteSpeed; // 음표 속도
    public float musicNoteDistance; // 음표 사거리

    public GameObject aimPosObject; // 음표를 발사할 방향 값을 가진 오브젝트
    public Transform bulletPos; // 음표를 발사할 위치

    private void Start()
    {
        // 플레이어 위치 할당
        targetTransform = GameObject.FindWithTag("Player").transform;

        // 배열 초기화
        mn = new MusicNote[shotMusicNoteNum];

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

        MonsterAI();

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

        // 공격 상태라면
        if (isAttack)
        {
            StartCoroutine("SetMusicNote"); // 음표 세팅

            // 공격
            Attack();

            // 공격 애니메이션
            anim.SetTrigger("Attack");

            // 공격 사운드 재생
            SoundManager.instance.PlaySound("Banshee_Scream");

            isAttack = false; // 공격 중 아님
        }
    }

    // 공격 함수
    public void Attack()
    {
        // 모든 음표 발사 함수 실행
        for (int i = 0; i < mn.Length; i++)
        {
            mn[i].Shot();
        }
    }

    // 몬스터 행동 AI 함수
    public override void MonsterAI()
    {
        // 플레이어가 공격 범위 안에 있다.
        if (Vector2.Distance(transform.position, targetTransform.position) < attackDistance)
        {
            if (Time.time >= nextAttackTime)
            {
                isAttack = true; // 공격 상태를 true로 변경

                nextAttackTime = Time.time + attackCoolDown;
            }
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
        }
        else
        {
            // 대미지 허드 텍스트 생성
            GameObject damage_HudText = ObjectPoolingManager.instance.GetObject("HudText_Damage");
            damage_HudText.transform.position = hudPos.position; // 허드 텍스트 위치 변경
            damage_HudText.transform.GetChild(0).GetComponent<Damage_HudText>().ShowDamageText(damage); // 허드 텍스트가 표시 할 대미지 전달 (그림자)
            damage_HudText.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<DamageHudText_Shadow>().ShowDamageText(damage); // 허드 텍스트가 표시 할 대미지 전달 (텍스트)
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

    // 음표 세팅 함수
    IEnumerator SetMusicNote()
    {
        // 항상 랜덤한 방향으로 공격하기 위한 랜덤 회전
        int rand_Angle = Random.Range(0, 360); // (0 ~ 359)
        aimPosObject.transform.Rotate(new Vector3(0, 0, rand_Angle)); // 회전

        // 음표 생성
        for (int i = 0; i < shotMusicNoteNum; i++)
        {
            // 오브젝트 풀에서 음표 프리팹 대여
            GameObject musicNote = ObjectPoolingManager.instance.GetObject("Bullet_MusicNote");
            mn[i] = musicNote.GetComponent<MusicNote>();

            // 음표 변수 전달
            mn[i].speed = musicNoteSpeed; // 음표 속도
            mn[i].distance = musicNoteDistance; // 음표 사거리
            mn[i].aimPos = aimPosObject.transform; // 음표 발사 방향
            mn[i].bulletPos = bulletPos; // 음표 발사 위치
            mn[i].shooterPos = transform; // 발사한 객체의 위치
            mn[i].damage = attackDamage; // 공격력
            mn[i].failCause = "밴시에게 패배"; // 사망 사유

            // 음표 세팅
            mn[i].Setting();

            float angle = 360 / shotMusicNoteNum;
            aimPosObject.transform.Rotate(new Vector3(0, 0, angle)); // 회전
        }

        return null;
    }

    // HP바를 숨기는 함수
    public override void Hide_HpBar()
    {
        base.Hide_HpBar();
    }
}
