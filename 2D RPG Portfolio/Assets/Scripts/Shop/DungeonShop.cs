using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonShop : MonoBehaviour
{
    public ShopSlot[] shopslots;

    // 상점 아이템 품목 리스트
    public List<GameObject> shopItemList;

    // 등장 아이템 리스트
    public List<ItemData> common_ItemList; // 일반 등급 아이템 리스트
    public List<ItemData> rare_ItemList; // 레어 등급 아이템 리스트
    public List<ItemData> epic_ItemList; // 에픽 등급 아이템 리스트

    public List<ItemData> targetList; // 상점에 등장할 아이템을 뽑을 리스트
    public List<ItemData> chosenItemList = new List<ItemData>(); // 이미 뽑혀서 상점에 등록된 아이템 리스트 (중복 등장을 막기 위함)

    public int minItemAmount; // 상점에 등장할 최소 아이템 개수
    public int maxItemAmount; // 상점에 등장할 최대 아이템 개수

    private int minItemPrice; // 상점 품목의 최소 가격
    private int maxItemPrice; // 상점 품목의 최대 가격

    public int minItemPrice_Common; // 일반 아이템의 최소 가격
    public int maxItemPrice_Common; // 일반 아이템의 최대 가격
    public int minItemPrice_Rare; // 레어 아이템의 최소 가격
    public int maxItemPrice_Rare; // 레어 아이템의 최대 가격
    public int minItemPrice_Epic; // 에픽 아이템의 최소 가격
    public int maxItemPrice_Epic; // 에픽 아이템의 최대 가격

    private bool isChosen; // 이미 뽑힌 아이템인지 체크

    // 랜덤 재입고 함수
    public void RandomRestock()
    {
        // 상점 품목 초기화 (비활성화)
        ResetShopItem();

        // 중복 아이템 리스트 비우기
        chosenItemList.Clear();

        // 상점 판매 품목 수 랜덤하게 뽑기
        int rand_shopItemAmount = Random.Range(minItemAmount, maxItemAmount + 1);

        // 상점 판매 품목 수 만큼 아이템 뽑기
        for (int i = 0; i < rand_shopItemAmount; i++)
        {
            float rand_ItemClass = Random.Range(0f, 1f);

            // 등급마다 다른 확률
            if (rand_ItemClass < 0.6f) // 일반 (60%) 
            {
                Debug.Log("일반 등급");
                targetList = common_ItemList; // 타겟 리스트에 일반 등급 대입
            }
            else if (rand_ItemClass < 0.95f) // 레어 (35%) 
            {
                Debug.Log("레어 등급");
                targetList = rare_ItemList; // 타겟 리스트에 레어 등급 대입
            }
            else if (rand_ItemClass < 1.0f) // 에픽 (5%) 
            {
                Debug.Log("에픽 등급");
                targetList = epic_ItemList; // 타겟 리스트에 에픽 등급 대입
            }

            int rand_Item = Random.Range(0, targetList.Count); // 타겟 등급 리스트의 크기보다 작은 난수 구하기

            isChosen = false; // 중복 체크 초기화

            // 중복 검사
            for (int k = 0; k < chosenItemList.Count; k++)
            {
                // 뽑은 아이템이 이미 등장했던 아이템 리스트에 있다면
                if (targetList[rand_Item] == chosenItemList[k])
                {
                    isChosen = true; // 중복 아이템 체크
                    i--; // 아이템을 추가하지 못했으므로, 한 번 더 뽑도록 함
                    break; // 중복 검사 반복문 탈출
                }
            }

            // 중복 아이템이 아닐 때
            if (!isChosen)
            {
                shopItemList[i].GetComponent<ShopSlot>().item = targetList[rand_Item]; // 상점 품목 아이템에 등록
                chosenItemList.Add(targetList[rand_Item]); // 뽑은 아이템 리스트에 등록

                // 등급마다 가격의 정도가 다름
                switch (targetList[rand_Item].itemClass)
                {
                    case ItemData.ItemClass.Common: // 일반 등급
                        minItemPrice = minItemPrice_Common;
                        maxItemPrice = maxItemPrice_Common;
                        break;
                    case ItemData.ItemClass.Rare: // 레어 등급
                        minItemPrice = minItemPrice_Rare;
                        maxItemPrice = maxItemPrice_Rare;
                        break;
                    case ItemData.ItemClass.Epic: // 에픽 등급
                        minItemPrice = minItemPrice_Epic;
                        maxItemPrice = maxItemPrice_Epic;
                        break;
                }

                int rand_Price = Random.Range(minItemPrice / 10, maxItemPrice / 10) * 10; // 가격 난수 구하기 - 10단위를 곱하여 한 자릿수가 나오지 않게 함.
                shopslots[i].itemPrice = rand_Price;

                shopslots[i].ShopItemInfoEnter(); // 품목 정보 재입력

                shopItemList[i].SetActive(true); // 품목 활성화
            }
        }
    }

    // 품목 비활성화 함수
    void ResetShopItem()
    {
        for (int i = 0; i < shopItemList.Count; i++)
        {
            shopItemList[i].SetActive(false);
        }
    }
}
