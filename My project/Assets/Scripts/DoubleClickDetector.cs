using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClickDetector : MonoBehaviour, IPointerClickHandler
{
    private float lastClickTime = 0f;
    private float doubleClickDelay = 0.5f; // 더블클릭으로 인식할 시간 간격 (초)
    Vector2 defaultSize = new Vector2(500, 300); // 기본 크기
    Vector2 wideSize = new Vector2(1920, 1080); // 확대 크기
    Vector2 defaultPosition = new Vector2(-262, -176); // 기본 위치
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
            // 확대된 상태에서 기본 상태로 서서히 변환
            float t = (Time.time - lastClickTime) / resizeDuration;
            this.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(wideSize, defaultSize, t);
            this.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(widePosition, defaultPosition, t);
        }
        else
        {
            // 기본 상태에서 확대된 상태로 서서히 변환
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
            // 첫 번째 클릭
            lastClickTime = Time.time;
        }
    }
}
