using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// 저장할 데이터 클래스
public class PlayerData
{
    public int level; // 플레이어 레벨
    public int highFloor; // 도달한 층
    public float playTime; // 플레이 시간
    public float gold; // 소지금
    public float exp; // 획득한 경험치
    public Item[] item; // 소지한 아이템
    public bool tutorial_Complete; // 튜토리얼 완료 여부
}

public class DataManager : MonoBehaviour
{
    // 싱글톤 변수
    public static DataManager instance;

    public PlayerData nowPlayer = new PlayerData(); // 플레이어 데이터 생성

    public string path; // 데이터 저장 경로
    public int nowSlot; // 고른 슬롯 번호


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
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 DataManager가 삭제되지 않음

        path = Application.persistentDataPath + "/Savefile"; // 경로 지정
        Debug.Log(path);
    }

    // 데이터 저장
    public void SaveData()
    {
        string data = JsonUtility.ToJson(nowPlayer); // 현재 플레이어 데이터를 Json으로 변환
        File.WriteAllText(path + nowSlot.ToString(), data); // Json을 저장 경로 파일에 저장
    }

    // 데이터 불러오기
    public void LoadData()
    {
        string data = File.ReadAllText(path + nowSlot.ToString()); // 저장 경로 파일에서 Json을 읽어옴
        nowPlayer = JsonUtility.FromJson<PlayerData>(data); // 읽어온 Json을 PlayerData형식으로 변환해서 현재 플레이어 데이터에 저장
    }

    // 현재 불러온 데이터 삭제
    public void DataClear()
    {
        nowSlot = -1; // 슬롯 선택 해제
        nowPlayer = new PlayerData(); // 데이터 초기화
    }

    // 세이브 파일 초기화
    public void DeleteSavefile()
    {
        File.Delete(path + nowSlot.ToString()); // 저장 경로 파일 삭제
    }
}
