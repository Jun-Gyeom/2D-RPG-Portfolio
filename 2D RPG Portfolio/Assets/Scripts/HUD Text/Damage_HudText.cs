using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage_HudText : MonoBehaviour
{
    public TextMesh hudText; // 허드텍스트
    Color alpha; // 투명도

    public float moveSpeed; // 허드텍스트가 움직이는 속도
    public float alphaSpeed; // 허드텍스트가 투명해지는 속도
    public int damage; // 텍스트에 반영할 대미지

    void Awake()
    {
        hudText = GetComponent<TextMesh>();
        alpha = hudText.color;
    }

    void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0)); // 이동속도 만큼 이동
        alpha.a = Mathf.Lerp(alpha.a, 0, alphaSpeed * Time.deltaTime); // 선형보간을 이용해서 부드럽게 투명도 조절
        hudText.color = alpha; // 투명도 반영
    }
    private void OnEnable()
    {
        transform.parent.gameObject.transform.position = new Vector3(0, 0, 0); // 부모 오브젝트 위치 초기화
        transform.position = new Vector3(0, 0, 0); // 텍스트 위치 초기화
        alpha.a = 255; // 투명도 초기화

        Invoke("DestroyText", 5f);
    }

    public void ShowDamageText(int damage)
    {
        hudText.text = damage.ToString(); // 텍스트 대미지 반영
    }

    public void DestroyText()
    {
        this.gameObject.transform.parent.gameObject.SetActive(false);
    }
}
