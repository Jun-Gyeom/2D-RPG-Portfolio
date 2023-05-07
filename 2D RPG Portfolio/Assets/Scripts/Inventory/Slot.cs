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
    public Transform playerTr; // 플레이어 위치
    public PlayerManager pm; // 플레이어 매니저

    private Rect inventoryRect; // 인벤토리 이미지의 Rect 정보

    // 툴팁
    private RectTransform tooltip_rt; // 툴팁 렉트 트랜스폼

    public TMP_Text tooltip_ItemName; // 툴팁 아이템 이름
    public TMP_Text tooltip_ItemDescription; // 툴팁 아이템 설명
    public Image tooltip_ItemImage; // 툴팁 아이템 이미지

    public GameObject tooltip_IncreaseMaxHealthObject; // 최대 체력 증가량 오브젝트
    public GameObject tooltip_IncreaseAttackDamageObject; // 공격력 증가량 오브젝트
    public GameObject tooltip_IncreaseMoveSpeedObject; // 이동 속도 증가량 오브젝트
    public GameObject tooltip_IncreaseJumpPowerObject; // 점프력 증가량 오브젝트
    public GameObject tooltip_IncreaseCriticalPercentageObject; // 크리티컬 확률 증가량 오브젝트
    public GameObject tooltip_IncreaseCriticalValueObject; // 크리티컬 계수 증가량 오브젝트
    public GameObject tooltip_IncreaseJumpCountObject; // 점프 횟수 증가량 오브젝트
    public GameObject tooltip_IncreaseDashCountObject; // 대쉬 횟수 증가량 오브젝트

    public TMP_Text tooltip_IncreaseMaxHealthText; // 최대 체력 증가량 텍스트
    public TMP_Text tooltip_IncreaseAttackDamageText; // 공격력 증가량 텍스트
    public TMP_Text tooltip_IncreaseMoveSpeedText; // 이동 속도 증가량 텍스트
    public TMP_Text tooltip_IncreaseJumpPowerText; // 점프력 증가량 텍스트
    public TMP_Text tooltip_IncreaseCriticalPercentageText; // 크리티컬 확률 증가량 텍스트
    public TMP_Text tooltip_IncreaseCriticalValueText; // 크리티컬 계수 증가량 텍스트
    public TMP_Text tooltip_IncreaseJumpCountText; // 점프 횟수 증가량 텍스트
    public TMP_Text tooltip_IncreaseDashCountText; // 대쉬 횟수 증가량 텍스트

    // 드래그 앤 드롭
    [SerializeField]
    private Transform _targetTr; // 이동될 UI

    private void Awake()
    {
        pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        inventoryRect = transform.parent.parent.parent.parent.GetComponent<RectTransform>().rect;

        tooltip_IncreaseMaxHealthText = tooltip_IncreaseMaxHealthObject.GetComponent<TextMeshProUGUI>();
        tooltip_IncreaseAttackDamageText = tooltip_IncreaseAttackDamageObject.GetComponent<TextMeshProUGUI>();
        tooltip_IncreaseMoveSpeedText = tooltip_IncreaseMoveSpeedObject.GetComponent<TextMeshProUGUI>();
        tooltip_IncreaseJumpPowerText = tooltip_IncreaseJumpPowerObject.GetComponent<TextMeshProUGUI>();
        tooltip_IncreaseCriticalPercentageText = tooltip_IncreaseCriticalPercentageObject.GetComponent<TextMeshProUGUI>();
        tooltip_IncreaseCriticalValueText = tooltip_IncreaseCriticalValueObject.GetComponent<TextMeshProUGUI>();
        tooltip_IncreaseJumpCountText = tooltip_IncreaseJumpCountObject.GetComponent<TextMeshProUGUI>();
        tooltip_IncreaseDashCountText = tooltip_IncreaseDashCountObject.GetComponent<TextMeshProUGUI>();

        // 이동 대상 UI를 지정하지 않은 경우, 자동으로 자신으로 초기화
        if (_targetTr == null)
        {
            _targetTr = this.transform;
        }
    }

    private void Start()
    {
        tooltip_rt = GameManager.instance.tooltipObject.GetComponent<RectTransform>();
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
        GameManager.instance.increased_MaxHealth += item.increase_Health; // 최대 체력 적용
        pm.health += item.increase_Health; // 최대 체력 적용하면서 체력도 같이 증가
        GameManager.instance.increased_CriticalPercentage += item.increase_CriticalPercentage; // 치명타 확률 적용
        GameManager.instance.increased_AttackDamage += item.increase_Damage; // 공격력 적용
        GameManager.instance.increased_CriticalValue += item.increase_CriticalValue; // 치명타 계수 적용
        GameManager.instance.increased_MoveSpeed += item.increase_MoveSpeed; // 이동 속도 적용
        GameManager.instance.increased_MaxDashCount += item.increase_DashCount; // 대쉬 횟수 적용
        pm.dashChargeCount += item.increase_DashCount; // 대쉬 횟수 증가하면서 대쉬 충전량도 같이 증가
        GameManager.instance.increased_JumpPower += item.increase_JumpPower; // 점프력 적용
        GameManager.instance.increased_MaxJump += item.increase_JumpCount; // 점프 횟수 적용
    }

    // 아이템 장착 해제
    private void UnequipItem()
    {
        GameManager.instance.increased_MaxHealth -= item.increase_Health; // 최대 체력 적용
        pm.health -= item.increase_Health; // 최대 체력 적용하면서 체력도 같이 감소
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
        // 슬롯에 아이템이 있을 때만
        if (item != null)
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
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position; // 이동
        }
           
    }

    // 마우스 드래그가 끝났을 때 발생하는 이벤트
    public void OnEndDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            SetColor(1); // 슬롯 아이템 보이게

            // 드래그 슬롯의 위치(마우스 커서의 위치)가 인벤토리 밖에 있을 때
            if (DragSlot.instance.transform.localPosition.x < inventoryRect.xMin)
            {
                GameObject throwItem = ObjectPoolingManager.instance.GetObject(DragSlot.instance.dragSlot.item.objectPoolName); // 아이템 필드에 생성

                Debug.Log("아이템 버림");
                DragSlot.instance.dragSlot.ClearSlot(); // 아이템 버림

                throwItem.transform.position = playerTr.position; // 플레이어 위치로 이동
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
        // 슬롯이 비어있을 때만 실행
        if (item != null)
        {
            ShowTooltip(); // 툴팁 활성화
        }
    }

    // 포인터가 나가면 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.instance.tooltipObject.SetActive(false); // 툴팁 패널 닫기

        tooltip_IncreaseMaxHealthObject.SetActive(false); // 최대 체력 증가량 닫기
        tooltip_IncreaseAttackDamageObject.SetActive(false); // 공격력 증가량 닫기
        tooltip_IncreaseMoveSpeedObject.SetActive(false); // 이동 속도 증가량 닫기
        tooltip_IncreaseJumpPowerObject.SetActive(false); // 점프력 증가량 닫기
        tooltip_IncreaseCriticalPercentageObject.SetActive(false); // 크리티컬 확률 증가량 닫기
        tooltip_IncreaseCriticalValueObject.SetActive(false); // 크리티컬 계수 증가량 닫기
        tooltip_IncreaseJumpCountObject.SetActive(false); // 점프 횟수 증가량 닫기
        tooltip_IncreaseDashCountObject.SetActive(false); // 대쉬 횟수 증가량 닫기
    }

    // 툴팁 활성화 함수
    public void ShowTooltip()
    {
        tooltip_ItemDescription.text = item.itemDescription; // 아이템 설명 변경
        tooltip_ItemImage.sprite = item.itemImage; // 아이템 이미지 변경

        // 등급에 따른 아이템 이름과 색깔 변경
        switch (item.itemClass)
        {
            case ItemData.ItemClass.Common:
                tooltip_ItemName.text = $"<color=#FFFFFF>{item.itemName}</color>"; // 일반
                break;
            case ItemData.ItemClass.Rare:
                tooltip_ItemName.text = $"<color=#FF7500>{item.itemName}</color>"; // 레어
                break;
            case ItemData.ItemClass.Epic:
                tooltip_ItemName.text = $"<color=#FF00AE>{item.itemName}</color>"; // 에픽
                break;
        }

        GameManager.instance.tooltipObject.transform.position = transform.position; // 툴팁 위치를 슬롯 위치로 변경

        // 툴팁이 화면 밖으로 나갈 시 티벗 변경
        if (Mathf.Abs(tooltip_rt.anchoredPosition.y) + Mathf.Abs(tooltip_rt.sizeDelta.y) > Mathf.Abs(inventoryRect.yMin))
        {
            Debug.Log("툴팁 삐져나감");
            tooltip_rt.pivot = new Vector2(1, 0);
        }
        else
        {
            tooltip_rt.pivot = new Vector2(1, 1);
        }

        // 최대 체력 텍스트
        if (item.increase_Health != 0)
        {
            // 최대 체력 증가량이 양수일 때
            if (item.increase_Health > 0)
            {
                tooltip_IncreaseMaxHealthText.text = $"> <color=#24FF00>+{float.Parse(item.increase_Health.ToString("F1"))}</color> 최대 체력"; // 최대 체력 증가량 텍스트 변경 (증가)
            }
            // 최대 체력 증가량이 음수일 때
            else if (item.increase_Health < 0)
            {
                tooltip_IncreaseMaxHealthText.text = $"> <color=#FF4000>{float.Parse(item.increase_Health.ToString("F1"))}</color> 최대 체력"; // 최대 체력 증가량 텍스트 변경 (감소)
            }

            tooltip_IncreaseMaxHealthObject.SetActive(true); // 최대 체력 증가량 띄우기
        }

        // 공격력 증가량 텍스트
        if (item.increase_Damage != 0)
        {
            // 공격력 증가량이 양수일 때
            if (item.increase_Damage > 0)
            {
                tooltip_IncreaseAttackDamageText.text = $"> <color=#24FF00>+{float.Parse(item.increase_Damage.ToString("F1"))}</color> 공격력"; // 공격력 증가량 텍스트 변경 (증가)
            }
            // 공격력 증가량이 음수일 때
            else if (item.increase_Damage < 0)
            {
                tooltip_IncreaseAttackDamageText.text = $"> <color=#FF4000>{float.Parse(item.increase_Damage.ToString("F1"))}</color> 공격력"; // 공격력 증가량 텍스트 변경 (감소)
            }

            tooltip_IncreaseAttackDamageObject.SetActive(true); // 공격력 증가량 띄우기
        }

        // 이동 속도 증가량 텍스트
        if (item.increase_MoveSpeed != 0)
        {
            // 증가량이 양수일 때
            if (item.increase_MoveSpeed > 0)
            {
                tooltip_IncreaseMoveSpeedText.text = $"> <color=#24FF00>+{float.Parse(item.increase_MoveSpeed.ToString("F1"))}</color> 이동 속도"; // 이동 속도 증가량 텍스트 변경 (증가)
            }
            // 증가량이 음수일 때
            else if (item.increase_MoveSpeed < 0)
            {
                tooltip_IncreaseMoveSpeedText.text = $"> <color=#FF4000>{float.Parse(item.increase_MoveSpeed.ToString("F1"))}</color> 이동 속도"; // 이동 속도 증가량 텍스트 변경 (감소)
            }

            tooltip_IncreaseMoveSpeedObject.SetActive(true); // 이동 속도 증가량 띄우기
        }

        // 점프력 증가량 텍스트
        if (item.increase_JumpPower != 0)
        {
            // 증가량이 양수일 때
            if (item.increase_JumpPower > 0)
            {
                tooltip_IncreaseJumpPowerText.text = $"> <color=#24FF00>+{float.Parse(item.increase_JumpPower.ToString("F1"))}</color> 점프력"; // 점프력 증가량 텍스트 변경 (증가)
            }
            // 증가량이 음수일 때
            else if (item.increase_JumpPower < 0)
            {
                tooltip_IncreaseJumpPowerText.text = $"> <color=#FF4000>{float.Parse(item.increase_JumpPower.ToString("F1"))}</color> 점프력"; // 점프력 증가량 텍스트 변경 (감소)
            }

            tooltip_IncreaseJumpPowerObject.SetActive(true); // 점프력 증가량 닫기 띄우기
        }

        // 치명타 확률 증가량 텍스트
        if (item.increase_CriticalPercentage != 0)
        {
            // 증가량이 양수일 때
            if (item.increase_CriticalPercentage > 0)
            {
                tooltip_IncreaseCriticalPercentageText.text = $"> <color=#24FF00>+{float.Parse((item.increase_CriticalPercentage * 100).ToString("F1"))}%</color> 치명타 확률"; // 치명타 확률 증가량 텍스트 변경 (증가)
            }
            // 증가량이 음수일 때
            else if (item.increase_CriticalPercentage < 0)
            {
                tooltip_IncreaseCriticalPercentageText.text = $"> <color=#FF4000>{float.Parse((item.increase_CriticalPercentage * 100).ToString("F1"))}%</color> 치명타 확률"; // 치명타 확률 증가량 텍스트 변경 (감소)
            }

            tooltip_IncreaseCriticalPercentageObject.SetActive(true); // 치명타 확률 증가량 띄우기
        }

        // 치명타 계수 증가량 텍스트
        if (item.increase_CriticalValue != 0)
        {
            // 증가량이 양수일 때
            if (item.increase_CriticalValue > 0)
            {
                tooltip_IncreaseCriticalValueText.text = $"> <color=#24FF00>+{float.Parse(item.increase_CriticalValue.ToString("F1"))}</color> 치명타 피해"; // 치명타 피해 증가량 텍스트 변경 (증가)
            }
            // 증가량이 음수일 때
            else if (item.increase_CriticalValue < 0)
            {
                tooltip_IncreaseCriticalValueText.text = $"> <color=#FF4000>{float.Parse(item.increase_CriticalValue.ToString("F1"))}</color> 치명타 피해"; // 치명타 피해 증가량 텍스트 변경 (감소)
            }

            tooltip_IncreaseCriticalValueObject.SetActive(true); // 치명타 피해 증가량 띄우기
        }

        // 점프 횟수 증가량 텍스트
        if (item.increase_JumpCount != 0)
        {
            // 증가량이 양수일 때
            if (item.increase_JumpCount > 0)
            {
                tooltip_IncreaseJumpCountText.text = $"> <color=#24FF00>+{float.Parse(item.increase_JumpCount.ToString("F1"))}</color> 점프 횟수"; // 점프 횟수 증가량 텍스트 변경 (증가)
            }
            // 증가량이 음수일 때
            else if (item.increase_JumpCount < 0)
            {
                tooltip_IncreaseJumpCountText.text = $"> <color=#FF4000>{float.Parse(item.increase_JumpCount.ToString("F1"))}</color> 점프 횟수"; // 점프 횟수 증가량 텍스트 변경 (감소)
            }

            tooltip_IncreaseJumpCountObject.SetActive(true); // 점프 횟수 증가량 띄우기
        }

        // 대쉬 횟수 증가량 텍스트
        if (item.increase_DashCount != 0)
        {
            // 증가량이 양수일 때
            if (item.increase_DashCount > 0)
            {
                tooltip_IncreaseDashCountText.text = $"> <color=#24FF00>+{float.Parse(item.increase_DashCount.ToString("F1"))}</color> 대쉬 횟수"; // 대쉬 횟수 증가량 텍스트 변경 (증가)
            }
            // 증가량이 음수일 때
            else if (item.increase_DashCount < 0)
            {
                tooltip_IncreaseDashCountText.text = $"> <color=#FF4000>{float.Parse(item.increase_DashCount.ToString("F1"))}</color> 대쉬 횟수"; // 대쉬 횟수 증가량 텍스트 변경 (감소)
            }

            tooltip_IncreaseDashCountObject.SetActive(true); // 대쉬 횟수 증가량 띄우기
        }

        GameManager.instance.tooltipObject.SetActive(true); // 툴팁 패널 띄우기
    }
}
