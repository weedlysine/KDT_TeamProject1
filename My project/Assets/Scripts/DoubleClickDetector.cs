using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClickDetector : MonoBehaviour, IPointerClickHandler
{
    public GameObject parent;
    public GameObject tmp;
    CCTV_Control cctvControl;
    private float lastClickTime = 0f;
    private float doubleClickDelay = 0.5f; // 더블클릭으로 인식할 시간 간격 (초)
    Vector2 defaultSize = new Vector2(384, 216); // 기본 크기
    Vector2 wideSize = new Vector2(1920, 1080); // 확대 크기
    Vector2 defaultPosition; // 기본 위치
    Vector2 widePosition = new Vector2(960, -540);
    bool wideview = false;
    float resizeDuration = 0.5f;

    Transform[] children;


    private void Start()
    {
        cctvControl = tmp.GetComponent<CCTV_Control>();
        defaultPosition = this.transform.position;
        children = parent.transform.GetComponentsInChildren<Transform>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - lastClickTime < doubleClickDelay)
        {
            if (!wideview)
            {
                for (int i = 0; i < children.Length;i++)
                {
                    if (children[i].gameObject == parent)
                        continue;
                    if(this.transform != children[i])
                        children[i].gameObject.SetActive(false);
                }
                this.GetComponent<RectTransform>().sizeDelta = wideSize;
                this.GetComponent<RectTransform>().anchoredPosition = widePosition;
                wideview = true;
                switch (this.name)
                {
                    case "cctv1":
                        StartCoroutine(cctvControl.cctv_change_tmp(1));
                        break;
                    case "cctv2":
                        StartCoroutine(cctvControl.cctv_change_tmp(2));
                        break;
                    case "cctv3":
                        StartCoroutine(cctvControl.cctv_change_tmp(3));
                        break;
                    case "cctv4":
                        StartCoroutine(cctvControl.cctv_change_tmp(4));
                        break;
                }
            }
            else
            {
                for (int i = 0; i < children.Length; i++)
                {
                    children[i].gameObject.SetActive(true);
                }
                this.GetComponent<RectTransform>().sizeDelta = new Vector2(384, 216);
                this.GetComponent<RectTransform>().position = defaultPosition;
                wideview = false;
                StartCoroutine(cctvControl.cctv_change_tmp(0));
            }
        }
        else
        {
            // 첫 번째 클릭
            lastClickTime = Time.time;
        }
    }
}
