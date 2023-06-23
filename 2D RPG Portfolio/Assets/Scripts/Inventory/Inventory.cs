using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    public Slot[] slots;
    public Slot[] deathMenuSlots;

    public TMP_Text goldText; // 골드 텍스트
    public TMP_Text getItemPanel_ItemName; // 아이템 획득 패널 아이템 이름
    public Image getItemPanel_ItemImage; // 아이템 획득 패널 아이템 이미지
    public GameObject getItemPanel; // 아이템 획득 패널 오브젝트

    // 플레이어 스탯 텍스트
    public TMP_Text maxHealth_StateText; // 최대 체력
    public TMP_Text criticalPercentage_StateText; // 치명타 확률
    public TMP_Text attackDamage_StateText; // 공격력
    public TMP_Text criticalValue_StateText; // 치명타 계수
    public TMP_Text moveSpeed_StateText; // 이동속도
    public TMP_Text dashCount_StateText; // 대쉬 횟수
    public TMP_Text jumpPower_StateText; // 점프력
    public TMP_Text jumpCount_StateText; // 점프 횟수

    private void Update()
    {
        // UI
        goldText.text = GameManager.instance.goldText.text; // 게임매니저의 골드텍스트와 동기화

        UpdateStatus(); // 스태이터스 업데이트
    }

    // 아이템 획득
    public void AcquireItem(GameObject _item)
    {
        ItemData item = _item.gameObject.GetComponent<Item>().item;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(item);

                Debug.Log($"{item.itemName}아이템을 주웠다!");

                // 아이템 획득 패널 띄우기
                getItemPanel_ItemImage.sprite = item.itemImage; // 아이템 획득 패널 아이템 이름 텍스트 변경 
                getItemPanel.SetActive(item);

                // 등급에 따른 아이템 이름과 색깔 변경
                switch (item.itemClass)
                {
                    case ItemData.ItemClass.Common:
                        getItemPanel_ItemName.text = $"<color=#FFFFFF>{item.itemName}</color>"; // 일반
                        break;
                    case ItemData.ItemClass.Rare:
                        getItemPanel_ItemName.text = $"<color=#FF7500>{item.itemName}</color>"; // 레어
                        break;
                    case ItemData.ItemClass.Epic:
                        getItemPanel_ItemName.text = $"<color=#FF00AE>{item.itemName}</color>"; // 에픽
                        break;
                }

                // 2.5초 후 아이템 패널 닫기
                if (IsInvoking("DestroyGetItemPanel")) // 이미 아이템 패널이 띄워져 있던 경우
                {
                    CancelInvoke("DestroyGetItemPanel"); // 인보크 함수 실행 취소 후
                    Invoke("DestroyGetItemPanel", 2.5f); // 다시 실행
                }
                else
                {
                    Invoke("DestroyGetItemPanel", 2.5f); // 아니라면 그냥 실행
                }

                _item.SetActive(false); // 오브젝트 풀에 오브젝트 반환
                return;
            }
        }
        Debug.Log("아이템 가득 참");
    }

    // 스테이터스 UI 업데이트
    void UpdateStatus()
    {
        // 최대 체력
        if (GameManager.instance.increased_MaxHealth != 0) // 추가 스탯이 있다면
        {
            maxHealth_StateText.text = $"최대 체력 {float.Parse((GameManager.instance.player_MaxHealth[GameManager.instance.level] + GameManager.instance.increased_MaxHealth).ToString("F1"))} <color=#FFA20D>+({float.Parse(GameManager.instance.increased_MaxHealth.ToString("F1"))})</color>"; // 증가 된 스탯 표시
        }
        else
        {
            maxHealth_StateText.text = $"최대 체력 {float.Parse((GameManager.instance.player_MaxHealth[GameManager.instance.level] + GameManager.instance.increased_MaxHealth).ToString("F1"))}";
        }

        // 치명타 확률
        if (Mathf.Abs(GameManager.instance.increased_CriticalPercentage * 100) > 0.1f) // 추가 스탯이 있다면
        {
            criticalPercentage_StateText.text = $"치명타 확률 {float.Parse(((GameManager.instance.critical_Percentage + GameManager.instance.increased_CriticalPercentage) * 100).ToString("F1"))}% <color=#FFA20D>+({float.Parse((GameManager.instance.increased_CriticalPercentage * 100).ToString("F1"))})%</color>"; // 증가 된 스탯 표시
        }
        else
        {
            criticalPercentage_StateText.text = $"치명타 확률 {float.Parse(((GameManager.instance.critical_Percentage + GameManager.instance.increased_CriticalPercentage) * 100).ToString("F1"))}%";
        }

        // 공격력
        if (GameManager.instance.increased_AttackDamage != 0) // 추가 스탯이 있다면
        {
            attackDamage_StateText.text = $"공격력 {GameManager.instance.player_AttackDamage[GameManager.instance.level] + GameManager.instance.increased_AttackDamage} <color=#FFA20D>+({GameManager.instance.increased_AttackDamage})</color>"; // 증가 된 스탯 표시
        }
        else
        {
            attackDamage_StateText.text = $"공격력 {GameManager.instance.player_AttackDamage[GameManager.instance.level] + GameManager.instance.increased_AttackDamage}";
        }

        // 치명타 피해
        if (GameManager.instance.increased_CriticalValue != 0) // 추가 스탯이 있다면
        {
            criticalValue_StateText.text = $"치명타 피해 x{float.Parse((GameManager.instance.critical_Value + GameManager.instance.increased_CriticalValue).ToString("F1"))}  <color=#FFA20D>+( {float.Parse(GameManager.instance.increased_CriticalValue.ToString("F1"))})</color>"; // 증가 된 스탯 표시
        }
        else
        {
            criticalValue_StateText.text = $"치명타 피해 x{float.Parse((GameManager.instance.critical_Value + GameManager.instance.increased_CriticalValue).ToString("F1"))}";
        }

        // 이동 속도
        if (GameManager.instance.increased_MoveSpeed != 0) // 추가 스탯이 있다면
        {
            moveSpeed_StateText.text = $"이동 속도 {float.Parse((GameManager.instance.player_MoveSpeed + GameManager.instance.increased_MoveSpeed).ToString("F1"))} <color=#FFA20D>+({float.Parse(GameManager.instance.increased_MoveSpeed.ToString("F1"))})</color>"; // 증가 된 스탯 표시
        }
        else
        {
            moveSpeed_StateText.text = $"이동 속도 {float.Parse((GameManager.instance.player_MoveSpeed + GameManager.instance.increased_MoveSpeed).ToString("F1"))}";
        }

        // 대쉬 횟수 
        if (GameManager.instance.increased_MaxDashCount != 0) // 추가 스탯이 있다면
        {
            dashCount_StateText.text = $"대쉬 횟수 {GameManager.instance.maxDashChargeCount + GameManager.instance.increased_MaxDashCount}회 <color=#FFA20D>+({GameManager.instance.increased_MaxDashCount})</color>"; // 증가 된 스탯 표시
        }
        else
        {
            dashCount_StateText.text = $"대쉬 횟수 {GameManager.instance.maxDashChargeCount + GameManager.instance.increased_MaxDashCount}회";
        }

        // 점프력
        if (GameManager.instance.increased_JumpPower != 0) // 추가 스탯이 있다면
        {
            jumpPower_StateText.text = $"점프력 {float.Parse((GameManager.instance.jumpPower + GameManager.instance.increased_JumpPower).ToString("F1"))} <color=#FFA20D>+({float.Parse(GameManager.instance.increased_JumpPower.ToString("F1"))})</color>"; // 증가 된 스탯 표시
        }
        else
        {
            jumpPower_StateText.text = $"점프력 {float.Parse((GameManager.instance.jumpPower + GameManager.instance.increased_JumpPower).ToString("F1"))}";
        }

        // 점프 횟수
        if (GameManager.instance.increased_MaxJump != 0) // 추가 스탯이 있다면
        {
            jumpCount_StateText.text = $"점프 횟수 {GameManager.instance.maxJump + GameManager.instance.increased_MaxJump}회 <color=#FFA20D>+({GameManager.instance.increased_MaxJump})</color>"; // 증가 된 스탯 표시
        }
        else
        {
            jumpCount_StateText.text = $"점프 횟수 {GameManager.instance.maxJump + GameManager.instance.increased_MaxJump}회";
        }
    }


    // 아이템 획득 패널 끄기
    private void DestroyGetItemPanel()
    {
        getItemPanel.SetActive(false);
    }
}

