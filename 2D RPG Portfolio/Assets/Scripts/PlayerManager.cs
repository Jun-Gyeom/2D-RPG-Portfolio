using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : EntityManager
{
    Rigidbody2D rb;
    Animator anim;

    public float jumpPower; // 점프력
    public int jumpCount; // 공중에서 점프한 횟수
    public int maxJump; // 최대 점프 횟수
    private bool isGround; // 바닥에 닿아있는지 체크
    public bool isWall; // 벽에 닿아있는지 체크
    private bool isWallJump; // 벽 점프를 하는 중인지 체크
    public float wallSlideSpeed; // 벽에 붙어있을 때 미끄러져 내려오는 속도
    public int attackCombo; // 몇 번째 공격인지 구분

    public float dashDistance; // 대쉬 거리
    public float dashCooldown; // 대쉬 쿨타임
    public int maxDashChargeCount; // 최대 대쉬 충전량
    public int dashChargeCount; // 대쉬 충전량
    private float timer_Dash; // 대쉬 타이머

    // UI 관련
    public GameObject hp_Bar; // HP 바 게임오브젝트
    public GameObject hp_fill_GameObject; // HP 바 fill 영역 게임오브젝트
    public GameObject hp_fill_Lerp_GameObject; // HP 바 부드러운 fill 영역 게임오브젝트
    public Image hp_fill; // HP 바 fill
    public Image hp_fill_Lerp; // HP 바 부드러운 fill

    public GameObject exp_Bar; // EXP 바 게임오브젝트
    public GameObject exp_fill_GameObject; // EXP 바 fill 영역 게임오브젝트
    public GameObject exp_fill_Lerp_GameObject; // EXP 바 부드러운 fill 영역 게임오브젝트
    public Image exp_fill; // EXP 바 fill
    public Image exp_fill_Lerp; // EXP 바 부드러운fill

    public GameObject dashGauge1; // 대쉬 게이지 첫 번째 칸
    public GameObject dashGauge2; // 대쉬 게이지 두 번째 칸


    Transform attackPos; // 공격받았을 때 상대가 공격한 위치

    // Ground 체크 박스캐스트
    private Vector2 boxCastSize = new Vector2(0.6f, 0.8f);
    private float boxCastMaxDistance = 0.5f;

    // Wall 체크 레이캐스트
    private float wallCastMaxDistance = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        dashChargeCount = maxDashChargeCount; // 대쉬 풀충전
        timer_Dash = dashCooldown; // 대쉬 타이머 초기화

        // HP 바 할당
        hp_Bar = GameObject.Find("Player_HP_Bar");
        hp_fill_GameObject = GameObject.Find("Player_HP_fill");
        hp_fill_Lerp_GameObject = GameObject.Find("Player_HP_fill_Lerp");

        hp_fill = hp_fill_GameObject.GetComponent<Image>(); // HP 바 fill 영역 이미지 할당
        hp_fill_Lerp = hp_fill_Lerp_GameObject.GetComponent<Image>(); // HP 바 부드러운 fill 영역 이미지 할당

        // EXP 바 할당
        exp_Bar = GameObject.Find("Player_EXP_Bar");
        exp_fill_GameObject = GameObject.Find("Player_EXP_fill");
        exp_fill_Lerp_GameObject = GameObject.Find("Player_EXP_fill_Lerp");

        exp_fill = exp_fill_GameObject.GetComponent<Image>(); // EXP 바 fill 영역 이미지 할당
        exp_fill_Lerp = exp_fill_Lerp_GameObject.GetComponent<Image>(); // EXP 바 부드러운 fill 영역 이미지 할당

        // 대쉬 게이지 할당
        dashGauge1 = GameObject.Find("Gauge 1");
        dashGauge2 = GameObject.Find("Gauge 2");
    }

    void Update()
    {
        // 선형보간 HP, EXP 바
        hp_fill_Lerp.fillAmount = Mathf.Lerp(hp_fill_Lerp.fillAmount, hp_fill.fillAmount, Time.deltaTime * 7.5f);
        exp_fill_Lerp.fillAmount = Mathf.Lerp(exp_fill_Lerp.fillAmount, exp_fill.fillAmount, Time.deltaTime * 7.5f);

        hp_fill.fillAmount = health / maxHealth; // HP 바 업데이트
        exp_fill.fillAmount = 0; // (임시 exp 바 업데이트) 현재 경험치 / 다음 레벨까지의 경험치

        // 죽었다면 Return
        if (GameManager.instance.isDie)
        {
            return;
        }

        // 점프
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // S키를 누르고 있거나 대쉬 중이거나 벽 점프 중이라면 점프하지 않도록 함
            if (Input.GetKey(KeyCode.S) 
                || isWallJump
                || (anim.GetCurrentAnimatorStateInfo(0).IsName("Dash") && (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f || anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f))
                || (anim.GetCurrentAnimatorStateInfo(0).IsName("Air_Dash") && (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f || anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)))
            {
                return;
            }

            // 벽 타기 중이라면
            if (isWall && !isGround && !isWallJump)
            {
                isWallJump = true;
                //rb.velocity = new Vector2(transform.localScale.x * -5f, jumpPower * 0.95f);

                // 벽 점프
                rb.velocity = new Vector2(0, 0); // 속도 초기화
                rb.AddForce(new Vector2(transform.localScale.x * -4.5f, jumpPower), ForceMode2D.Impulse);

                // 방향 전환
                if (transform.localScale.x > 0) // 오른쪽을 보고 있는 경우
                {
                    // 왼쪽으로 방향 전환
                    Vector3 scale = transform.localScale; // 플레이어의 스케일 값
                    scale.x = -Mathf.Abs(scale.x); // 스케일의 x값의 -절댓값
                    transform.localScale = scale; // 플레이어의 스케일 값을 
                }
                else if (transform.localScale.x < 0) // 왼쪽을 보고 있는 경우
                {
                    // 오른쪽으로 방향 전환
                    Vector3 scale = transform.localScale; // 플레이어의 스케일 값
                    scale.x = Mathf.Abs(scale.x); // 스케일의 x값의 -절댓값
                    transform.localScale = scale; // 플레이어의 스케일 값을 
                }
            }
            else
            {
                if (jumpCount < maxJump)
                {
                    // 점프
                    Jump();
                    jumpCount++; // 점프 횟수 증가
                }
            }
        }

        // 점프 애니메이션 매개변수
        anim.SetFloat("y_Velocity", rb.velocity.y);

        // 공격
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 대쉬 중이거나 벽 타기 중이라면 공격 X
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Dash") && (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f || anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                    || (anim.GetCurrentAnimatorStateInfo(0).IsName("Air_Dash") && (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f || anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                    || (anim.GetCurrentAnimatorStateInfo(0).IsName("Wall_Slide") && (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f || anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f))))
                {
                    return;
                }
                else
                {
                    Attack();
                }
            }
        }

        // 콤보가 0이 아닐 때
        if (attackCombo != 0)
        {
            // 공격 준비시간 + 0.5f만큼의 시간이 지났다면
            if (Time.time >= nextAttackTime + 0.5f)
            {
                // 콤보를 0으로 초기화
                attackCombo = 0;
            }
        }

        // 대쉬
        if (Input.GetMouseButtonDown(1) && dashChargeCount > 0)
        {
            // 공격 중이거나 벽에 매달려있다면 대쉬 안함
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f || anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                || anim.GetCurrentAnimatorStateInfo(0).IsName("Wall_Slide") && (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f || anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f))
            {
                return;
            }
            else
            {
                Dash();
            }
        }

        // 대쉬 타이머
        if ((timer_Dash > 0) && !(dashChargeCount == maxDashChargeCount)) // 타이머가 완료되지 않았고, 대쉬가 최대로 충전되지 않았다면
        {
            // 타이머 실행
            timer_Dash -= Time.deltaTime;
        }
        // 타이머가 0이면
        else if (timer_Dash <= 0)
        {
            // 대쉬 충전량이 최대 충전량보다 적다면
            if (dashChargeCount < maxDashChargeCount)
            {
                dashChargeCount++; // 대쉬 1충전
                timer_Dash = dashCooldown; // 타이머 초기화
            }
        }

        // 대쉬 게이지
        dashGauge1.SetActive(false);
        dashGauge2.SetActive(false);

        if (dashChargeCount == 2) // 대쉬가 2개 충전돼있다면
        {
            dashGauge1.SetActive(true);
            dashGauge2.SetActive(true);
        }
        else if (dashChargeCount == 1) // 대쉬가 1개 충전돼있다면
        {
            dashGauge1.SetActive(true);
        }

        // 벽 타기
        if (isWall && !isGround && rb.velocity.y <= 0)
        {
            isWallJump = false;
            // y속도에 미끄러져 내려오는 속도를 곱해줌으로 느리게 흘러내리도록 함
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * wallSlideSpeed);
        }
    }
    void FixedUpdate()
    {
        // 죽었다면 Return
        if (GameManager.instance.isDie)
        {
            return;
        }

        GroundCheck(); // 땅에 있는지 체크
        WallCheck(); // 벽에 붙어있는지 체크
        Move(); // 플레이어 이동
    }

    // 이동 함수
    void Move()
    {
        // 공격 중이거나 맞는 중이거나 대쉬 중이거나 벽 점프 중이라면 움직이지 않기
        if (isWallJump
            || (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f || anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            || (anim.GetCurrentAnimatorStateInfo(0).IsName("Hit") && (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f || anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            || (anim.GetCurrentAnimatorStateInfo(0).IsName("Dash") && (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f || anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            || (anim.GetCurrentAnimatorStateInfo(0).IsName("Air_Dash") && (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f || anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f))))))
        {
            return;
        }

        float h = Input.GetAxisRaw("Horizontal") * moveSpeed;
        rb.velocity = new Vector2(h, rb.velocity.y);

        // 플레이어 보는 방향
        if (h < 0)
        {
            // 왼쪽
            Vector3 scale = transform.localScale; // 플레이어의 스케일 값
            scale.x = -Mathf.Abs(scale.x); // 스케일의 x값의 -절댓값
            transform.localScale = scale; // 플레이어의 스케일 값을 
        }
        else if (h > 0)
        {
            // 오른쪽
            Vector3 scale = transform.localScale; // 플레이어의 스케일 값
            scale.x = Mathf.Abs(scale.x); // 스케일의 x값의 절댓값
            transform.localScale = scale; // 플레이어의 스케일 값을 재정의
        }

        // 가만히 있을 때
        if (h == 0)
        {
            // 이동 애니메이션
            anim.SetBool("isMove", false);
        }
        else
        {
            // 이동 애니메이션
            anim.SetBool("isMove", true);
        }
    }

    // 점프 함수
    void Jump()
    {
        // 점프
        rb.velocity = Vector2.up * jumpPower;
    }

    // 공격 함수
    void Attack()
    {
        anim.SetFloat("AttackCombo", attackCombo);
        anim.SetTrigger("Attack");

        // 공격이 진행되기 전
        if (!isGround) // 공중에 있을 떄의 공격
        {
            // 공중 공격을 뜻하는 콤보3
            attackCombo = 3;
        }

        Collider2D[] monsters = Physics2D.OverlapCircleAll(attackTransform[attackCombo].position, attackRadius[attackCombo], LayerMask.GetMask("Monster"));

        if (monsters != null)
        {
            foreach (Collider2D monster in monsters)
            {
                attackPos = transform;
                monster.GetComponent<MonsterManager>().TakeDamage(attackDamage, attackPos);
            }
        }

        // 공격이 끝난 이후
        if (attackCombo >= 2) // 콤보가 최대 (2)보다 크거나 같으면
        {
            // 콤보 초기화
            attackCombo = 0;
        }
        else
        {
            // 콤보 단계 증가
            attackCombo++;
        }

        nextAttackTime = Time.time + attackCoolDown;

    }

    // 대쉬
    void Dash()
    {
        // 이미 대쉬 중이라면 대쉬하지 않기
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Dash") && (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f || anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            || (anim.GetCurrentAnimatorStateInfo(0).IsName("Air_Dash") && (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f || anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f))))
        {
            return;
        }

        dashChargeCount--; // 1대쉬 사용

        anim.SetTrigger("Dash"); // 대쉬 애니메이션
        rb.velocity = new Vector2(transform.localScale.x * dashDistance, rb.velocity.y); // 대쉬
    }

    // 플레이어가 대미지를 입는 함수
    public override void TakeDamage(int damage, Transform Pos)
    {
        if (anim.GetBool("isDeath"))
            return;

        base.TakeDamage(damage, Pos);

        anim.SetTrigger("Hit");

        // 넉백
        float x = transform.position.x - Pos.position.x; // 밀려날 방향
        if (x > 0)
        {
            Debug.Log("test");
            rb.velocity = new Vector2(3f, rb.velocity.y); // 오른쪽으로 3만큼 넉백 
        }
        else if (x < 0)
        {
            Debug.Log("test");
            rb.velocity = new Vector2(-3f, rb.velocity.y); // 왼쪽으로 3만큼 넉백 // 무기 밀치기 값도 받아오면 무기마다 밀치기 다르게 가능
        }
    }

    // 플레이어가 죽는 함수
    public override void Die()
    {
        anim.SetBool("isDeath", true);
        GameManager.instance.isDie = true; // 플레이어 사망
    }

    // 바닥이 있는지 체크하는 함수
    private void GroundCheck()
    {
        isGround = false;

        RaycastHit2D rayHit = Physics2D.BoxCast(this.transform.position, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, LayerMask.GetMask("Platform"));
        if (rayHit.collider != null)
        {
            isGround = true;
            jumpCount = 0; // 점프 횟수 초기화

            isWallJump = false; // 벽 점프 상태 아님
        }

        // 점프 애니메이션
        anim.SetBool("isJump", !isGround);
    }

    // 벽에 붙어있는지 체크하는 함수
    private void WallCheck()
    {
        isWall = false;

        RaycastHit2D rayHit = Physics2D.Raycast(this.transform.position, transform.localScale, wallCastMaxDistance, LayerMask.GetMask("Platform"));
        if (rayHit.collider != null)
        {
            Debug.Log("벽 있음");
            isWall = true; // 벽 체크
        }

        // 벽 타기 애니메이션
        anim.SetBool("isWall", isWall);
    }

    // 기즈모를 그려주는 함수
    private void OnDrawGizmos()
    {
        // 그라운드 체크 기즈모
        RaycastHit2D groundRayHit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, LayerMask.GetMask("Platform"));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + Vector3.down * groundRayHit.distance, boxCastSize);
    }
}
