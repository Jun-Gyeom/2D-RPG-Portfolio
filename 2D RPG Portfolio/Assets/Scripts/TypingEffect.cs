using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypingEffect : MonoBehaviour
{
    public int typingPerSeconds; // 초당 타이핑되는 글자 수
    public TMP_Text typingText; // 텍스트 컴포넌트
    public GameObject talkCursor; // 대화창 엔터 커서
    public bool isTyping; // 현재 타이핑이 진행되고 있는지 체크

    Animator talkCursorAnim; // 대화창 엔터 커서 애니메이터
    float intervar; // 다음 타이핑까지의 간격
    string targetMessage; // 타이핑 할 문자
    int typingIndex; // 타이핑 인덱스

    private void Awake()
    {
        // 컴포넌트 할당
        typingText = GetComponent<TMP_Text>();
        talkCursorAnim = talkCursor.GetComponent<Animator>();
    }

    // 문자 초기화 함수
    public void SetMessage(string msg)
    {
        // 타이핑 중에 입력이 들어왔다면
        if (isTyping)
        {
            typingText.text = targetMessage; // 타이핑 스킵
            CancelInvoke("Effecting"); // 타이핑 중지
            EffectEnd(); // 타이핑 종료
        }
        else
        {
            targetMessage = msg; // 타겟 문자 전달
            EffectStart(); // 타이핑 시작
        }
    }

    // 타이핑 시작 함수
    void EffectStart()
    {
        typingText.text = ""; // 타이핑을 위해 텍스트 비우기
        typingIndex = 0; // 타이핑 인덱스 초기화

        intervar = 1.0f / typingPerSeconds; // 다음 타이핑까지의 간격 계산
        Invoke("Effecting", intervar); // 타이핑 진행

        isTyping = true; // 현재 타이핑 진행 중

        talkCursorAnim.SetBool("isBlink", false); // 타이핑 중엔 엔터 커서 깜빡임 애니메이션 중지
    }

    // 타이핑 진행 함수
    void Effecting()
    {
        // 텍스트가 타겟 문자와 같으면 
        if (typingText.text == targetMessage)
        {
            EffectEnd(); // 타이핑 종료
            return;
        }

        typingText.text += targetMessage[typingIndex]; // 텍스트에 타이핑 인덱스의 문자를 더해줌

        // 타이핑 된 문자가 공백 또는 마침표일 경우엔 예외처리
        if (targetMessage[typingIndex] != ' ' || targetMessage[typingIndex] != '.')
        {
            SoundManager.instance.PlaySound("Typing"); // 타이핑 효과음 재생
        }

        typingIndex++; // 타이핑 인덱스 증가

        Invoke("Effecting", intervar); // 재귀함수로 다음 타이핑 진행
    }

    // 타이핑 종료 함수
    void EffectEnd()
    {
        // 대화 선택을 해야한다면
        if (GameManager.instance.isChoiceTalk)
        {
            GameManager.instance.choicePanel.SetActive(true); // 대화 선택창 활성화
        }
        else
        {
            GameManager.instance.choicePanel.SetActive(false); // 대화 선택창 비활성화
        }

        talkCursorAnim.SetBool("isBlink", true); // 엔터 커서 깜빡임 애니메이션 재생

        isTyping = false; // 타이핑 진행 중이 아님
    }
}
