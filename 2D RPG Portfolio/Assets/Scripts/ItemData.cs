using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class ItemData : ScriptableObject
{
    public enum ItemClass
    {
        Common,
        Rare,
        Epic,
    }

    public string itemName; // 아이템 이름
    public string itemDescription; // 아이템 설명
    public ItemClass itemClass; // 아이템 등급
    public Sprite itemImage; // 아이템 이미지
    public string objectPoolName; // 오브젝트 풀에 등록 된 아이템 이름

    public float increase_Health; // 체력 증가량
    public float increase_CriticalPercentage; // 치명타 확률 증가량
    public float increase_CriticalValue; // 치명타 계수 증가량
    public float increase_MoveSpeed; // 이동 속도 증가량
    public int increase_Damage; // 대미지 증가량
    public float increase_JumpPower; // 점프력 증가량
    public int increase_DashCount; // 대쉬 횟수 증가량
    public int increase_JumpCount; // 점프 횟수 증가량
}
