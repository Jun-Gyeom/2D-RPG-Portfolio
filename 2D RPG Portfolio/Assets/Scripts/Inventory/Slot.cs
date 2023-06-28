using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData item; // 획득한 아이템
    public Image itemImage;  // 아이템의 이미지

    public bool isEquipmentSlot; // 해당 슬롯이 장비 장착 슬롯인지 여부
    public bool isDeathMenuUISlot; // 해당 슬롯이 사망 정보 창 슬롯인지 여부

    // 드래그 앤 드롭
    [SerializeField]
    private Transform _targetTr; // 이동될 UI

    private void Awake()
    {
        // 이동 대상 UI를 지정하지 않은 경우, 자동으로 자신으로 초기화
        if (_targetTr == null)
        {
            _targetTr = this.transform;
        }
    }

    private void Update()
    {
        // 아이템 이미지의 종횡비 유지 설정이 꺼져있다면
        if (!itemImage.preserveAspect)
        {
            itemImage.preserveAspect = true; // 켜기
        }
    }

    // 아이템 이미지의 투명도 조절
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 인벤토리에 새로운 아이템 슬롯 추가
    public void AddItem(ItemData _item)
    {
        item = _item;
        itemImage.sprite = item.itemImage;

        if (isEquipmentSlot)
        {
            // 장비 능력치 Up
            EquipItem();
        }
    
        SetColor(1);
    }

    // 해당 슬롯 하나 삭제
    public void ClearSlot()
    {
        if (isEquipmentSlot)
        {
            // 장비 능력치 제거
            UnequipItem();
        }

        item = null;
        itemImage.sprite = null;
        SetColor(0);
    }

    // 아이템 장착
    private void EquipItem()
    {
        Debug.Log($"장착 전 체력: {PlayerManager.instance.health} " +
            $"\n장착 전 최대체력: {GameManager.instance.player_MaxHealth[GameManager.instance.level] + GameManager.instance.increased_MaxHealth} " +
            $"\n 장착 후 체력: {Mathf.Round((PlayerManager.instance.health * (GameManager.instance.player_MaxHealth[GameManager.instance.level] + GameManager.instance.increased_MaxHealth + item.increase_Health)) / (GameManager.instance.player_MaxHealth[GameManager.instance.level] + GameManager.instance.increased_MaxHealth))} " +
            $"\n 장착 후 최대체력: {(GameManager.instance.player_MaxHealth[GameManager.instance.level] + GameManager.instance.increased_MaxHealth + item.increase_Health)}");

        // 아이템 장착 후 체력 구하기
        PlayerManager.instance.health = Mathf.Round((PlayerManager.instance.health * (GameManager.instance.player_MaxHealth[GameManager.instance.level] + GameManager.instance.increased_MaxHealth + item.increase_Health))
            / (GameManager.instance.player_MaxHealth[GameManager.instance.level] + GameManager.instance.increased_MaxHealth));

        GameManager.instance.increased_MaxHealth += item.increase_Health; // 최대 체력 적용
        GameManager.instance.increased_CriticalPercentage += item.increase_CriticalPercentage; // 치명타 확률 적용
        GameManager.instance.increased_AttackDamage += item.increase_Damage; // 공격력 적용
        GameManager.instance.increased_CriticalValue += item.increase_CriticalValue; // 치명타 계수 적용
        GameManager.instance.increased_MoveSpeed += item.increase_MoveSpeed; // 이동 속도 적용
        GameManager.instance.increased_MaxDashCount += item.increase_DashCount; // 대쉬 횟수 적용
        PlayerManager.instance.dashChargeCount += item.increase_DashCount; // 대쉬 횟수 증가하면서 대쉬 충전량도 같이 증가
        GameManager.instance.increased_JumpPower += item.increase_JumpPower; // 점프력 적용
        GameManager.instance.increased_MaxJump += item.increase_JumpCount; // 점프 횟수 적용
    }

    // 아이템 장착 해제
    private void UnequipItem()
    {
        Debug.Log($"해제 전 체력: {PlayerManager.instance.health} " +
            $"\n해제 전 최대체력: {GameManager.instance.player_MaxHealth[GameManager.instance.level] + GameManager.instance.increased_MaxHealth} " +
            $"\n 해제 후 체력: {Mathf.Round((PlayerManager.instance.health * (GameManager.instance.player_MaxHealth[GameManager.instance.level] + GameManager.instance.increased_MaxHealth - item.increase_Health)) / (GameManager.instance.player_MaxHealth[GameManager.instance.level] + GameManager.instance.increased_MaxHealth))} " +
            $"\n 해제 후 최대체력: {(GameManager.instance.player_MaxHealth[GameManager.instance.level] + GameManager.instance.increased_MaxHealth - item.increase_Health)}");

        // 아이템 장착 해제 후 체력 구하기
        PlayerManager.instance.health = Mathf.Round((PlayerManager.instance.health * (GameManager.instance.player_MaxHealth[GameManager.instance.level] + GameManager.instance.increased_MaxHealth - item.increase_Health))
            / (GameManager.instance.player_MaxHealth[GameManager.instance.level] + GameManager.instance.increased_MaxHealth));

        GameManager.instance.increased_MaxHealth -= item.increase_Health; // 최대 체력 적용
        GameManager.instance.increased_CriticalPercentage -= item.increase_CriticalPercentage; // 치명타 확률 적용
        GameManager.instance.increased_AttackDamage -= item.increase_Damage; // 공격력 적용
        GameManager.instance.increased_CriticalValue -= item.increase_CriticalValue; // 치명타 계수 적용
        GameManager.instance.increased_MoveSpeed -= item.increase_MoveSpeed; // 이동 속도 적용
        GameManager.instance.increased_MaxDashCount -= item.increase_DashCount; // 대쉬 횟수 적용
        GameManager.instance.increased_JumpPower -= item.increase_JumpPower; // 점프력 적용
        GameManager.instance.increased_MaxJump -= item.increase_JumpCount; // 점프 횟수 적용
    }

    // 마우스 드래그가 시작 됐을 때 발생하는 이벤트
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 슬롯에 아이템이 있고 사망 정보 창 슬롯이 아닐 때만
        if (item != null && !isDeathMenuUISlot)
        {
            SetColor(0); // 슬롯 아이템 안 보이게

            DragSlot.instance.dragSlot = this; // 드래그 슬롯에 참조
            DragSlot.instance.DragSetImage(itemImage); // 드래그 슬롯에 이미지 전달
            DragSlot.instance.transform.position = eventData.position; // 드래그 슬롯 위치 변경
        }
    }

    // 마우스 드래그 중일 때 계속 발생하는 이벤트
    public void OnDrag(PointerEventData eventData)
    {
        if (item != null && !isDeathMenuUISlot)
        {
            DragSlot.instance.transform.position = eventData.position; // 이동
        }
           
    }

    // 마우스 드래그가 끝났을 때 발생하는 이벤트
    public void OnEndDrag(PointerEventData eventData)
    {
        if (item != null && !isDeathMenuUISlot)
        {
            SetColor(1); // 슬롯 아이템 보이게

            // 드래그 슬롯의 위치(마우스 커서의 위치)가 인벤토리 밖에 있을 때
            if (DragSlot.instance.transform.localPosition.x < GameManager.instance.invectoryPanel.GetComponent<RectTransform>().rect.xMin)
            {
                GameObject throwItem = ObjectPoolingManager.instance.GetObject(DragSlot.instance.dragSlot.item.objectPoolName); // 아이템 필드에 생성

                Debug.Log("아이템 버림");
                DragSlot.instance.dragSlot.ClearSlot(); // 아이템 버림

                throwItem.transform.position = GameManager.instance.player.transform.position; // 플레이어 위치로 이동
            }

            DragSlot.instance.SetColor(0); // 드래그 슬롯 안 보이게
            DragSlot.instance.dragSlot = null; // 드래그 슬롯 참조 제거
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
        {
            DragSlot.instance.SetColor(0); // 드래그 슬롯 안 보이게
            ChangeSlot();

            // 장비 장착 효과음 재생
            if (isEquipmentSlot)
            {
                SoundManager.instance.PlaySound("EquipItem");
            }
        }
            
    }

    // 아이템 이동과 교환
    private void ChangeSlot()
    {
        ItemData _tempItem = item; // 현재 아이템을 임시 변수에 저장

        AddItem(DragSlot.instance.dragSlot.item); // 자신의 슬롯에 드래그 된 아이템을 저장

        // 자신이 아이템을 가지고 있던 상황이었다면
        if (_tempItem != null)
        {
            // 아이템 바꾸기

            // 드래그 슬롯에 자신의 아이템을 저장
            DragSlot.instance.dragSlot.AddItem(_tempItem);

            if (isEquipmentSlot)
            {
                // 현재 슬롯 장비 장착 해제
                DragSlot.instance.dragSlot.UnequipItem();
            }
            
            if (DragSlot.instance.dragSlot.isEquipmentSlot)
            {
                // 드래그 된 슬롯 장비 장착 해제
                UnequipItem();
            }
        }
        else
        {
            // 아이템 이동
            DragSlot.instance.dragSlot.ClearSlot(); // 드래그 된 슬롯을 비움
        }        
    }

    // 포인터가 접촉 시 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 슬롯에 아이템이 있을 때만 실행
        if (item != null)
        {
            GameManager.instance.ShowTooltip(item, transform); // 툴팁 활성화
        }
    }

    // 포인터가 나가면 호출
    public void OnPointerExit(PointerEventData eventData)
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
