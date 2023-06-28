using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 싱글톤을 할당할 전역변수
    public Portal pt; // 포탈 스크립트
    public Inventory iv; // 인벤토리 스크립트
    public TownShop town_Shop; // 마을 상점 스크립트
    public DungeonShop dungeon_Shop; // 던전 상점 스크립트
    public GameObject fairySpawnPoint; // 페어리 스폰포인트 오브젝트

    public bool isDropFairy; // 현재 스테이지에 페어리를 소환했는지 체크

    public GameObject player_Prefab; // 플레이어 프리팹

    public GameObject player; // DontDestroy에 등록할 캔버스 오브젝트

    private GameObject[] monsterInStage; // 현재 스테이지에 있는 몬스터 배열
    public List<GameObject> monsterInStageList; // 현재 스테이지에 있는 몬스터 리스트

    public List<string> stageSceneList; // 던전(스테이지) 씬 리스트
    public List<string> randomStageSceneList; // 랜덤으로 뽑을 던전 씬 리스트

    public string playerLocation; // 플레이어의 위치
    public int currentDungeonFloor; // 현재 던전 층
    public int highDungeonFloor; // 도달한 층
    public float playTime; // 게임 플레이 시간
    public float playDungeonTime; // 던전에 머문 시간
    public float saveTime; // 자동 저장 시점

    public bool isInGame; // 인게임인지 체크

    // UI 관련
    public GameObject hud_CanvasObject; // HUD 캔버스 오브젝트

    public TMP_Text goldText; // 보유한 골드량 텍스트 (좌측 상단)

    public GameObject mainMenuPanel; // 메인메뉴 창 오브젝트
    public GameObject dataSlotSelectPanel; // 메인메뉴 데이터 슬롯 선택 패널 오브젝트
    public GameObject invectoryPanel; // 인벤토리 오브젝트
    public GameObject tooltipObject; // 툴팁 오브젝트
    public GameObject deathMenu; // 죽음 메뉴 오브젝트
    public GameObject escMenuPanel; // 일시정지 메뉴 오브젝트
    public GameObject townEscMenuPanel; // 마을 일시정지 메뉴 오브젝트
    public GameObject optionMenuPanel; // 설정 메뉴 오브젝트
    public GameObject townShopPanel; // 마을 상점 창 오브젝트
    public GameObject dungeonShopPanel; // 던전 상점 창 오브젝트

    public bool activeInventoty; // 인벤토리의 활성 여부
    public bool activeEscMenu; // 일시정지 메뉴 활성 여부
    public bool activeShopPanel; // 상점 창 활성 여부

    public GameObject uiTooltipObject; // UI 툴팁 오브젝트

    public GameObject realyEscapeDungeonPanel; // 던전을 나갈 것인지 되묻는 창 오브젝트
    public GameObject realyGoMainPanel; // 메인화면으로 갈 것인지 되묻는 창 오브젝트

    public TMP_Text townLocationText; // 마을 일시정지 메뉴플레이어 현재 위치 텍스트
    public TMP_Text townEscGoldText; // 마을 일시정지 메뉴 소지금 텍스트
    public TMP_Text locationText; // 일시정지 메뉴플레이어 현재 위치 텍스트
    public TMP_Text escGoldText; // 일시정지 메뉴 소지금 텍스트
    public TMP_Text playtimeText; // 일시정지 메뉴 시간 텍스트
    public TMP_Text enemyText; // 일시정지 메뉴 남은 적 텍스트

    public GameObject mouseCursor; // 마우스 커서 오브젝트

    // 대쉬 게이지 관련
    public GameObject dashGauge1; // 대쉬 게이지 첫 번째 칸
    public GameObject dashGauge2; // 대쉬 게이지 두 번째 칸
    public GameObject dashGauge3; // 대쉬 게이지 세 번째 칸
    public GameObject dashGauge4; // 대쉬 게이지 네 번째 칸
    public GameObject dashGauge5; // 대쉬 게이지 다섯 번째 칸
    public GameObject dashGauge6; // 대쉬 게이지 여섯 번째 칸

    public GameObject dashFrame3; // 대쉬 세 번째 프레임
    public GameObject dashFrame4; // 대쉬 네 번째 프레임
    public GameObject dashFrame5; // 대쉬 다섯 번째 프레임
    public GameObject dashFrame6; // 대쉬 여섯 번째 프레임
    public GameObject gaugeLastFrame; // 대쉬 게이지 프레임 닫기

    // 사망 정보창 관련
    public TMP_Text deathMenu_TimeText; // 시간 텍스트
    public TMP_Text deathMenu_LocationText; // 위치 텍스트
    public TMP_Text deathMenu_GoldText; // 소지금 텍스트
    public TMP_Text deathMenu_DeathCauseText; // 사망 이유 텍스트
    public TMP_Text deathMenu_NextExpText; // 다음 레벨까지 남은 경험치 텍스트
    public GameObject adventureFailPanel; // 탐험 실패 패널 오브젝트

    // 게임 클리어 창 관련
    public GameObject goldPenaltyText; // 패널티 텍스트 오브젝트
    public TMP_Text clearMenu_GameClearTitleText; // 게임 클리어 (사망 이유) 제목 텍스트
    public TMP_Text clearMenu_ComentTitleText; // 개발자 코멘트 (다음 레벨까지) 제목 텍스트
    public GameObject adventureClearPanel; // 탐험 성공 패널 오브젝트

    // 대화창 관련
    public GameObject talkPanel; // 대화창 패널
    public GameObject choicePanel; // 대화 선택 창
    public GameObject choiceBtn_Talk; // 대화 선택 버튼 - 이야기 하기
    public GameObject choiceBtn_TownShop; // 대화 선택 버튼 - 마을 상점
    public GameObject choiceBtn_DungeonShop; // 대화 선택 버튼 - 던전 상점
    public TMP_Text npcNameText; // NPC 이름 텍스트
    public TypingEffect talkText; // 대화 텍스트
    public int talkIndex; // 대사 번호
    public bool isTalk; // 현재 대화 중인지 체크
    public bool isChoiceTalk; // 현재 대화 선택 중인지 체크
    public int currentChoiceTalkIndex; // NPC에게서 전달받을 대화 선택창이 나올 대화 인덱스
    public int currentNpcId; // 현재 대화 중인 NPC ID
    public string currentNpcName; // 현재 대화 중인 NPC 이름

    // 설정 관련
    public List<Resolution> resolutionList = new List<Resolution>(); // 해상도 리스트
    public FullScreenMode[] displayModeList; // 화면 모드

    private int displayModePage; // 현재 화면 모드 페이지
    private int resolutionPage; // 현재 해상도 페이지
    public TMP_Text displayModeText; // 현재 화면 모드 텍스트
    public TMP_Text resolutionText; // 현재 해상도 텍스트

    // 아이템 툴팁 관련
    public RectTransform tooltip_rt; // 툴팁 렉트 트랜스폼

    public TMP_Text tooltip_ItemName; // 툴팁 아이템 이름
    public TMP_Text tooltip_ItemDescription; // 툴팁 아이템 설명
    public Image tooltip_ItemImage; // 툴팁 아이템 이미지

    public GameObject tooltip_IncreaseMaxHealthObject; // 최대 체력 증가량 오브젝트
    public GameObject tooltip_IncreaseAttackDamageObject; // 공격력 증가량 오브젝트
    public GameObject tooltip_IncreaseMoveSpeedObject; // 이동 속도 증가량 오브젝트
    public GameObject tooltip_IncreaseJumpPowerObject; // 점프력 증가량 오브젝트
    public GameObject tooltip_IncreaseCriticalPercentageObject; // 크리티컬 확률 증가량 오브젝트
    public GameObject tooltip_IncreaseCriticalValueObject; // 크리티컬 계수 증가량 오브젝트
    public GameObject tooltip_IncreaseJumpCountObject; // 점프 횟수 증가량 오브젝트
    public GameObject tooltip_IncreaseDashCountObject; // 대쉬 횟수 증가량 오브젝트

    public TMP_Text tooltip_IncreaseMaxHealthText; // 최대 체력 증가량 텍스트
    public TMP_Text tooltip_IncreaseAttackDamageText; // 공격력 증가량 텍스트
    public TMP_Text tooltip_IncreaseMoveSpeedText; // 이동 속도 증가량 텍스트
    public TMP_Text tooltip_IncreaseJumpPowerText; // 점프력 증가량 텍스트
    public TMP_Text tooltip_IncreaseCriticalPercentageText; // 크리티컬 확률 증가량 텍스트
    public TMP_Text tooltip_IncreaseCriticalValueText; // 크리티컬 계수 증가량 텍스트
    public TMP_Text tooltip_IncreaseJumpCountText; // 점프 횟수 증가량 텍스트
    public TMP_Text tooltip_IncreaseDashCountText; // 대쉬 횟수 증가량 텍스트

    // 페이드 인아웃 관련 + 씬 전환
    public Image fadePanel; // 페이드인아웃 패널 오브젝트
    public float fadeTime; // 페이드 되는 시간
    public bool isFade; // 현재 페이드 중인지 체크
    public bool canSceneLoad; // 페이드가 완료되어 씬을 불러와야 하는 상태인지 체크
    public string loadSceneName; // 로드할 씬 이름
    public bool isLoadScene; // 씬을 로드했는지 체크
    public AsyncOperation operation; // 씬과 관련된 정보를 얻기위한 클래스

    // 플레이어 관련
    public bool isDie; // 플레이어가 죽었는지
    public bool dontMove; // 플레이어 움직임 제어 체크
    public bool canHitPlayer; // 플레이어가 공격당할 수 있는지 체크 (무적시간)

    public int level; // 플레이어 레벨
    public int maxLevel; // 플레이어 최대 레벨
    public int[] levelUp_exp; // 레벨업에 필요한 경험치량 배열

    public float gold; // 골드
    public float lerpGold; // 골드텍스트 러프를 위한 변수
    public float exp; // 경험치

    public string failCause; // 탐험 실패 이유

    public bool tutorial_Complete; // 튜토리얼 완료 여부
    public bool isTutorial; // 현재 튜토리얼 진행 중인지 여부


    // 플레이어 능력치

    // 레벨에 따라 변화함
    public int[] player_AttackDamage; // 공격력
    public float[] player_MaxHealth; // 최대 체력

    // 기본 고정 수치
    public float jumpPower;// 점프력
    public int maxJump; // 점프횟수
    public int maxDashChargeCount; // 대쉬 횟수
    public float player_MoveSpeed; // 이동속도    
    public float critical_Percentage; // 크리티컬 확률
    public float critical_Value; // 크리티컬 대미지 배율

    // 아이템 등으로 인해 증가되는 추가 능력치
    public float increased_MaxHealth; // 추가 최대 체력
    public int increased_AttackDamage; // 추가 공격력
    public float increased_JumpPower; // 추가 점프력
    public int increased_MaxJump; // 추가 점프 횟수
    public int increased_MaxDashCount; // 추가 대쉬 횟수
    public float increased_MoveSpeed; // 추가 이동 속도
    public float increased_CriticalPercentage; // 추가 크리티컬 확률
    public float increased_CriticalValue; // 추가 크리티컬 계수

    // 싱글톤 패턴
    private void Awake()
    {
        // 싱글톤 변수가 비어있으면
        if (instance == null)
        {
            // 자신을 할당
            instance = this;
        }
        // 싱글톤 변수가 비어있지 않으면
        else
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 GameManager가 삭제되지 않음

        DontDestroyOnLoad(hud_CanvasObject); // HUD 캔버스 DontDestroy 씬으로 이동

        // 이벤트 등록
        SceneManager.sceneLoaded += LoadedsceneEvent;

        randomStageSceneList = stageSceneList.ToList(); // 던전 씬 이름 리스트 초기화

        resolutionList.AddRange(Screen.resolutions); // 화면 해상도 리스트 초기화
    }

    private void Start()
    {
        invectoryPanel.SetActive(false); // 인벤토리 패널 닫기

        // 초기화 씬에서 메인메뉴 씬으로 이동
        SceneManager.LoadScene(1);
    }

    void Update()
    {
        // 골드 러프
        lerpGold = Mathf.Lerp(lerpGold, gold, Time.deltaTime * 60f);

        // 골드 소지량 텍스트 업데이트
        goldText.text = string.Format("{0:n0} G", lerpGold); // 좌측 상단
        escGoldText.text = string.Format("{0:n0} G", lerpGold); ; // 마을 일시정지 메뉴
        townEscGoldText.text = string.Format("{0:n0} G", lerpGold); ; // 일시정지 메뉴

        // Esc 키를 눌렀을 때 설정 메뉴가 열려있었다면
        if (Input.GetKeyDown(KeyCode.Escape) && optionMenuPanel.activeSelf)
        {
            // 설정 메뉴 닫기
            optionMenuPanel.SetActive(false);
        }

        // 씬을 불러와야 할 떄
        if (canSceneLoad)
        {
            // 아직 씬 로드를 요청하지 않았다면
            if (isLoadScene == false)
            {
                Debug.Log("씬 로드 요청");

                // 비동기 방식으로 씬 로드 (로딩 완료 여부를 확인하기 위해
                operation = SceneManager.LoadSceneAsync(loadSceneName);
                isLoadScene = true; // 씬 로드 체크
            }
            // 씬 로드를 요청했다면
            else
            {
                if (operation.isDone) // 씬 로딩이 끝났다면
                {
                    StartCoroutine("FadeOut");
                }
            }
        }

        // 마을에 있으면 1분에 한번씩 자동 저장
        if (playerLocation == "마을" && playTime > saveTime)
        {
            // 다음 저장 시점 설정
            saveTime = playTime;
            saveTime += 15f;

            Save(); // 저장
        }

        // 게임 플레이 시간 업데이트
        if (isInGame)
        {
            playTime += Time.deltaTime;
        }

        // 던전에 머문 시간 업데이트
        if (playerLocation == "던전" && !isDie) // 던전에 있고 죽지 않았을 때
        {
            playDungeonTime += Time.deltaTime;
            playtimeText.text = (playDungeonTime / 60).ToString("00") + "분 " + (playDungeonTime % 60).ToString("00") + "초";
        }

        // 포탈 매니저가 할당되어있고
        if (pt != null)
        {
            // 스테이지에 모든 몬스터를 처리했다면
            if (monsterInStageList.Count == 0)
            {
                // 포탈 개방
                pt.isPortalOpen = true;

                // 남은 적 텍스트 업데이트
                enemyText.text = "없음";

                // 페어리 생성
                if (!isDropFairy && fairySpawnPoint != null)
                {
                    isDropFairy = true; // 페어리 생성 처리

                    float rand_FairySize = Random.Range(0f, 1f);

                    if (rand_FairySize < 0.30f) // None (30%) 
                    {
                        Debug.Log("Not Fairy");
                    }
                    else if (rand_FairySize < 0.70f) // M (40%) 
                    {
                        Debug.Log("Fairy M");
                        GameObject fairy = ObjectPoolingManager.instance.GetObject("Item_FairyM");
                        fairy.transform.position = fairySpawnPoint.transform.position;

                        // 페어리 생성 효과음 재생
                        SoundManager.instance.PlaySound("SpawnFairy");
                    }
                    else if (rand_FairySize < 0.95f) // L (25%) 
                    {
                        Debug.Log("Fairy L");
                        GameObject fairy = ObjectPoolingManager.instance.GetObject("Item_FairyL");
                        fairy.transform.position = fairySpawnPoint.transform.position;

                        // 페어리 생성 효과음 재생
                        SoundManager.instance.PlaySound("SpawnFairy");
                    }
                    else if (rand_FairySize < 1.0f) // XL (5%) 
                    {
                        Debug.Log("Fairy XL");
                        GameObject fairy = ObjectPoolingManager.instance.GetObject("Item_FairyXL");
                        fairy.transform.position = fairySpawnPoint.transform.position;

                        // 페어리 생성 효과음 재생
                        SoundManager.instance.PlaySound("SpawnFairy");
                    }
                }
            }
            // 스테이지에 몬스터가 남아있다면
            else
            {
                // 포탈 미개방
                pt.isPortalOpen = false;

                // 남은 적 텍스트 업데이트
                enemyText.text = monsterInStageList.Count.ToString();
            }
        }

        // 메인메뉴 씬이거나 메뉴 또는 인벤토리가 활성화 되어있거나 탐험실패, 탐험성공 패널이 활성화 되어있거나 대화 선택 중이라면
        if (SceneManager.GetActiveScene().name == "Main" || activeInventoty || activeEscMenu || choicePanel.activeSelf || deathMenu.activeSelf)
        {
            mouseCursor.SetActive(true); // 커서 숨김 해제
            Cursor.lockState = CursorLockMode.None; // 커서 고정 해제
        }
        // 메뉴 또는 인벤토리가 비활성화 되어있다면
        else
        {
            mouseCursor.SetActive(false); // 커서 숨김
            Cursor.lockState = CursorLockMode.Locked; // 커서 고정
        }
    }

    // 씬 변경 시 호출되는 메서드
    private void LoadedsceneEvent(Scene scene, LoadSceneMode mode)
    {
        // 플레이어 현재 위치 설정
        switch (scene.buildIndex)
        {
            case 1: // 메인 씬 번호 : 1
                SoundManager.instance.PlayBackgroundMusic("MainMenu"); // 메인메뉴 BGM 재생
                mainMenuPanel.SetActive(true); // 메인메뉴 활성화

                isInGame = false; // 인게임 아님

                // 플레이어 삭제
                Destroy(player);
                break;

            case 2: // 튜토리얼 씬 번호 : 2
                isInGame = true; // 인게임

                mainMenuPanel.SetActive(false); // 메인메뉴 비활성화
                dataSlotSelectPanel.SetActive(false); // 데이터 슬롯 선택 패널 비활성화

                // 플레이어가 할당되지 않았다면
                if (player == null)
                {
                    // 플레이어 생성
                    player = Instantiate(player_Prefab);
                }

                isTutorial = true; // 튜토리얼 진행 중임

                playerLocation = "튜토리얼";
                townLocationText.text = playerLocation; // 마을 일시정지 메뉴 현재 위치 텍스트 업데이트

                player.transform.position = new Vector3(-32f, 4.5f, 0); // 플레이어 위치

                // 체력 초기화
                PlayerManager.instance.health = player_MaxHealth[level];

                // 포탈 매니저가 할당되지 않았다면
                if (pt == null)
                {
                    pt = GameObject.Find("Portal").GetComponent<Portal>(); // 포탈 할당

                    StageMonsterCheck(); // 게임매니저의 스테이지 몬스터 체크 함수 실행
                }

                SoundManager.instance.PlayBackgroundMusic("Cave"); // 튜토리얼 BGM 재생

                break;

            case 3: // 마을 씬 번호 : 3
                playerLocation = "마을";

                playDungeonTime = 0; // 던전에 머문 시간 초기화
                townLocationText.text = playerLocation; // 마을 일시정지 메뉴 현재 위치 텍스트 업데이트

                isInGame = true; // 인게임
                isTutorial = false; // 튜토리얼 끝남

                mainMenuPanel.SetActive(false); // 메인메뉴 비활성화
                dataSlotSelectPanel.SetActive(false); // 데이터 슬롯 선택 패널 비활성화

                // 플레이어가 할당되지 않았다면
                if (player == null)
                {
                    // 플레이어 생성
                    player = Instantiate(player_Prefab);
                }

                player.transform.position = new Vector3(-20, -1, 0); // 플레이어 위치

                deathMenu.SetActive(false); // 게임 클리어 창 비활성화
                adventureClearPanel.SetActive(false); // 탐험 성공 패널 비활성화

                // 만약 이번 던전에서 도달한 층 수가 최대로 도달한 층 수보다 크다면
                if (currentDungeonFloor > highDungeonFloor)
                {
                    highDungeonFloor = currentDungeonFloor; // 도달한 층 기록 갱신
                }

                // 포탈 매니저가 할당되지 않았다면
                if (pt == null)
                {
                    pt = GameObject.Find("Portal").GetComponent<Portal>(); // 포탈 할당

                    StageMonsterCheck(); // 게임매니저의 스테이지 몬스터 체크 함수 실행
                }

                // 죽어서 온 것이라면
                if (isDie)
                {
                    adventureFailPanel.SetActive(false); // 탐험 실패 패널 비활성화

                    currentDungeonFloor = 0; // 던전 층 초기화

                    // 인벤토리 비우기
                    for (int i = 0; i < iv.slots.Length; i++)
                    {
                        // 슬롯에 아이템이 있다면
                        if (iv.slots[i].item != null)
                        {
                            // 비우기
                            iv.slots[i].ClearSlot();
                        }
                    }

                    // 사망 정보 창 아이템 슬롯 비활성화, 비우기
                    for (int i = 0; i < iv.deathMenuSlots.Length; i++)
                    {
                        // 슬롯이 활성화 되어있다면
                        if (iv.deathMenuSlots[i].gameObject.transform.parent.gameObject.activeSelf)
                        {
                            // 비활성화
                            iv.deathMenuSlots[i].gameObject.transform.parent.gameObject.SetActive(false);

                            // 비우기
                            iv.deathMenuSlots[i].ClearSlot();
                        }
                    }

                    // 소지금 반띵
                    gold = gold / 2;

                    // 플레이어 부활 처리
                    isDie = false;
                    PlayerManager.instance.anim.SetBool("isDeath", false);
                }

                // 체력 초기화
                PlayerManager.instance.health = player_MaxHealth[level];

                // 던전 씬 리스트 초기화
                randomStageSceneList = stageSceneList.ToList(); // 던전 씬 이름 리스트 초기화
                currentDungeonFloor = 0; // 던전 층 초기화

                town_Shop.Restock(); // 마을 상점 재입고
                dungeon_Shop.RandomRestock(); // 던전 상점 랜덤 재입고

                SoundManager.instance.PlayBackgroundMusic("Town"); // 마을 BGM 재생

                Save(); // 저장
                break;

            case >= 4: // 던전 씬 번호 : 4이상
                playerLocation = "던전";
                player.transform.position = new Vector3(0, 0, 0); // 플레이어 위치
                locationText.text = $"{playerLocation} {currentDungeonFloor}층"; // 일시정지 메뉴 현재 위치 텍스트 업데이트
                isDropFairy = false; // 페어리 생성되지 않음

                // 포탈 매니저가 할당되지 않았다면
                if (pt == null)
                {
                    pt = GameObject.Find("Portal").GetComponent<Portal>(); // 포탈 할당

                    StageMonsterCheck(); // 게임매니저의 스테이지 몬스터 체크 함수 실행
                }

                // 만약 현재 씬이 보스 씬이라면
                if (SceneManager.GetActiveScene().name == "DungeonBoss")
                {
                    SoundManager.instance.StopBackgroundMusic(); // 배경음악 끄기
                }
                // 던전 상점 씬이라면
                else if (SceneManager.GetActiveScene().name == "DungeonShop")
                {
                    SoundManager.instance.PlayBackgroundMusic("DungeonShop"); // 던전 상점 BGM 재생
                }
                // 던전 스테이지 씬이라면
                else
                {
                    SoundManager.instance.PlayBackgroundMusic("Dungeon"); // 던전 BGM 재생

                    // 페어리 스폰포인트가 할당되지 않았다면
                    if (fairySpawnPoint == null)
                    {
                        fairySpawnPoint = GameObject.Find("FairySpawnPoint"); // 페어리 스폰포인트 할당
                    }
                }
                break;
        }
    }

    // 스테이지에 있는 몬스터 수를 체크하는 함수
    public void StageMonsterCheck()
    {
        monsterInStage = GameObject.FindGameObjectsWithTag("Monster");
        monsterInStageList = new List<GameObject>(monsterInStage); // 배열을 리스트로 형변환
    }

    // 포탈 탑승 함수
    public void GetIntoPortal()
    {
        switch (pt.portal_ID)
        {
            // 포탈 ID가 0일 때 (던전으로 가는 포탈)
            case 0:
                if (currentDungeonFloor >= 9) // 9층에서 탑승했다면 보스 스테이지로 이동
                {
                    SetLoadScene("DungeonBoss"); // 보스 스테이지로 이동
                    currentDungeonFloor++; // 던전 한 층 올라감
                }
                else
                {
                    int rand = Random.Range(0, randomStageSceneList.Count); // 불러올 씬 이름을 던전 씬 리스트에서 랜덤으로 뽑기
                    SetLoadScene(randomStageSceneList[rand]); // 뽑은 랜덤 값의 씬으로 이동
                    randomStageSceneList.RemoveAt(rand); // 한번 사용한 씬은 리스트에서 제거
                    currentDungeonFloor++; // 던전 한 층 올라감
                }
                break;

            // 포탈 ID가 1일 때 (마을로 가는 포탈)
            case 1:
                tutorial_Complete = true; // 튜토리얼 완료

                // 인벤토리 비우기
                for (int i = 0; i < iv.slots.Length; i++)
                {
                    // 슬롯에 아이템이 있다면
                    if (iv.slots[i].item != null)
                    {
                        // 비우기
                        iv.slots[i].ClearSlot();
                    }
                }

                SetLoadScene("Town"); // 마을로 이동
                break;

            // 포탈 ID가 2일 때 (게임 클리어 포탈)
            case 2:
                dontMove = true; // 플레이어 움직임 제어

                SoundManager.instance.PlaySound("GameClear"); // 게임 클리어 효과음 재생
                // 인벤토리 닫기
                activeInventoty = false;
                invectoryPanel.SetActive(activeInventoty);

                // 게임 클리어 창 정보 업데이트
                deathMenu_TimeText.text = (playDungeonTime / 60).ToString("00") + "분 " + (playDungeonTime % 60).ToString("00") + "초"; // 시간
                deathMenu_LocationText.text = string.Format("{0} {1}층", playerLocation, currentDungeonFloor); // 위치
                deathMenu_GoldText.text = string.Format("{0:n0} G", gold); // 소지금
                goldPenaltyText.SetActive(false); // 패널티 없음
                clearMenu_GameClearTitleText.text = "게임 클리어"; // 제목 텍스트 변경
                deathMenu_DeathCauseText.text = "축하합니다. 게임을 클리어하셨습니다!"; // 게임 클리어 텍스트
                clearMenu_ComentTitleText.text = "개발자 코멘트"; // 제목 텍스트 변경
                instance.deathMenu_NextExpText.text = "플레이해주셔서 감사합니다. \n열심히 만들었습니다."; // 개발자 코멘트

                // 획득했던 아이템
                for (int i = 0; i < iv.slots.Length; i++)
                {
                    // 인벤토리 슬롯에 아이템이 있다면
                    if (iv.slots[i].item != null)
                    {
                        // 사망 정보 창 슬롯 배열을 뒤져서
                        for (int j = 0; j < iv.deathMenuSlots.Length; j++)
                        {
                            // 비어있는 사망 정보 창 슬롯을 찾았다면
                            if (iv.deathMenuSlots[j].item == null)
                            {
                                // 비어있는 사망 정보 창 슬롯에 인벤토리 슬롯 아이템을 넣고 활성화 시킨다
                                iv.deathMenuSlots[j].AddItem(iv.slots[i].item);
                                iv.deathMenuSlots[j].gameObject.transform.parent.gameObject.SetActive(true);

                                break;
                            }
                        }
                    }
                }

                deathMenu.SetActive(true); // 게임 클리어 창 활성화
                adventureClearPanel.SetActive(true); // 탐험 성공 패널 활성화

                deathMenu.GetComponent<Animator>().SetTrigger("Show"); // 사망 정보 패널 애니메이션 재생
                adventureClearPanel.GetComponent<Animator>().SetTrigger("Show"); // 탐험 성공 패널 애니메이션 재생

                break;
        }
    }

    // 대화 함수
    public void Talk(int id, string name)
    {
        string talkData = "";

        // 현재 타이핑이 진행되고 있다면
        if (talkText.isTyping)
        {
            // 대화 불러오지 않고 리턴
            talkText.SetMessage("");
            return;
        }
        // 타이핑이 완료되었다면
        else
        {
            talkData = TalkManager.instance.GetTalk(id, talkIndex); // 대화 불러오기
        }

        choicePanel.SetActive(false); // 대화 선택창 비활성화 (초기화)

        // 현재 대화 인덱스가 전달받은 대화 선택창이 나올 인덱스라면
        if (talkIndex == currentChoiceTalkIndex)
        {
            isChoiceTalk = true; // 대화 선택 중
        }
        else
        {
            isChoiceTalk = false; // 대화 선택 중 아님
        }

        if (talkData == null) // 불러온 대화가 없다면
        {
            talkPanel.SetActive(false); // 대화창 닫기
            isTalk = false; // 대화 중 아님
            talkIndex = 0; // 대사 번호 초기화
            return;
        }
        else
        {
            npcNameText.text = name; // 이름 텍스트 업데이트
            talkPanel.SetActive(true); // 대화창 활성화
            choicePanel.SetActive(false); // 대화 선택창 비활성화 (초기화)
            talkText.SetMessage(talkData); // 대화 텍스트 업데이트, 타이핑 시작

            talkIndex++; // 대사 번호 + 1
        }
    }

    // 플레이어 레벨업 함수
    public void LevelUp()
    {
        // 레벨이 30보다 작을 경우에만 레벨업
        if (level < maxLevel)
        {
            level++; // 레벨 + 1
            // 효과음
        }
    }

    // 부활 함수
    public void Retry()
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        // 마을로 씬 변경
        SetLoadScene("Town"); // 마을로 이동
    }

    // 툴팁 활성화 함수 (아이템 데이터, 툴팁 위치)
    public void ShowTooltip(ItemData item, Transform transform)
    {
        tooltip_ItemDescription.text = item.itemDescription; // 아이템 설명 변경
        tooltip_ItemImage.sprite = item.itemImage; // 아이템 이미지 변경

        // 등급에 따른 아이템 이름과 색깔 변경
        switch (item.itemClass)
        {
            case ItemData.ItemClass.Common:
                tooltip_ItemName.text = $"<color=#FFFFFF>{item.itemName}</color>"; // 일반
                break;
            case ItemData.ItemClass.Rare:
                tooltip_ItemName.text = $"<color=#FF7500>{item.itemName}</color>"; // 레어
                break;
            case ItemData.ItemClass.Epic:
                tooltip_ItemName.text = $"<color=#FF00AE>{item.itemName}</color>"; // 에픽
                break;
        }

        tooltipObject.transform.position = transform.position; // 툴팁 위치를 슬롯 위치로 변경

        float halfWidth = hud_CanvasObject.GetComponent<CanvasScaler>().referenceResolution.x * 0.5f;
        float halfHeight = hud_CanvasObject.GetComponent<CanvasScaler>().referenceResolution.y * 0.5f;

        // 툴팁이 화면 밖으로 나갈 시 티벗 변경 ( 툴팁의 포지션과 길이, 캔버스 스케일러를 이용해 인벤토리가 잘렸는지 계산 )
        if ((tooltip_rt.anchoredPosition.y - tooltip_rt.sizeDelta.y < -halfHeight)
            && (tooltip_rt.anchoredPosition.x - tooltip_rt.sizeDelta.x < -halfWidth)) // 아래, 왼쪽 모두 잘림
        {
            Debug.Log("툴팁 아래, 왼쪽 잘림");
            tooltip_rt.pivot = new Vector2(0, 0); // 툴팁 위치 -> 오른쪽 위
        }
        else if (tooltip_rt.anchoredPosition.y - tooltip_rt.sizeDelta.y < -halfHeight) // 아래가 잘림
        {
            Debug.Log("툴팁 아래 잘림");
            tooltip_rt.pivot = new Vector2(1, 0); // 툴팁 위치 -> 왼쪽 위
        }
        else if (tooltip_rt.anchoredPosition.x - tooltip_rt.sizeDelta.x < -halfWidth) // 왼쪽이 잘림
        {
            Debug.Log("툴팁 왼쪽 잘림");
            tooltip_rt.pivot = new Vector2(0, 1); // 툴팁 위치 -> 오른쪽 아래
        }
        else // 온전함
        {
            tooltip_rt.pivot = new Vector2(1, 1); // 툴팁 위치 -> 왼쪽 아래
        }

        // 최대 체력 텍스트
        if (item.increase_Health != 0)
        {
            // 최대 체력 증가량이 양수일 때
            if (item.increase_Health > 0)
            {
                tooltip_IncreaseMaxHealthText.text = $"> <color=#24FF00>+{float.Parse(item.increase_Health.ToString("F1"))}</color> 최대 체력"; // 최대 체력 증가량 텍스트 변경 (증가)
            }
            // 최대 체력 증가량이 음수일 때
            else if (item.increase_Health < 0)
            {
                tooltip_IncreaseMaxHealthText.text = $"> <color=#FF4000>{float.Parse(item.increase_Health.ToString("F1"))}</color> 최대 체력"; // 최대 체력 증가량 텍스트 변경 (감소)
            }

            tooltip_IncreaseMaxHealthObject.SetActive(true); // 최대 체력 증가량 띄우기
        }

        // 공격력 증가량 텍스트
        if (item.increase_Damage != 0)
        {
            // 공격력 증가량이 양수일 때
            if (item.increase_Damage > 0)
            {
                tooltip_IncreaseAttackDamageText.text = $"> <color=#24FF00>+{float.Parse(item.increase_Damage.ToString("F1"))}</color> 공격력"; // 공격력 증가량 텍스트 변경 (증가)
            }
            // 공격력 증가량이 음수일 때
            else if (item.increase_Damage < 0)
            {
                tooltip_IncreaseAttackDamageText.text = $"> <color=#FF4000>{float.Parse(item.increase_Damage.ToString("F1"))}</color> 공격력"; // 공격력 증가량 텍스트 변경 (감소)
            }

            tooltip_IncreaseAttackDamageObject.SetActive(true); // 공격력 증가량 띄우기
        }

        // 이동 속도 증가량 텍스트
        if (item.increase_MoveSpeed != 0)
        {
            // 증가량이 양수일 때
            if (item.increase_MoveSpeed > 0)
            {
                tooltip_IncreaseMoveSpeedText.text = $"> <color=#24FF00>+{float.Parse(item.increase_MoveSpeed.ToString("F1"))}</color> 이동 속도"; // 이동 속도 증가량 텍스트 변경 (증가)
            }
            // 증가량이 음수일 때
            else if (item.increase_MoveSpeed < 0)
            {
                tooltip_IncreaseMoveSpeedText.text = $"> <color=#FF4000>{float.Parse(item.increase_MoveSpeed.ToString("F1"))}</color> 이동 속도"; // 이동 속도 증가량 텍스트 변경 (감소)
            }

            tooltip_IncreaseMoveSpeedObject.SetActive(true); // 이동 속도 증가량 띄우기
        }

        // 점프력 증가량 텍스트
        if (item.increase_JumpPower != 0)
        {
            // 증가량이 양수일 때
            if (item.increase_JumpPower > 0)
            {
                tooltip_IncreaseJumpPowerText.text = $"> <color=#24FF00>+{float.Parse(item.increase_JumpPower.ToString("F1"))}</color> 점프력"; // 점프력 증가량 텍스트 변경 (증가)
            }
            // 증가량이 음수일 때
            else if (item.increase_JumpPower < 0)
            {
                tooltip_IncreaseJumpPowerText.text = $"> <color=#FF4000>{float.Parse(item.increase_JumpPower.ToString("F1"))}</color> 점프력"; // 점프력 증가량 텍스트 변경 (감소)
            }

            tooltip_IncreaseJumpPowerObject.SetActive(true); // 점프력 증가량 닫기 띄우기
        }

        // 치명타 확률 증가량 텍스트
        if (item.increase_CriticalPercentage != 0)
        {
            // 증가량이 양수일 때
            if (item.increase_CriticalPercentage > 0)
            {
                tooltip_IncreaseCriticalPercentageText.text = $"> <color=#24FF00>+{float.Parse((item.increase_CriticalPercentage * 100).ToString("F1"))}%</color> 치명타 확률"; // 치명타 확률 증가량 텍스트 변경 (증가)
            }
            // 증가량이 음수일 때
            else if (item.increase_CriticalPercentage < 0)
            {
                tooltip_IncreaseCriticalPercentageText.text = $"> <color=#FF4000>{float.Parse((item.increase_CriticalPercentage * 100).ToString("F1"))}%</color> 치명타 확률"; // 치명타 확률 증가량 텍스트 변경 (감소)
            }

            tooltip_IncreaseCriticalPercentageObject.SetActive(true); // 치명타 확률 증가량 띄우기
        }

        // 치명타 계수 증가량 텍스트
        if (item.increase_CriticalValue != 0)
        {
            // 증가량이 양수일 때
            if (item.increase_CriticalValue > 0)
            {
                tooltip_IncreaseCriticalValueText.text = $"> <color=#24FF00>+{float.Parse(item.increase_CriticalValue.ToString("F1"))}</color> 치명타 피해"; // 치명타 피해 증가량 텍스트 변경 (증가)
            }
            // 증가량이 음수일 때
            else if (item.increase_CriticalValue < 0)
            {
                tooltip_IncreaseCriticalValueText.text = $"> <color=#FF4000>{float.Parse(item.increase_CriticalValue.ToString("F1"))}</color> 치명타 피해"; // 치명타 피해 증가량 텍스트 변경 (감소)
            }

            tooltip_IncreaseCriticalValueObject.SetActive(true); // 치명타 피해 증가량 띄우기
        }

        // 점프 횟수 증가량 텍스트
        if (item.increase_JumpCount != 0)
        {
            // 증가량이 양수일 때
            if (item.increase_JumpCount > 0)
            {
                tooltip_IncreaseJumpCountText.text = $"> <color=#24FF00>+{float.Parse(item.increase_JumpCount.ToString("F1"))}</color> 점프 횟수"; // 점프 횟수 증가량 텍스트 변경 (증가)
            }
            // 증가량이 음수일 때
            else if (item.increase_JumpCount < 0)
            {
                tooltip_IncreaseJumpCountText.text = $"> <color=#FF4000>{float.Parse(item.increase_JumpCount.ToString("F1"))}</color> 점프 횟수"; // 점프 횟수 증가량 텍스트 변경 (감소)
            }

            tooltip_IncreaseJumpCountObject.SetActive(true); // 점프 횟수 증가량 띄우기
        }

        // 대쉬 횟수 증가량 텍스트
        if (item.increase_DashCount != 0)
        {
            // 증가량이 양수일 때
            if (item.increase_DashCount > 0)
            {
                tooltip_IncreaseDashCountText.text = $"> <color=#24FF00>+{float.Parse(item.increase_DashCount.ToString("F1"))}</color> 대쉬 횟수"; // 대쉬 횟수 증가량 텍스트 변경 (증가)
            }
            // 증가량이 음수일 때
            else if (item.increase_DashCount < 0)
            {
                tooltip_IncreaseDashCountText.text = $"> <color=#FF4000>{float.Parse(item.increase_DashCount.ToString("F1"))}</color> 대쉬 횟수"; // 대쉬 횟수 증가량 텍스트 변경 (감소)
            }

            tooltip_IncreaseDashCountObject.SetActive(true); // 대쉬 횟수 증가량 띄우기
        }

        tooltipObject.SetActive(true); // 툴팁 패널 띄우기
    }

    // ESC 메뉴 버튼 관련 함수
    public void OpenInventory() // 인벤토리 열기
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        // ESC 메뉴 닫기
        activeEscMenu = false;
        if (playerLocation == "던전")
        {
            escMenuPanel.SetActive(activeEscMenu);
        }
        else
        {
            townEscMenuPanel.SetActive(activeEscMenu);
        }

        uiTooltipObject.SetActive(false); // UI 툴팁 닫기

        // 일시정지 해제
        Time.timeScale = 1f;

        // 인벤토리 열림 사운드
        SoundManager.instance.PlaySound("OpenInventory");

        // 인벤토리 열기
        activeInventoty = true;
        invectoryPanel.SetActive(activeInventoty);
    }

    public void OpenOption() // 설정 열기
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        // 설정 열기
        optionMenuPanel.SetActive(true);
    }

    public void EscapeDungeon() // 던전에서 나가기
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        // 진짜 나갈건지 되묻는 창 띄우기
        realyEscapeDungeonPanel.SetActive(true);
    }

    public void EscapeYes() // 되묻는 창에서 '네'를 눌렀을 때 실행할 함수
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        // 되묻는 창 닫기
        realyEscapeDungeonPanel.SetActive(false);

        // 일시정지 메뉴 닫기
        activeEscMenu = false;
        escMenuPanel.SetActive(activeEscMenu);

        // 일시정지 해제
        Time.timeScale = 1f;

        failCause = "용사는 무기를 버리고 \n던전에서 도망쳤다!";
        // 플레이어 사망 처리
        PlayerManager.instance.Die();
    }

    public void EscapeNo() // 되묻는 창에서 '아니요'를 눌렀을 때 실행할 함수
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        // 되묻는 창 닫기
        realyEscapeDungeonPanel.SetActive(false);
    }


    public void GoMain() // 메인화면으로 나가기
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        // 진짜 나갈건지 되묻는 창 띄우기
        realyGoMainPanel.SetActive(true);
    }

    public void GoMainYes() // 되묻는 창에서 '네'를 눌렀을 때 실행할 함수
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        // 되묻는 창 닫기
        realyGoMainPanel.SetActive(false);

        // 일시정지 메뉴 닫기
        activeEscMenu = false;
        escMenuPanel.SetActive(activeEscMenu); // 던전 일시정지 메뉴
        townEscMenuPanel.SetActive(activeEscMenu); // 마을 일시정지 메뉴

        // 일시정지 해제
        Time.timeScale = 1f;

        // BGM 끔
        SoundManager.instance.StopBackgroundMusic();

        // 화면 어두워지며 메인으로 이동
        SetLoadScene("Main"); // 메인으로 이동
    }

    public void GoMainNo() // 되묻는 창에서 '아니요'를 눌렀을 때 실행할 함수
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        // 되묻는 창 닫기
        realyGoMainPanel.SetActive(false);
    }


    // 설정 화면 탭 페이지 관련 함수
    public void DisplayModePageUp() // 화면 모드 페이지 업 함수
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        // 마지막 페이지라면
        if (displayModePage >= displayModeList.Length - 1)
        {
            displayModePage = 0; // 처음 페이지로
        }
        else
        {
            ++displayModePage; // 다음 페이지로
        }

        ChangeDisplayModePage(); // 화면 모드 페이지 업데이트
    }

    public void DisplayModePageDown() // 화면 모드 페이지 다운 함수
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        // 처음 페이지라면
        if (displayModePage <= 0)
        {
            Debug.Log(displayModeList.Length);
            displayModePage = displayModeList.Length - 1; // 마지막 페이지로
        }
        else
        {
            --displayModePage; // 이전 페이지로
        }

        ChangeDisplayModePage(); // 화면 모드 페이지 업데이트
    }
    public void ResolutionPageUp() // 해상도 페이지 업 함수
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        // 마지막 페이지라면
        if (resolutionPage >= resolutionList.Count -1)
        {
            resolutionPage = 0; // 처음 페이지로
        }
        else
        {
            ++resolutionPage; // 다음 페이지로
        }

        ChangeResolutionPage(); // 화면 해상도 페이지 업데이트
    }

    public void ResolutionPageDown() // 해상도 페이지 다운 함수
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        // 처음 페이지라면
        if (resolutionPage <= 0)
        {
            resolutionPage = resolutionList.Count - 1; // 마지막 페이지로
        }
        else
        {
            --resolutionPage; // 이전 페이지로
        }
        ChangeResolutionPage(); // 화면 해상도 페이지 업데이트
    }

    // 화면 모드 페이지 업데이트
    void ChangeDisplayModePage()
    {
        if (displayModeList[displayModePage] == FullScreenMode.FullScreenWindow)
        {
            displayModeText.text = "전체 화면";
        }
        else if (displayModeList[displayModePage] == FullScreenMode.Windowed)
        {
            displayModeText.text = "창 모드";
        }
    }

    // 화면 해상도 페이지 업데이트
    void ChangeResolutionPage()
    {
        resolutionText.text = $"{resolutionList[resolutionPage].width} X {resolutionList[resolutionPage].height}";
    }

    // 화면 설정 적용
    public void ApplyOption()
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        Screen.SetResolution(resolutionList[resolutionPage].width, resolutionList[resolutionPage].height, displayModeList[displayModePage]); // 화면 설정 적용
    }

    // 게임 시작
    public void GameStart()
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        dataSlotSelectPanel.SetActive(true); // 데이터 슬롯 선택 패널 띄우기
    }

    // 게임 종료
    public void QuitGame()
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        Application.Quit(); // 종료
    }

    // 데이터 슬롯 선택 화면 닫기
    public void CloseSelectDataSlotMenu()
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        dataSlotSelectPanel.SetActive(false); // 데이터 슬롯 선택 패널 닫기
    }

    // 페이드 인
    IEnumerator FadeIn()
    {
        dontMove = true; // 플레이어 움직임 제어

        isFade = true; // 현재 페이드 중 체크

        // 페이드 패널 활성화
        fadePanel.gameObject.SetActive(true);

        // 페이드 되는 시간
        float time = 0;

        Color alpha = fadePanel.color;

        float volume = PlayerPrefs.GetFloat("BGM_Volume");

        while (alpha.a < 1f)
        {
            time += Time.deltaTime / fadeTime;

            // 페이드 인
            alpha.a = Mathf.Lerp(0, 1, time);

            // 투명도 적용
            fadePanel.color = alpha;

            yield return null;
        }

        canSceneLoad = true; // 씬 로드해야 하는 상태 체크

        yield return null;
    }

    // 페이드 아웃
    IEnumerator FadeOut()
    {
        canSceneLoad = false; // 씬 로드해야 하는 상태 아님 체크
        isLoadScene = false; // 씬 불러오기를 했는지 체크 해제

        Color alpha = fadePanel.color;

        float volume = PlayerPrefs.GetFloat("BGM_Volume");

        // 페이드 되는 시간
        float time = 0;

        while (alpha.a > 0f)
        {
            time += Time.deltaTime / fadeTime;

            // 페이드 아웃
            alpha.a = Mathf.Lerp(1, 0, time);

            // 투명도 적용
            fadePanel.color = alpha;

            yield return null;
        }

        isFade = false; // 현재 페이드 끝남 체크

        // 페이드 패널 비활성화
        fadePanel.gameObject.SetActive(false);

        dontMove = false; // 플레이어 움직임 제어 해제

        yield return null;
    }

    public void SetLoadScene(string sceneName)
    {
        // 로드할 씬 이름 설정
        loadSceneName = sceneName;

        // 페이드 인
        StartCoroutine("FadeIn");
    }

    // 저장
    public void Save()
    {
        DataManager.instance.nowPlayer.tutorial_Complete = tutorial_Complete; // 튜토리얼 완료 여부
        DataManager.instance.nowPlayer.gold = gold; // 소지금
        DataManager.instance.nowPlayer.exp = exp; // 경험치
        DataManager.instance.nowPlayer.level = level; // 레벨
        DataManager.instance.nowPlayer.highFloor = highDungeonFloor; // 도달한 층
        DataManager.instance.nowPlayer.playTime = playTime; // 플레이 시간

        DataManager.instance.SaveData();
    }

    // 데이터 적용
    public void ApplyData()
    {
        tutorial_Complete = DataManager.instance.nowPlayer.tutorial_Complete; // 튜토리얼 완료 여부
        gold = DataManager.instance.nowPlayer.gold; // 소지금
        exp = DataManager.instance.nowPlayer.exp; // 경험치
        level = DataManager.instance.nowPlayer.level; // 레벨
        highDungeonFloor = DataManager.instance.nowPlayer.highFloor; // 도달한 층
        playTime = DataManager.instance.nowPlayer.playTime; // 플레이 시간
    }
}
