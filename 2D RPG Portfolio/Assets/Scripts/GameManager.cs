using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 싱글톤을 할당할 전역변수
    public PortalManager pm; // 포탈매니저

    private GameObject player; // DontDestroy에 등록할 플레이어 오브젝트
    private GameObject mainCamera; // DontDestroy에 등록할 메인카메라 오브젝트
    private GameObject canvas; // DontDestroy에 등록할 캔버스 오브젝트

    private GameObject[] monsterInStage; // 현재 스테이지에 있는 몬스터 배열
    public List<GameObject> monsterInStageList; // 현재 스테이지에 있는 몬스터 리스트

    private List<string> stageSceneList; // 던전(스테이지) 씬 리스트

    // UI 관련
    private GameObject goldTextObject; // 보유한 골드량 텍스트 오브젝트 (좌측 상단)
    public TMP_Text goldText; // 보유한 골드량 텍스트 (좌측 상단)


    public bool isDie; // 플레이어가 죽었는지

    public float gold; // 골드

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


        DontDestroyOnLoad(pm); // 씬 전환 시에도 PortalManager가 삭제되지 않음
        DontDestroyOnLoad(player); // 씬 전환 시에도 플레이어가 삭제되지 않음
        DontDestroyOnLoad(mainCamera); // 씬 전환 시에도 메인카메라가 삭제되지 않음
        DontDestroyOnLoad(canvas); // 씬 전환 시에도 캔버스가 삭제되지 않음

        stageSceneList = new List<string> { "Dungeon01", "Dungeon02" }; // 던전 씬 이름 리스트 초기화
    }
    

    void Update()
    {
        //goldText.text = string.Format("{0:n0}", Mathf.SmoothStep(float.Parse(goldText.text), gold, 5f));
        goldText.text = string.Format("{0:n0}", Mathf.Lerp(float.Parse(goldText.text), gold, Time.deltaTime * 40f));

        // 스테이지에 모든 몬스터를 처리했다면
        if (monsterInStageList.Count == 0)
        {
            // 포탈 개방
            pm.isPortalOpen = true;
        }
        // 스테이지에 몬스터가 남아있다면
        else
        {
            // 포탈 미개방
            pm.isPortalOpen = false;
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
        switch (pm.portal.portal_ID)
        {
            // 포탈 ID가 0일 때 (던전으로 가는 포탈)
            case 0:
                Debug.Log("던전으로 가는포탈 탑승!");

                int rand = Random.Range(0, stageSceneList.Count); // 불러올 씬 이름을 던전 씬 리스트에서 랜덤으로 뽑기
                SceneManager.LoadScene(stageSceneList[rand]); // 뽑은 랜덤 값의 씬으로 이동
                stageSceneList.RemoveAt(rand); // 한번 사용한 씬은 리스트에서 제거
                player.transform.position = new Vector3(0, 0, 0); // 플레이어 위치
                break;
            // 포탈 ID가 1일 때 (마을로 가는 포탈)
            case 1:
                Debug.Log("마을로 가는포탈 탑승!");

                SceneManager.LoadScene("Town"); // 마을로 이동
                player.transform.position = new Vector3(-20, -1, 0); // 플레이어 위치

                stageSceneList = new List<string> { "Dungeon01", "Dungeon02" }; // 던전 씬 이름 리스트 초기화
                break;
        }
    }
}
