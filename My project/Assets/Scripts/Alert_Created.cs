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
        float duration = 0.7f; // �������� ����� �ð� (��)
        float moveDistance = 200f; // ������ �Ÿ�

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + Vector3.up * moveDistance; // ���� �̵��� ��ġ

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ���� ��ġ ����
        transform.position = targetPosition;
    }

    public void detroy_button()
    {
        Destroy(this, 0.3f);
    }
}
