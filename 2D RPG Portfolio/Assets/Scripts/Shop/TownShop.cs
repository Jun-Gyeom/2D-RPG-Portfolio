using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownShop : MonoBehaviour
{
    public ShopSlot[] shopslots;

    public List<GameObject> shopItemList;

    // 재입고 함수
    public void Restock()
    {
        // 모든 상점 아이템 품목 활성화
        for (int i = 0; i < shopItemList.Count; i++)
        {
            shopItemList[i].SetActive(true);
        }
    }
}
