using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class DataSlot : MonoBehaviour
{
    public TMP_Text[] dataText; // 저장 된 데이터 정보를 나타내는 텍스트

    bool[] savefile = new bool[3]; // 세이브파일 존재 여부

    private void OnEnable()
    {
        UpdateSlotDataText(); // 슬롯 데이터 텍스트 갱신
    }

    // 데이터 슬롯 기능
    public void Slot(int number)
    {
        DataManager.instance.nowSlot = number; // 해당 슬롯 선택

        if (savefile[number]) // 데이터가 있으면
        {
            // 해당 슬롯 데이터 불러오기
            DataManager.instance.LoadData();

            // 데이터 적용하기
            Debug.Log("데이터를 불러옴");
            GameManager.instance.ApplyData();

            if (DataManager.instance.nowPlayer.tutorial_Complete) // 튜토리얼을 완료했다면
            {
                // 게임으로
                Debug.Log("게임으로");
                GameManager.instance.SetLoadScene("Town");
            }
            else // 튜토리얼을 완료하지 않았다면
            {
                // 튜토리얼로
                Debug.Log("튜토리얼로");
                GameManager.instance.SetLoadScene("Tutorial");
            }
        }
        else // 데이터가 없으면
        {
            // 새 세이브파일 생성
            Debug.Log("세이브파일 생성");
            DataManager.instance.nowPlayer = new PlayerData();
            DataManager.instance.SaveData();

            // 데이터 적용하기
            Debug.Log("데이터를 불러옴");
            GameManager.instance.ApplyData();

            // 튜토리얼로
            Debug.Log("튜토리얼로");
            GameManager.instance.SetLoadScene("Tutorial");
        }

    }

    // 슬롯 세이브 파일 삭제
    public void DeleteSavefile(int number)
    {
        DataManager.instance.nowSlot = number; // 해당 슬롯 선택

        DataManager.instance.DeleteSavefile(); // 세이브 파일 삭제

        UpdateSlotDataText(); // 슬롯 데이터 텍스트 갱신
    }

    // 슬롯 데이터 텍스트 갱신
    public void UpdateSlotDataText()
    {
        // 슬롯별로 저장된 데이터가 있는지 검사
        for (int i = 0; i < 3; i++)
        {
            if (File.Exists(DataManager.instance.path + $"{i}")) // 데이터가 있는 경우
            {
                savefile[i] = true; // 데이터 있음
                DataManager.instance.nowSlot = i; // 해당 슬롯 선택
                DataManager.instance.LoadData(); // 선택한 슬롯 데이터 불러오기
                
                // 도달한 층 수가 1이상이면
                if (DataManager.instance.nowPlayer.highFloor > 0)
                {
                    dataText[i].text =
                    $"< 플레이 시간 >\n{(DataManager.instance.nowPlayer.playTime / 3600).ToString("00") + "H " + ((DataManager.instance.nowPlayer.playTime % 3600) / 60).ToString("00") + "M"}\n\n< 도달한 층 >\n{DataManager.instance.nowPlayer.highFloor}F\n\n< 레벨 >\n{DataManager.instance.nowPlayer.level + 1}LV\n\n< 소지금 >\n{string.Format("{0:n0}G", DataManager.instance.nowPlayer.gold)}"; // 슬롯 데이터 정보 텍스트 표시
                }
                else
                {
                    dataText[i].text =
                    $"< 플레이 시간 >\n{(DataManager.instance.nowPlayer.playTime / 3600).ToString("00") + "H " + ((DataManager.instance.nowPlayer.playTime % 3600) / 60).ToString("00") + "M"}\n\n< 레벨 >\n{DataManager.instance.nowPlayer.level + 1}LV\n\n< 소지금 >\n{string.Format("{0:n0}G", DataManager.instance.nowPlayer.gold)}"; // 슬롯 데이터 정보 텍스트 표시
                }
            }
            else // 데이터가 없는 경우
            {
                savefile[i] = false; // 데이터 없음
                dataText[i].text = "데이터 없음";
            }
        }
        // 슬롯에 데이터 정보를 표시하기 위한 데이터였기 때문에 데이터 초기화
        DataManager.instance.DataClear();
    }
}
