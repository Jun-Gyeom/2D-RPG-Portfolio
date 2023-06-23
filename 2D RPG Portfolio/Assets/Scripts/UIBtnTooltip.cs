using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UIBtnTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text uiTooltipDescriptionText; // UI 툴팁 설명 텍스트

    public string uiTooltipDescription; // UI 툴팁 설명


    // 포인터가 접촉 시 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip(); // 툴팁 활성화
    }

    // 포인터가 나가면 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.instance.uiTooltipObject.SetActive(false); // 툴팁 패널 닫기
    }

    void ShowTooltip()
    {
        GameManager.instance.uiTooltipObject.SetActive(true); // 툴팁 패널 활성화

        GameManager.instance.uiTooltipObject.transform.position = transform.position; // 툴팁 위치 변경
        uiTooltipDescriptionText.text = uiTooltipDescription; // 툴팁 설명 변경
    }
}
