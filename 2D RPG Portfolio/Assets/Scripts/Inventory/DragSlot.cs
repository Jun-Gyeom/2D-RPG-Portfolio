using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    static public DragSlot instance;
    public Slot dragSlot;

    [SerializeField]
    public Image imageItem;

    void Start()
    {
        instance = this;
    }

    private void Update()
    {
        // 아이템 이미지의 종횡비 유지 설정이 꺼져있다면
        if (!imageItem.preserveAspect)
        {
            imageItem.preserveAspect = true; // 켜기
        }
    }

    public void DragSetImage(Image _itemImage)
    {
        imageItem.sprite = _itemImage.sprite;
        SetColor(1);
    }

    public void SetColor(float _alpha)
    {
        Color color = imageItem.color;
        color.a = _alpha;
        imageItem.color = color;
    }
}
