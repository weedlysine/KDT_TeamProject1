using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoDetecting : MonoBehaviour
{
    public Slider slider; // �����̴�
    void Start()
    {
        // �����̴� �ʱⰪ ����
        slider.value = 0;

        // �����̴� Ŭ�� �� ��� �Լ� ȣ��
        slider.onValueChanged.AddListener(delegate { Toggle(); });
    }

    // �����̴� ���� ����ϴ� �Լ�
    void Toggle()
    {
        // �����̴� ���� 0�̸� 1��, 1�̸� 0���� ����
        slider.value = (slider.value == 0) ? 1 : 0;
    }


}
