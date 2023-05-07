using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetGoldHudText_Shadow : MonoBehaviour
{
    TextMesh hudText; // 허드텍스트
    Color alpha; // 투명도

    public float alphaSpeed; // 허드텍스트가 투명해지는 속도
    public int gold; // 텍스트에 반영할 골드 획득량

    void Awake()
    {
        hudText = GetComponent<TextMesh>();
        alpha = hudText.color;
    }
    void Update()
    {
        alpha.a = Mathf.Lerp(alpha.a, 0, alphaSpeed * Time.deltaTime); // 선형보간을 이용해서 부드럽게 투명도 조절
        hudText.color = alpha; // 투명도 반영
    }
    private void OnEnable()
    {
        alpha.a = 255; // 투명도 초기화
    }

    public void ShowGetGoldText(int gold)
    {
        hudText.text = $"+{gold.ToString()}G"; // 텍스트 골드 획득량 반영
    }
}
