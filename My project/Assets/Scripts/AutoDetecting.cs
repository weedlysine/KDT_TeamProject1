using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoDetecting : MonoBehaviour
{
    public Slider slider; // 슬라이더
    void Start()
    {
        // 슬라이더 초기값 설정
        slider.value = 0;

        // 슬라이더 클릭 시 토글 함수 호출
        slider.onValueChanged.AddListener(delegate { Toggle(); });
    }

    // 슬라이더 값을 토글하는 함수
    void Toggle()
    {
        // 슬라이더 값이 0이면 1로, 1이면 0으로 변경
        slider.value = (slider.value == 0) ? 1 : 0;
    }


}
