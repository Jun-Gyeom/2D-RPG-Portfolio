using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    public static TalkManager instance; // 싱글톤을 할당할 전역변수

    public Dictionary<int, string[]> talkData; // 대화 데이터 딕셔너리< id, 대화 배열 >

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
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 TalkManager가 삭제되지 않음


        talkData = new Dictionary<int, string[]>();

        GenerateData();
    }

    void GenerateData()
    {
        // 마을 이장
        talkData.Add(1000, new string[] { "반갑네 용사여..", "우리 마을을 위해 먼 길 와주어 고맙네.." });
        talkData.Add(1010, new string[] { "이 마을은 예전부터 아주 살기 좋은 마을이었어.. 마을 사람들은 모두 친절했고 여름이 올 때면 마을에서 축제를 했지..",
            "그런데 하루는 마을에 태풍이 불어 큰 피해가 있었어.. 건물이 무너지고 흉년이 들었지..", "마을 사람들은 절망에 빠졌지만 나는 아니었어.. 이 마을에 계속 살고 싶었거든..", "결국 나와 몇몇의 사람들이 마을을 모두 재건했지.. 그 이후로 난 마을 사람들에게 존경 받으며 이 자리까지 올 수 있었겠지.." });

        // 뚱보 상인
        talkData.Add(2000, new string[] { "모험가여 반갑다.", "나는 이 곳에서 물건을 판다. 한번 보고 가라"});

        // 셀레나
        talkData.Add(3000, new string[] { "안녕하세요 용사님! 무엇을 도와드릴까요?"});

        // 비석
        talkData.Add(4000, new string[] { "개발자: 김준겸\n제작기간: 약 3개월\n\"버그는 사절합니다.\" 라고 쓰여있다." });
    }

    // 대화 불러오기 함수
    public string GetTalk(int id, int talkIndex)
    {
        if (talkIndex == talkData[id].Length) // 마지막 대사라면
        {
            return null; // 대사 없음
        }
        else
        {
            return talkData[id][talkIndex]; // 대사 반환
        }
    }

    // 대화 선택 - 이야기 하기 함수
    public void choiceBtn_Talk()
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        GameManager.instance.currentNpcId = GameManager.instance.currentNpcId + 10; // 대화 ID에 10을 더해 분기를 나눠 다른 대화를 하도록 한다.
        GameManager.instance.talkIndex = 0; // 대사 번호 초기화
        GameManager.instance.currentChoiceTalkIndex = -1; // 대화 선택창 인덱스 번호 초기화

        GameManager.instance.Talk(GameManager.instance.currentNpcId, GameManager.instance.currentNpcName); // 대화 시작
    }

    // 대화 선택 - 마을 상점 함수
    public void choiceBtn_TownShop()
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        GameManager.instance.townShopPanel.SetActive(true); // 상점 열기

        // 인벤토리 활성화
        GameManager.instance.activeInventoty = true;
        GameManager.instance.invectoryPanel.SetActive(GameManager.instance.activeInventoty);

        SoundManager.instance.PlaySound("OpenInventory"); // 효과음 재생

        GameManager.instance.Talk(GameManager.instance.currentNpcId, GameManager.instance.currentNpcName); // 대화 끝내기 (다음 대사가 없으므로)
    }

    // 대화 선택 - 던전 상점 함수
    public void choiceBtn_DungeonShop()
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        GameManager.instance.dungeonShopPanel.SetActive(true); // 상점 열기

        // 인벤토리 활성화
        GameManager.instance.activeInventoty = true;
        GameManager.instance.invectoryPanel.SetActive(GameManager.instance.activeInventoty);

        SoundManager.instance.PlaySound("OpenInventory"); // 효과음 재생

        GameManager.instance.Talk(GameManager.instance.currentNpcId, GameManager.instance.currentNpcName); // 대화 끝내기 (다음 대사가 없으므로)
    }

    // 대화 선택 - 아무것도 함수
    public void choiceBtn_Nothing()
    {
        // 버튼 클릭 사운드
        SoundManager.instance.PlaySound("ButtonClick");

        GameManager.instance.Talk(GameManager.instance.currentNpcId, GameManager.instance.currentNpcName); // 대화 끝내기 (다음 대사가 없으므로)
    }
}
