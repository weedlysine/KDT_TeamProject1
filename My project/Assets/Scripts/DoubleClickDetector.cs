using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClickDetector : MonoBehaviour, IPointerClickHandler
{
    private float lastClickTime = 0f;
    private float doubleClickDelay = 0.5f; // ����Ŭ������ �ν��� �ð� ���� (��)
    bool wideview = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - lastClickTime < doubleClickDelay)
        {
            if (!wideview)
            {
                this.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);
                this.GetComponent<RectTransform>().anchoredPosition = new Vector2(-960, -540);
                wideview = true;
            }
            else
            {
                this.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 300);
                this.GetComponent<RectTransform>().anchoredPosition = new Vector2(-262, -176);
                wideview = false;
            }
        }
        else
        {
            // ù ��° Ŭ��
            lastClickTime = Time.time;
        }
    }
}
