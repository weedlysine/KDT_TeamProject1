using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

public class DayTimeController : MonoBehaviour
{
    // sun�� �θ� ������Ʈ, �¾��� ���߰���ŭ x���� ȸ����Ų��.
    public Transform sunAltitude;
    public Light sun;
    public TextMeshProUGUI dateText;
    public Slider slider;

    private SunData sunData;
    private string serviceKey = "dmhrSq%2BTqlzT%2BnZUeLs4aOLl034z1ORuIrI0GvJjb86PSCTT6ycLhKNmZXrGETGBOBftom48mqszKlqj%2FXMCug%3D%3D";

    void Start()
    {
        // �����̴��� �⺻���� 0.5
        slider.value = 12f;
        // �����̴� �� ���� �̺�Ʈ�� �޼��� ����
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        // �ʱ� ������ �ε� �� �¾� ��ġ ������Ʈ
        FetchSunData();
    }
    // �����̴��� �¾� ȸ��
    void OnSliderValueChanged(float value)
    {
        value = slider.value;
        rotateSunAndPanel(value);
    }

    /// <summary>
    /// �¾��� ����, �� ������ �޾Ƽ� ����
    /// </summary>
    public void FetchSunData()
    {
        string _dateText = dateText.text.Replace("-", "");
        string url = new UriBuilder("http://apis.data.go.kr/B090041/openapi/service/SrAltudeInfoService/getAreaSrAltudeInfo")
        {
            Query = $"ServiceKey={serviceKey}&location=����&locdate={_dateText}"
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

        // ���߰��� ���� �¾� ǥ��
        rotateSunAndPanel(12f);
        // �����̴��� �⺻���� 0.5
        slider.value = 12f;
    }

    // �¾�� �¾籤 �г� ȸ��
    private void rotateSunAndPanel(float value)
    {
        sunAltitude.rotation = Quaternion.Euler(new Vector3(-sunData.altitudeMeridian, 0, 0));
        sun.transform.localRotation = Quaternion.AngleAxis(value / 24 * 360, Vector3.up);

        // �г� ȸ���κ� �����ؾ���.
    }

    // �ð��� ���� �¾��� �� ���
    private int GetAngle(string angle)
    {
        return int.Parse(angle.Split('��')[0]);
    }

    public class SunData
    {
        public int altitudeMeridian; // ���߰�
        public int altitudeNine; // 9���� ��
        public int azimuthNine; // 9���� ������
        public int altitudeTwelve; // 12���� ��
        public int azimuthTwelve; // 12���� ������
        public int altitudeFifteen; // 15���� ��
        public int azimuthFifteen; // 15���� ������
        public int altitudeEighteen; // 18���� ��
        public int azimuthEighteen; // 18���� ������
    }
}
