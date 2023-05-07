using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHitEffect : MonoBehaviour
{
    public SpriteRenderer sr;
    private void OnEnable()
    {
        Invoke("DestroyEffect", 1f); // 1초 후 이펙트 오브젝트 풀에 반환
    }

    private void Update()
    {
        sr.color = Color.white;
    }

    void DestroyEffect()
    {
        gameObject.SetActive(false);
    }
}
