using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 싱글톤을 할당할 전역변수
    public PortalManager pt; // 포탈매니저 스크립트
    public PlayerManager pm; // 플레이어 매니저 스크립트
    public Inventory iv; // 인벤토리 스크립트

    public ParticleSystem footDustParticle; // 플레이어가 이동할 때 흩날리는 먼지 파티클
    public ParticleSystem doubleJumpParticle; // 플레이어가 2단 점프할 때 나오는 파티클

    private GameObject player; // DontDestroy에 등록할 플레이어 오브젝트
    private GameObject mainCamera; // DontDestroy에 등록할 메인카메라 오브젝트
    private GameObject canvas; // DontDestroy에 등록할 캔버스 오브젝트

    private GameObject[] monsterInStage; // 현재 스테이지에 있는 몬스터 배열
    public List<GameObject> monsterInStageList; // 현재 스테이지에 있는 몬스터 리스트

    private List<string> stageSceneList; // 던전(스테이지) 씬 리스트

    // UI 관련
    private GameObject goldTextObject; // 보유한 골드량 텍스트 오브젝트 (좌측 상단)
    public TMP_Text goldText; // 보유한 골드량 텍스트 (좌측 상단)

    public GameObject invectoryPanel; // 인벤토리 오브젝트
    public GameObject tooltipObject; // 툴팁 오브젝트
    public GameObject deathMenu; // 죽음 메뉴 오브젝트
    public GameObject escMenuPanel; // 일시정지 메뉴 오브젝트
    public GameObject optionMenuPanel; // 설정 메뉴 오브젝트
    public bool activeInventoty; // 인벤토리의 활성 여부
    public bool activeEscMenu; // 일시정지 메뉴 활성 여부

    public GameObject mouseCursor; // 마우스 커서 오브젝트

    // 설정 관련
    public List<Resolution> resolutionList = new List<Resolution>(); // 해상도 리스트
    public FullScreenMode[] displayModeList; // 화면 모드

    private int displayModePage; // 현재 화면 모드 페이지
    private int resolutionPage; // 현재 해상도 페이지
    public TMP_Text displayModeText; // 현재 화면 모드 텍스트
    public TMP_Text resolutionText; // 현재 해상도 텍스트

    // 플레이어 관련
    public bool isDie; // 플레이어가 죽었는지

    public int level; // 플레이어 레벨
    public int maxLevel; // 플레이어 최대 레벨
    public int[] levelUp_exp; // 레벨업에 필요한 경험치량 배열

    public float gold; // 골드
    public float exp; // 경험치


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

        // Find 함수로 찾아서 할당
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        canvas = GameObject.Find("HUD_Canvas");
        goldTextObject = GameObject.Find("Gold_Text");

        // 컴포넌트 할당
        goldText = goldTextObject.GetComponent<TextMeshProUGUI>();

        DontDestroyOnLoad(pt); // 씬 전환 시에도 PortalManager가 삭제되지 않음
        DontDestroyOnLoad(player); // 씬 전환 시에도 플레이어가 삭제되지 않음
        DontDestroyOnLoad(mainCamera); // 씬 전환 시에도 메인카메라가 삭제되지 않음
        DontDestroyOnLoad(canvas); // 씬 전환 시에도 캔버스가 삭제되지 않음

        stageSceneList = new List<string> { "Dungeon01", "Dungeon02" }; // 던전 씬 이름 리스트 초기화

        resolutionList.AddRange(Screen.resolutions); // 화면 해상도 리스트 초기화
    }

    private void Start()
    {
        invectoryPanel.SetActive(false); // 인벤토리 패널 닫기
    }

    void Update()
    {
        goldText.text = string.Format("{0:n0}", Mathf.Lerp(float.Parse(goldText.text), gold, Time.deltaTime * 60f));

        // 스테이지에 모든 몬스터를 처리했다면
        if (monsterInStageList.Count == 0)
        {
            // 포탈 개방
            pt.isPortalOpen = true;
        }
        // 스테이지에 몬스터가 남아있다면
        else
        {
            // 포탈 미개방
            pt.isPortalOpen = false;
        }

        // 메뉴 또는 인벤토리가 활성화 되어있거나 죽었다면
        if (activeInventoty || activeEscMenu || isDie)
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

        // 인벤토리가 닫혀있는데 툴팁은 활성화 돼있다면
        if (!activeInventoty && tooltipObject.activeSelf)
        {
            tooltipObject.SetActive(false); // 툴팁 닫기
        }

        // 일시정지 메뉴가 활성화 되어있다면
        if (activeEscMenu)
        {
            // 게임 일시정지
            Time.timeScale = 0f;
        }
        // 아니라면
        else
        {
            // 게임 일시정지 해제
            Time.timeScale = 1f;
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
        switch (pt.portal.portal_ID)
        {
            // 포탈 ID가 0일 때 (던전으로 가는 포탈)
            case 0:
                int rand = Random.Range(0, stageSceneList.Count); // 불러올 씬 이름을 던전 씬 리스트에서 랜덤으로 뽑기
                SceneManager.LoadScene(stageSceneList[rand]); // 뽑은 랜덤 값의 씬으로 이동
                stageSceneList.RemoveAt(rand); // 한번 사용한 씬은 리스트에서 제거
                player.transform.position = new Vector3(0, 0, 0); // 플레이어 위치
                break;
            // 포탈 ID가 1일 때 (마을로 가는 포탈)
            case 1:
                SceneManager.LoadScene("Town"); // 마을로 이동
                player.transform.position = new Vector3(-20, -1, 0); // 플레이어 위치

                stageSceneList = new List<string> { "Dungeon01", "Dungeon02" }; // 던전 씬 이름 리스트 초기화
                break;
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
        // 죽음 메뉴 닫기
        deathMenu.SetActive(false);

        // 마을로 씬 변경
        SceneManager.LoadScene("Town");
        player.transform.position = new Vector3(-20, -1, 0); // 플레이어 위치

        // 인벤토리 비우기
        for (int i = 0; i < iv.slots.Length; i++)
        {
            // 슬롯에 아이템이 있다면
            if (iv.slots[i].item != null)
            {
                iv.slots[i].ClearSlot();
            }

        }

        // 소지금 반띵
        gold = gold / 2;

        // 플레이어 부활 처리
        isDie = false;
        pm.health = pm.maxHealth;
        pm.anim.SetBool("isDeath", false);

        // 던전 씬 리스트 초기화
        stageSceneList = new List<string> { "Dungeon01", "Dungeon02" }; // 던전 씬 이름 리스트 초기화
    }

    // 설정 화면 탭 페이지 관련 함수
    public void DisplayModePageUp() // 화면 모드 페이지 업 함수
    {
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
        Screen.SetResolution(resolutionList[resolutionPage].width, resolutionList[resolutionPage].height, displayModeList[displayModePage]); // 화면 설정 적용
    }
}
