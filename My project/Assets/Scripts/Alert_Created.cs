using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alert_Created : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveObjectUp());
    }
    IEnumerator MoveObjectUp()
    {
        float elapsedTime = 0f;
        float duration = 0.7f; // 움직임이 진행될 시간 (초)
        float moveDistance = 200f; // 움직일 거리

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + Vector3.up * moveDistance; // 위로 이동할 위치

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종 위치 보정
        transform.position = targetPosition;
    }

    public void detroy_button()
    {
        Destroy(this, 0.3f);
    }
}
