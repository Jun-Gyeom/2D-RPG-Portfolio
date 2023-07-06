using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantBat : MonsterManager
{
    public Rigidbody2D rb; // 거대 박쥐 리지드바디
    public Animator anim; // 거대 박쥐 애니메이터

    public Animator dieAnim; // 사망 시 이펙트 애니메이터

    public bool isFireBallSetting; // 현재 파이어볼 세팅 중인지 체크

    // 발사체 관련
    FireBall[] fb; // 파이어볼 스크립트 배열

    public float sideFireBallAngle; // 사이드 파이어볼 각도
    public float fireBallSpeed; // 파이어볼 속도
    public float fireBallDistance; // 파이어볼 사거리
    public int shotNumber; // 공격 당 연속 발사 횟수
    public float shotDelay; // 연속 발사 시 딜레이

    public GameObject aimPosObject; // 파이어볼을 발사할 방향 값을 가진 오브젝트
    public Transform bulletPos; // 파이어볼을 발사할 위치

    public Transform leftSideBulletPos; // 좌측 파이어볼을 발사할 위치
    public Transform rightSideBulletPos; // 우측 파이어볼을 발사할 위치
    public Transform laftSidePosObject; // 좌측 파이어볼 발사를 위한 방향 값을 가진 오브젝트
    public Transform rightSidePosObject; // 우측 파이어볼 발사를 위한 방향 값을 가진 오브젝트

    private void Start()
    {
        // 플레이어 위치 할당
        targetTransform = GameObject.FindWithTag("Player").transform;

        // 배열 초기화
        fb = new FireBall[shotNumber * 3];

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

        AimPlayer();
        MonsterAI();

        // 공격 상태라면
        if (isAttack)
        {
            StartCoroutine("SetFireBall"); // 파이어볼 세팅

            isAttack = false; // 공격 중 아님
        }

        // 만약 파이어볼 세팅 중이라면 리턴
        if (isFireBallSetting)
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

    // 플레이어 조준 함수
    public void AimPlayer()
    {
        // 파이어볼 세팅 중이라면 조준 안 함
        if (isFireBallSetting)
        {
            return;
        }

        Vector3 dir = targetTransform.position - transform.position;

        // 방향에 따라 다른 조준 (정확히 위나 아래를 조준할 때 생기는 버그를 막기 위함)
        if (transform.localScale.x > 0)
        {
            aimPosObject.transform.right = dir.normalized;
        }
        else if (transform.localScale.x < 0)
        {
            aimPosObject.transform.right = -dir.normalized;
        }
    }

    // 파이어볼 세팅 함수
    IEnumerator SetFireBall()
    {
        // 파이어볼 세팅 중 체크
        isFireBallSetting = true;

        int k = 0; // 파이어볼 인덱스 변수

        // 파이어볼 생성
        for (int i = 0; i < shotNumber; i++)
        {
            // [좌측 파이어볼]
            // 오브젝트 풀에서 파이어볼 프리팹 대여
            GameObject fireBall_Left = ObjectPoolingManager.instance.GetObject("Bullet_FireBall");
            fb[k] = fireBall_Left.GetComponent<FireBall>();

            // 파이어볼 변수 전달
            fb[k].speed = fireBallSpeed; // 파이어볼 속도
            fb[k].distance = fireBallDistance; // 파이어볼 사거리

            laftSidePosObject.rotation = aimPosObject.transform.rotation * Quaternion.Euler(new Vector3(0, 0, -sideFireBallAngle)); // 조준 방향에 사이드 파이어볼 각도를 뺀 값을 구함
            fb[k].aimPos = laftSidePosObject.transform; // 파이어볼 발사 방향

            fb[k].bulletPos = leftSideBulletPos; // 파이어볼 발사 위치
            fb[k].shooterPos = transform; // 발사한 객체의 위치
            fb[k].damage = attackDamage; // 공격력
            fb[k].failCause = "자이언트 박쥐에게 당함"; // 사망 사유

            // 파이어볼 세팅
            fb[k].Setting();

            k++;

            // [중앙 파이어볼]
            // 오브젝트 풀에서 파이어볼 프리팹 대여
            GameObject fireBall_Middle = ObjectPoolingManager.instance.GetObject("Bullet_FireBall");
            fb[k] = fireBall_Middle.GetComponent<FireBall>();

            // 파이어볼 변수 전달
            fb[k].speed = fireBallSpeed; // 파이어볼 속도
            fb[k].distance = fireBallDistance; // 파이어볼 사거리

            fb[k].aimPos = aimPosObject.transform; // 파이어볼 발사 방향

            fb[k].bulletPos = bulletPos; // 파이어볼 발사 위치
            fb[k].shooterPos = transform; // 발사한 객체의 위치
            fb[k].damage = attackDamage; // 공격력
            fb[k].failCause = "자이언트 박쥐에게 패배"; // 사망 사유

            // 파이어볼 세팅
            fb[k].Setting();

            k++;

            // [우측 파이어볼]
            // 오브젝트 풀에서 파이어볼 프리팹 대여
            GameObject fireBall_Right = ObjectPoolingManager.instance.GetObject("Bullet_FireBall");
            fb[k] = fireBall_Right.GetComponent<FireBall>();

            // 파이어볼 변수 전달
            fb[k].speed = fireBallSpeed; // 파이어볼 속도
            fb[k].distance = fireBallDistance; // 파이어볼 사거리

            rightSidePosObject.rotation = aimPosObject.transform.rotation * Quaternion.Euler(new Vector3(0, 0, sideFireBallAngle)); // 조준 방향에 사이드 파이어볼 각도를 더한 값을 구함
            fb[k].aimPos = rightSidePosObject.transform; // 파이어볼 발사 방향

            fb[k].bulletPos = rightSideBulletPos; // 파이어볼 발사 위치
            fb[k].shooterPos = transform; // 발사한 객체의 위치
            fb[k].damage = attackDamage; // 공격력
            fb[k].failCause = "자이언트 박쥐에게 패배"; // 사망 사유

            // 파이어볼 세팅
            fb[k].Setting();

            k++;

            // 잠시 후
            yield return new WaitForSeconds(shotDelay);
        }

        // 공격 애니메이션
        anim.SetTrigger("Attack");

        // 0.4초 기다린 후
        yield return new WaitForSeconds(0.4f);

        // 파이어볼 발사 코루틴 실행
        StartCoroutine("Shot");
    }

    // 파이어볼 발사 함수
    IEnumerator Shot()
    {
        Debug.Log("거대 박쥐 발사!");

        int k = 0; // 파이어볼 인덱스 변수 선언

        // 파이어볼 발사
        for (int i = 0; i < shotNumber; i++)
        {
            fb[k].Shot(); // 중앙
            k++;

            fb[k].Shot(); // 좌측
            k++;

            fb[k].Shot(); // 우측
            k++;

            yield return new WaitForSeconds(shotDelay);
        }

        // 파이어볼 세팅 중 아님
        isFireBallSetting = false;
    }

    // HP바를 숨기는 함수
    public override void Hide_HpBar()
    {
        base.Hide_HpBar();
    }
}
