using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneDoor : MonoBehaviour
{
    public Animator anim; // 문 애니메이터
    public BoxCollider2D col; // 콜라이더
    public void DoorOpen()
    {
        // 콜라이더 끄기
        col.enabled = false;

        // 문 열기 애니메이션
        anim.SetBool("isClose", false);

        // 관련 사운드 재생
        SoundManager.instance.PlaySound("StoneDoor");
    }

    public void DoorClose()
    {
        // 콜라이더 켜기
        col.enabled = true;

        // 문 닫기 애니메이션
        anim.SetBool("isClose", true);

        // 관련 사운드 재생
        SoundManager.instance.PlaySound("StoneDoor");
    }
}
