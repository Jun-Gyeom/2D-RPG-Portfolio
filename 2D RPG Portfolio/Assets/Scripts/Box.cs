using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : EntityManager
{
    // 드롭 아이템 관련
    public int minItemDrop; // 최소 드롭 아이템 개수
    public int maxItemDrop; // 최대 드롭 아이템 개수

    public override void TakeDamage(int damage, Transform Pos, bool isCritical)
    {
        base.TakeDamage(damage, Pos, isCritical);
    }

    public override void Die()
    {
        Explode(); // 상자 파괴

        gameObject.SetActive(false);
    }

    // 상자 파괴 함수
    private void Explode()
    {
        // 풀에서 파편 프리팹 생성
        GameObject boxPiece =  ObjectPoolingManager.instance.GetObject("Object_BoxPiece");
        boxPiece.transform.position = transform.position;

        DropItem(); // 아이템 드롭
    }

    // 아이템 드롭 함수
    private void DropItem()
    {
        // 드롭할 아이템 갯수 랜덤 값
        int rand_dropAmount = Random.Range(minItemDrop, maxItemDrop);

        // 드롭할 아이템 갯수만큼 for문 실행
        for (int i = 0; i < rand_dropAmount; i++)
        {
            float rand_dropItem = Random.Range(0f, 1f);

            if (rand_dropItem < 0.95f) // 코인 (95%) 
            {
                Debug.Log("코인 드롭");
                GameObject coin = ObjectPoolingManager.instance.GetObject("Item_Coin"); // 오브젝트 풀에서 코인 대여
                coin.transform.position = this.transform.position; // 위치 초기화
            }
            else if (rand_dropItem < 1.0f) // 금괴 (5%) 
            {
                Debug.Log("금괴 드롭");
                GameObject bullion = ObjectPoolingManager.instance.GetObject("Item_Bullion"); // 오브젝트 풀에서 금괴 대여
                bullion.transform.position = this.transform.position; // 위치 초기화
            }
        }
    }
}
