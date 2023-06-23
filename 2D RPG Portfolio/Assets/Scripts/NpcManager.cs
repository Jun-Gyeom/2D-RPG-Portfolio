using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcManager : MonoBehaviour
{
    public enum NpcType // NPC 대화 유형
    {
        common, // 대화의 나열뿐인 일반적인 유형
        Talk, // 이야기 하기가 가능한 유형
        Shop_Town, // 마을 상점 이용이 가능한 유형
        shop_Dungeon // 던전 상점 이용이 가능한 유형
    }

    public GameObject keyUI_F; // F키 UI

    public int id; // NPC ID
    public string npcName; // NPC 이름
    private bool isTrigger; // NPC 근처에 있는지 체크
    public int choiceTalkIndexNumber; // 대화 선택창이 나올 인덱스 번호

    public NpcType type; // NPC 대화 유형

    private void Update()
    {
        keyUI_F.SetActive(isTrigger); // 머리 위 F버튼 UI

        // NPC 근처에서 F키를 눌렀을 때, 다만 대화 중이 아니고 일시정지 또는 인벤토리, 상점이 열려있지 않고 죽지 않았을 때만
        if (Input.GetKeyDown(KeyCode.F) && isTrigger
            && !GameManager.instance.isTalk && !GameManager.instance.activeEscMenu && !GameManager.instance.activeInventoty && GameManager.instance.townShopPanel && GameManager.instance.townShopPanel && !GameManager.instance.isDie)
        {
            // 현재 대화 중인 NPC 정보 넘겨주기
            GameManager.instance.currentNpcId = id;
            GameManager.instance.currentNpcName = npcName;
            GameManager.instance.currentChoiceTalkIndex = choiceTalkIndexNumber;

            // NPC 유형에 따른 정보전달
            switch (type)
            {
                case NpcType.Talk: // 이야기 하기가 가능한 NPC
                    GameManager.instance.choiceBtn_Talk.SetActive(true);
                    GameManager.instance.choiceBtn_TownShop.SetActive(false);
                    GameManager.instance.choiceBtn_DungeonShop.SetActive(false);
                    break;
                case NpcType.Shop_Town: // 마을 상점 이용이 가능한 NPC
                    GameManager.instance.choiceBtn_Talk.SetActive(false);
                    GameManager.instance.choiceBtn_TownShop.SetActive(true);
                    GameManager.instance.choiceBtn_DungeonShop.SetActive(false);
                    break;
                case NpcType.shop_Dungeon: // 던전 상점 이용이 가능한 NPC
                    GameManager.instance.choiceBtn_Talk.SetActive(false);
                    GameManager.instance.choiceBtn_TownShop.SetActive(false);
                    GameManager.instance.choiceBtn_DungeonShop.SetActive(true);
                    break;
            }

            GameManager.instance.Talk(id, npcName); // 대화 불러오기
            GameManager.instance.isTalk = true; // 대화 중
        }
    }

    // 트리거가 처음 발동될 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 트리거를 발동시킨 오브젝트의 태그가 플레이어일 때
        if (collision.gameObject.CompareTag("Player"))
        {
            isTrigger = true; // 트리거 활성화
        }
    }

    // 트리거의 발동이 꺼질 때
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 트리거를 발동시킨 오브젝트의 태그가 플레이어일 때
        if (collision.gameObject.CompareTag("Player"))
        {
            isTrigger = false; // 트리거 활성화
        }
    }
}
