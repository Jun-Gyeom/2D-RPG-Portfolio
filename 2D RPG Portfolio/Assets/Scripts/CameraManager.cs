using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    private GameObject player; // 플레이어 오브젝트
    private Transform targetTransform; // 카메라가 따라다닐 오브젝트(플레이어)의 위치
    private CinemachineVirtualCamera virtualCam; // 시네머신 카메라

    void Start()
    {
        virtualCam = GetComponent<CinemachineVirtualCamera>();
        player = null;
    }

    private void Update()
    {
        // 플레이어 오브젝트가 null이면
        if (player == null)
        {
            // 변수 할당
            player = GameObject.FindWithTag("Player");
        }
        // 플레이어 오브젝트가 있다면
        else
        {
            // 시네머신 카메라의 Follow를 플레이어의 위치로 변경
            virtualCam.Follow = player.transform;
        }
    }
}
