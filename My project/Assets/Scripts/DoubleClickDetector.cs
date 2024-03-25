using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClickDetector : MonoBehaviour, IPointerClickHandler
{
    private float lastClickTime = 0f;
    private float doubleClickDelay = 0.5f; // ����Ŭ������ �ν��� �ð� ���� (��)
    Vector2 defaultSize = new Vector2(500, 300); // �⺻ ũ��
    Vector2 wideSize = new Vector2(1920, 1080); // Ȯ�� ũ��
    Vector2 defaultPosition = new Vector2(-262, -176); // �⺻ ��ġ
    Vector2 widePosition = new Vector2(-960, -540);
    bool wideview = false;
    float resizeDuration = 0.5f;

    void Update()
    {
        if (Time.time - lastClickTime >= doubleClickDelay && wideview != !wideview)
        {
            return;
        }

        if (wideview)
        {
            // Ȯ��� ���¿��� �⺻ ���·� ������ ��ȯ
            float t = (Time.time - lastClickTime) / resizeDuration;
            this.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(wideSize, defaultSize, t);
            this.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(widePosition, defaultPosition, t);
        }
        else
        {
            // �⺻ ���¿��� Ȯ��� ���·� ������ ��ȯ
            float t = (Time.time - lastClickTime) / resizeDuration;
            this.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(defaultSize, wideSize, t);
            this.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(defaultPosition, widePosition, t);
        }
    }

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
