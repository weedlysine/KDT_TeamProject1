using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;

public class WeatherAPI : MonoBehaviour
{
    public string serviceKey; // �������������п��� ���� ����Ű
    public string baseUrl; // API base URL
    public string pageNo; // ������ ��ȣ
    public string numOfRows; // �� ������ ��� ��
    public string dataType; // ��û �ڷ� ���� (XML/JSON)
    public string baseDate; // ��ǥ����
    public string baseTime; // ��ǥ�ð�
    public string nx; // �������� X ��ǥ
    public string ny; // �������� Y ��ǥ
    public Text resultText; // ����� ����� Text ������Ʈ

    void Start()
    {
        string url = $"{baseUrl}?ServiceKey={serviceKey}&pageNo={pageNo}&numOfRows={numOfRows}&dataType={dataType}&base_date={baseDate}&base_time={baseTime}&nx={nx}&ny={ny}";
        StartCoroutine(GetWeatherData(url));
    }

    IEnumerator GetWeatherData(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                resultText.text = "Error: " + www.error;
            }
            else
            {
                var json = JSON.Parse(www.downloadHandler.text);
                var items = json["response"]["body"]["items"]["item"].AsArray;

                foreach (JSONNode item in items)
                {
                    string category = item["category"];
                    string value = item["obsrValue"];
                    switch (category)
                    {
                        case "SKY":
                            resultText.text += "�ϴ� ����: " + SkyStatus(value) + "\n";
                            break;
                        case "PTY":
                            resultText.text += "���� ����: " + PrecipitationType(value) + "\n";
                            break;
                        case "RN1":
                            resultText.text += "������: " + value + "mm\n";
                            break;
                        case "SNO":
                            resultText.text += "���� ��: " + value + "cm\n";
                            break;
                        case "UUU":
                        case "VVV":
                            resultText.text += "�ٶ�: " + WindStatus(value) + "\n";
                            break;
                    }
                }
            }
        }
    }

    string SkyStatus(string value)
    {
        switch (value)
        {
            case "1":
                return "����";
            case "3":
                return "���� ����";
            case "4":
                return "�帲";
            default:
                return "��� ������ �������� ���߽��ϴ�.";
        }
    }

    string PrecipitationType(string value)
    {
        switch (value)
        {
            case "0":
                return "����";
            case "1":
                return "��";
            case "2":
                return "��/��";
            case "3":
                return "��";
            default:
                return "��� ������ �������� ���߽��ϴ�.";
        }
    }

    string WindStatus(string value)
    {
        float windValue = float.Parse(value);
        if (windValue > 0)
        {
            return "���� �Ǵ� �������� " + Mathf.Abs(windValue) + "m/s";
        }
        else
        {
            return "���� �Ǵ� �������� " + Mathf.Abs(windValue) + "m/s";
        }
    }
}
