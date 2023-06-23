using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData item; // 판매할 아이템
    public int itemPrice; // 아이템 가격

    public Image itemImage;  // 아이템의 이미지
    public TMP_Text itemNameText; // 아이템 이름 텍스트
    public TMP_Text itemPriceText; // 아이템 가격 텍스트

    public Transform tooltipTransform; // 툴팁 위치

    private void Awake()
    {
        // 아이템 정보 입력
        ShopItemInfoEnter();
    }

    public void BuyItem()
    {
        // 1. 돈이 부족할 떄
        if (GameManager.instance.gold < itemPrice)
        {
            // 구매 실패 효과음 재생
            SoundManager.instance.PlaySound("CantBuyClick");
        }
        // 2. 돈이 충분할 때
        else
        {
            // 아이템 획득
            for (int i = 0; i < GameManager.instance.iv.slots.Length; i++)
            {
                if (GameManager.instance.iv.slots[i].item == null)
                {
                    // 돈 지불
                    GameManager.instance.gold -= itemPrice;

                    // 구매 효과음 재생
                    SoundManager.instance.PlaySound("HandleCoins");

                    GameManager.instance.iv.slots[i].AddItem(item);
                    Debug.Log($"{item.itemName}아이템을 구매했다!");

                    // 상점 슬롯 비활성화 (판매 완료) 
                    gameObject.SetActive(false);

                    CloseTooltip(); // 툴팁 닫기

                    return;
                }
            }

            Debug.Log("아이템 가득 참");
            // 구매 실패 효과음 재생
            SoundManager.instance.PlaySound("CantBuyClick");
        }
    }

    // 품목 아이템 정보 입력
    public void ShopItemInfoEnter()
    {
        // 이미지
        itemImage.sprite = item.itemImage;

        // 이름
        switch (item.itemClass) // 등급에 따른 아이템 이름과 색깔 변경
        {
            case ItemData.ItemClass.Common:
                itemNameText.text = $"<color=#FFFFFF>{item.itemName}</color>"; // 일반
                break;
            case ItemData.ItemClass.Rare:
                itemNameText.text = $"<color=#FF7500>{item.itemName}</color>"; // 레어
                break;
            case ItemData.ItemClass.Epic:
                itemNameText.text = $"<color=#FF00AE>{item.itemName}</color>"; // 에픽
                break;
        }

        // 가격
        itemPriceText.text = itemPrice.ToString();
    }

    // 포인터가 접촉 시 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.instance.ShowTooltip(item, tooltipTransform); // 툴팁 활성화
    }

    // 포인터가 나가면 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        CloseTooltip(); // 툴팁 닫기
    }

    // 툴팁 닫기 함수
    void CloseTooltip()
    {
        GameManager.instance.tooltipObject.SetActive(false); // 툴팁 패널 닫기

        GameManager.instance.tooltip_IncreaseMaxHealthObject.SetActive(false); // 최대 체력 증가량 닫기
        GameManager.instance.tooltip_IncreaseAttackDamageObject.SetActive(false); // 공격력 증가량 닫기
        GameManager.instance.tooltip_IncreaseMoveSpeedObject.SetActive(false); // 이동 속도 증가량 닫기
        GameManager.instance.tooltip_IncreaseJumpPowerObject.SetActive(false); // 점프력 증가량 닫기
        GameManager.instance.tooltip_IncreaseCriticalPercentageObject.SetActive(false); // 크리티컬 확률 증가량 닫기
        GameManager.instance.tooltip_IncreaseCriticalValueObject.SetActive(false); // 크리티컬 계수 증가량 닫기
        GameManager.instance.tooltip_IncreaseJumpCountObject.SetActive(false); // 점프 횟수 증가량 닫기
        GameManager.instance.tooltip_IncreaseDashCountObject.SetActive(false); // 대쉬 횟수 증가량 닫기
    }
}
