using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

public class DayTimeController : MonoBehaviour
{
    // sun의 부모 오브젝트, 태양의 남중고도만큼 x축을 회전시킨다.
    public Transform sunAltitude;
    public Light sun;
    public TextMeshProUGUI dateText;
    public Slider slider;

    private SunData sunData;
    private string serviceKey = "dmhrSq%2BTqlzT%2BnZUeLs4aOLl034z1ORuIrI0GvJjb86PSCTT6ycLhKNmZXrGETGBOBftom48mqszKlqj%2FXMCug%3D%3D";

    void Start()
    {
        // 슬라이더의 기본값은 0.5
        slider.value = 12f;
        // 슬라이더 값 변경 이벤트에 메서드 연결
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        // 초기 데이터 로드 및 태양 위치 업데이트
        FetchSunData();
    }
    // 슬라이더로 태양 회전
    void OnSliderValueChanged(float value)
    {
        value = slider.value;
        rotateSunAndPanel(value);
    }

    /// <summary>
    /// 태양의 위도, 고도 데이터 받아서 저장
    /// </summary>
    public void FetchSunData()
    {
        string _dateText = dateText.text.Replace("-", "");
        string url = new UriBuilder("http://apis.data.go.kr/B090041/openapi/service/SrAltudeInfoService/getAreaSrAltudeInfo")
        {
            Query = $"ServiceKey={serviceKey}&location=서울&locdate={_dateText}"
        }.Uri.ToString();
        Debug.Log(dateText.text);

        sunData = new SunData();

        XDocument xmlDoc = XDocument.Load(url);

        foreach (var node in xmlDoc.Descendants("item"))
        {
            if (node.Element("altitudeMeridian").Value == null)
            {
                return;
            }
            else
            {
                sunData.altitudeMeridian = GetAngle(node.Element("altitudeMeridian").Value);
            }

        }

        // 남중고도일 때의 태양 표현
        rotateSunAndPanel(12f);
        // 슬라이더의 기본값은 0.5
        slider.value = 12f;
    }

    // 태양과 태양광 패널 회전
    private void rotateSunAndPanel(float value)
    {
        sunAltitude.rotation = Quaternion.Euler(new Vector3(-sunData.altitudeMeridian, 0, 0));
        sun.transform.localRotation = Quaternion.AngleAxis(value / 24 * 360, Vector3.up);

        // 패널 회전부분 구현해야함.
    }

    // 시간에 따른 태양의 고도 계산
    private int GetAngle(string angle)
    {
        return int.Parse(angle.Split('˚')[0]);
    }

    public class SunData
    {
        public int altitudeMeridian; // 남중고도
        public int altitudeNine; // 9시의 고도
        public int azimuthNine; // 9시의 방위각
        public int altitudeTwelve; // 12시의 고도
        public int azimuthTwelve; // 12시의 방위각
        public int altitudeFifteen; // 15시의 고도
        public int azimuthFifteen; // 15시의 방위각
        public int altitudeEighteen; // 18시의 고도
        public int azimuthEighteen; // 18시의 방위각
    }
}
