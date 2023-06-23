using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SkellBoss : MonsterManager
{
    public Animator head_Anim; // 머리 애니메이터
    public Animator skellBoss_Anim; // 델리알 애니메이터

    public SkellBossHands leftHand; // 왼쪽 손 스크립트
    public SkellBossHands rightHand; // 오른쪽 손 스크립트

    public int patternNumber; // 공격 패턴의 갯수
    public bool isSetPattern; // 다음 공격 패턴을 골랐는지 체크

    public int min_LaserShotNum; // 레이저 공격을 할 최소 횟수
    public int max_LaserShotNum; // 레이저 공격을 할 최대 횟수

    public bool isLeftHand; // 왼손을 사용할 차례인지 체크

    public bool isBattleStart; // 전투 시작했는지 체크
    public bool isFindPlayer; // 플레이어를 감지했는지 체크

    public ParticleSystem bossDie_Particle; // 보스 사망 파티클
    public ParticleSystem bossDieBoom_Particle; // 보스 사망 폭발 파티클

    public GameObject bossSkullPrefab; // 보스 해골 프리팹


    // 카메라, 컷씬 관련
    public PlayableDirector director_Show; // 보스 등장 씬 타임라인 재생할 디렉터
    public PlayableDirector director_BossDie; // 보스 사망 씬 타임라인 재생할 디렉터

    public GameObject defaultCam; // 기본 카메라
    public GameObject battleCam; // 전투 카메라

    // 발사체 관련 - 섀도우볼
    ShadowBall[] sb; // 섀도우볼 스크립트 배열

    public int shotShadowBallNum; // 발사할 섀도우볼 개수
    public float shotTime; // 섀도우볼을 발사할 시간
    public float rotateAngle; // 회전 각도
    public float shadowBallSpeed; // 섀도우볼 속도
    public float shadowBallDistance; // 섀도우볼 사거리

    public Transform bulletPos; // 섀도우볼의 발사 위치
    public Transform axisPos; // 섀도우볼의 발사 방향

    // 발사체 관련 - 델리알 검
    BossSword[] bs; // 검 스크립트 배열

    public float bossSword_shotTime; // 검을 발사할 시간
    public float bossSwordSpeed; // 검 속도
    public float bossSwordlDistance; // 검 사거리
    public int bossSword_Damage; // 검 대미지

    public Transform[] bossSword_BulletPos; // 검의 발사 위치
    public Transform bossSword_axisPos; // 검의 발사 방향

    // 코루틴 변수
    IEnumerator laserAttackCoroutine; // 레이저 패턴
    IEnumerator setShadowBallCoroutine; // 섀도우볼 패턴
    IEnumerator swordAttackCoroutine; // 검 패턴

    // 문 스크립트
    public StoneDoor leftDoor; // 왼쪽 문 스크립트
    public StoneDoor rightDoor; // 오른쪽 문 스크립트

    private void Start()
    {
        // 플레이어 위치 할당
        targetTransform = GameObject.FindWithTag("Player").transform;

        // 배열 초기화
        sb = new ShadowBall[shotShadowBallNum * 4 + 4];
        bs = new BossSword[bossSword_BulletPos.Length];

        // 코루틴 변수 할당
        laserAttackCoroutine = LaserAttack();
        setShadowBallCoroutine = SetShadowBall();
        swordAttackCoroutine = SwordAttack();
    }

    private void Update()
    {
        // 전투가 시작되지 않았다면 플레이어 감지를 시도하고 리턴
        if (!isBattleStart)
        {
            PlayerSensor(); // 플레이어 감지 함수
            return;
        }

        SetPattern(); // 공격 패턴 선택 함수
    }

    // 공격 패턴 설정 함수
    public void SetPattern()
    {
        if (!isSetPattern) // 다음 공격 패턴을 정하지 않았다면
        {
            float rand_Pattern = Random.Range(0f, 1f); // 랜덤 공격 패턴을 사용하기 위한 랜덤 수 구하기

            if (rand_Pattern < 0.45f) // 레이저 공격 (45%) 
            {
                // 레이저 공격 코루틴 실행
                StartCoroutine("LaserAttack");
            }
            else if (rand_Pattern < 0.6f) // 탄환 발사 (15%) 
            {
                // 탄환 발사 코루틴 실행
                StartCoroutine("BulletAttack");
            }
            else if (rand_Pattern < 1.0f) // 대검 소환 (40%) 
            {
                // 대검 소환 코루틴 실행
                StartCoroutine("SwordAttack");
            }

            isSetPattern = true; // 공격 패턴 정함 체크
        }
    }

    // 플레이어 감지 함수
    public void PlayerSensor()
    {
        // 인식 범위 내에 플레이어를 찾음
        Collider2D player = Physics2D.OverlapCircle(attackTransform[0].position, attackRadius[0], LayerMask.GetMask("Player"));

        if (player != null && !isFindPlayer) // 플레이어가 존재하고 전투 중이 아니라면
        {
            // 전투시작
            Debug.Log("플레이어 감지! 전투 시작");
            isFindPlayer = true;
            skellBoss_Anim.SetBool("isBattle", true);
            SoundManager.instance.PlaySound("SkellBossLaugh"); // 보스 등장 사운드 재생
            SoundManager.instance.PlayBackgroundMusic("Boss"); // 보스 전투 BGM 재생

            GameManager.instance.dontMove = true; // 플레이어 움직임 불가능

            // HP바 활성화
            Invoke("Show_HpBar", 2f);

            director_Show.Play(); // 보스 등장 컷씬 재생

            // 카메라 전환 - 전투 카메라
            Invoke("ChangeBattaleCam", 5.5f);

            // 문 닫기
            leftDoor.DoorClose();
            rightDoor.DoorClose();
        }
    }

    // 대미지 적용 함수
    public override void TakeDamage(int damage, Transform Pos, bool isCritical)
    {
        // 죽었거나 전투가 시작되지 않았다면 리턴
        if (isDie || !isBattleStart)
        {
            return;
        }

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
        isDie = true; // 몬스터 사망

        // 공격 패턴 정지
        StopCoroutine(laserAttackCoroutine);
        StopCoroutine(setShadowBallCoroutine);
        StopCoroutine(swordAttackCoroutine);

        GameManager.instance.canHitPlayer = false; // 플레이어 무적

        // 죽는 사운드
        SoundManager.instance.PlaySound("EnemyDie");

        skellBoss_Anim.SetBool("isBattle", false);
        SoundManager.instance.StopBackgroundMusic();
        isBattleStart = false;

        // HP바 비활성화
        hp_Bar.SetActive(false);

        // 컷씬 재생
        director_BossDie.Play();

        // 사망 연출 코루틴 재생
        StartCoroutine("DieEffect");
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
            Debug.Log("금괴 드롭"); // 보스는 금괴만 드롭
            GameObject bullion = ObjectPoolingManager.instance.GetObject("Item_Bullion"); // 오브젝트 풀에서 금괴 대여
            bullion.transform.position = this.transform.position; // 위치 초기화
        }
    }

    // 체력바 활성화 함수
    public void Show_HpBar()
    {
        hp_Bar.SetActive(true); // HP 바 활성화
    }

    // 카메라 전환 함수 ( 전투 캠으로 전환 )
    public void ChangeBattaleCam()
    {
        // 카메라 전환 - 전투 카메라
        defaultCam.SetActive(false);
        battleCam.SetActive(true);

        GameManager.instance.dontMove = false; // 플레이어 움직임 가능
    }

    // 레이저 공격 코루틴
    IEnumerator LaserAttack()
    {
        int rand_ShotNum = Random.Range(min_LaserShotNum - 1, max_LaserShotNum - 1);

        for (int i = 0; i < rand_ShotNum + 1; i++)
        {
            // 죽었다면 종료
            if (isDie)
            {
                yield break;
            }

            // 어느 손인지 체크
            if (isLeftHand)
            {
                // 손 위치 이동
                leftHand.SetHandPos();

                yield return new WaitForSeconds(0.5f);

                // 공격 애니메이션 재생
                leftHand.anim.SetTrigger("Attack");

                yield return new WaitForSeconds(0.75f);

                // 레이저 발사 함수 실행
                leftHand.laser.LaserShot();
            }
            else
            {
                // 손 위치 이동
                rightHand.SetHandPos();

                yield return new WaitForSeconds(0.5f);

                // 공격 애니메이션 재생
                rightHand.anim.SetTrigger("Attack");

                yield return new WaitForSeconds(0.75f);

                // 레이저 발사 함수 실행
                rightHand.laser.LaserShot();
            }

            isLeftHand = !isLeftHand; // 손 변경
        }

        yield return new WaitForSeconds(1f);

        isSetPattern = false; // 공격 패턴 정함 체크 해제
    }

    // 탄환 발사 함수
    IEnumerator BulletAttack()
    {
        // 입 벌리기
        head_Anim.SetBool("isAttack", true);

        yield return new WaitForSeconds(0.5f);

        // 탄환 발사
        StartCoroutine("SetShadowBall");

        yield return new WaitForSeconds(shotTime);

        // 입 닫기
        head_Anim.SetBool("isAttack", false);

        yield return new WaitForSeconds(1f);

        isSetPattern = false; // 공격 패턴 정함 체크 해제
    }

    // 대검 소환 함수
    IEnumerator SwordAttack()
    {
        // 검 생성
        for (int i = 0; i < bossSword_BulletPos.Length; i++)
        {
            // 죽었다면 종료
            if (isDie)
            {
                yield break;
            }

            SoundManager.instance.PlaySound("CreateSword");

            // 오브젝트 풀에서 섀도우볼 프리팹 대여
            GameObject bossSword = ObjectPoolingManager.instance.GetObject("Bullet_BossSword");
            bs[i] = bossSword.GetComponent<BossSword>();

            bs[i].speed = bossSwordSpeed; // 검 속도
            bs[i].distance = bossSwordlDistance; // 검 사거리
            bs[i].targetTransform = targetTransform; // 플레이어 위치
            bs[i].bulletPos = bossSword_BulletPos[i]; // 검 발사 위치
            bs[i].shooterPos = transform; // 발사한 객체의 위치
            bs[i].damage = bossSword_Damage; // 검 공격력
            bs[i].failCause = "델리알에게 패배"; // 사망 사유

            bs[i].createAnim.SetTrigger("Create"); // 검 생성 애니메이션 재생
            bs[i].chargeAnim.SetBool("isCharge", true); // 검 충전 애니메이션 재생

            bs[i].Setting(); // 세팅

            yield return new WaitForSeconds(bossSword_shotTime / bossSword_BulletPos.Length);
        }

        yield return new WaitForSeconds(1f);

        // 검 발사
        for (int j = 0; j < bossSword_BulletPos.Length; j++)
        {
            // 죽었다면 종료
            if (isDie)
            {
                // 발사 대기 중인 검 모두 제거
                for (int k = 0; k < bossSword_BulletPos.Length; k++)
                {
                    bs[k].gameObject.SetActive(false);
                }

                yield break;
            }

            bs[j].Shot(); // 발사

            bs[j].anim.SetBool("isShot", true); // 샷 애니메이션 재생
            bs[j].chargeAnim.SetBool("isCharge", false); // 충전 애니메이션 끄기

            yield return new WaitForSeconds(0.275f);
        }


        yield return new WaitForSeconds(1f);

        isSetPattern = false; // 공격 패턴 정함 체크 해제
    }

    // 새도우볼 세팅 함수
    IEnumerator SetShadowBall()
    {
        // 섀도우볼 생성
        for (int i = 0; i <= shotShadowBallNum; i++)
        {
            // 죽었다면 종료
            if (isDie)
            {
                yield break;
            }

            // 4방향으로 생성
            for (int k = 0; k < 4; k++)
            {
                // 오브젝트 풀에서 섀도우볼 프리팹 대여
                GameObject shadowBall = ObjectPoolingManager.instance.GetObject("Bullet_ShadowBall");
                sb[i * 4 + k] = shadowBall.GetComponent<ShadowBall>();

                // 섀도우볼 변수 전달
                sb[i * 4 + k].speed = shadowBallSpeed; // 섀도우볼 속도
                sb[i * 4 + k].distance = shadowBallDistance; // 섀도우볼 사거리
                sb[i * 4 + k].aimPos = axisPos; // 섀도우볼 발사 방향
                sb[i * 4 + k].bulletPos = bulletPos; // 섀도우볼 발사 위치
                sb[i * 4 + k].shooterPos = transform; // 발사한 객체의 위치
                sb[i * 4 + k].damage = attackDamage; // 공격력
                sb[i * 4 + k].failCause = "델리알에게 패배"; // 사망 사유

                // 파이어볼 세팅 후 발사
                sb[i * 4 + k].Setting();
                sb[i * 4 + k].Shot();

                axisPos.transform.Rotate(new Vector3(0, 0, 90)); // 4방향으로 쏘기 위한 회전
            }

            SoundManager.instance.PlaySound("ShadowBallShot"); // 탄환 발사 사운드 재생

            // 잠시 후
            yield return new WaitForSeconds(shotTime / shotShadowBallNum);

            axisPos.transform.Rotate(new Vector3(0, 0, rotateAngle)); // 회전
        }

        axisPos.rotation = new Quaternion(0, 0, 0, 0); // 방향 초기화   
    }

    // 사망 연출
    IEnumerator DieEffect()
    {
        GameManager.instance.dontMove = true; // 플레이어 움직임 제어

        Time.timeScale = 0.5f; // 슬로우 모션 효과
        Debug.Log("슬로우");

        yield return new WaitForSecondsRealtime(5f); // 실제 시간으로 5초 후

        Time.timeScale = 1.0f; // 슬로우 모션 효과 해제
        Debug.Log("슬로우 해제");

        bossDie_Particle.Play();

        // 파티클이 재생되는 동안 0.25초 마다 사운드 재생
        while (bossDie_Particle.isPlaying)
        {
            SoundManager.instance.PlaySound("EnemyDie");

            yield return new WaitForSeconds(0.25f);
        }

        bossDieBoom_Particle.Play();
        SoundManager.instance.PlaySound("EnemyDie");

        // 보스 해골 소환
        GameObject bossSkull = Instantiate(bossSkullPrefab);
        bossSkull.transform.position = bulletPos.position;

        // 보스 사망 애니메이션
        skellBoss_Anim.SetTrigger("Die");

        yield return new WaitForSeconds(2.5f);

        GameManager.instance.dontMove = false;

        // 카메라 전환 - 기본 카메라
        defaultCam.SetActive(true);
        battleCam.SetActive(false);

        // 플레이어 무적 해제
        GameManager.instance.canHitPlayer = true;

        // [사망 처리]
        GameManager.instance.monsterInStageList.RemoveAt(GameManager.instance.monsterInStageList.Count - 1);

        // 몬스터 리스트의 수가 0이 되었다면
        if (GameManager.instance.monsterInStageList.Count == 0)
        {
            // 포탈 열림 사운드 재생
            SoundManager.instance.PlaySound("OpenPortal");
        }

        // 보상
        DropItem(); // 아이템 드롭

        // 문 열기
        leftDoor.DoorOpen();
        rightDoor.DoorOpen();
    }
}
