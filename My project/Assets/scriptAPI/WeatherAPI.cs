using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;

public class WeatherAPI : MonoBehaviour
{
    public string serviceKey; // 공공데이터포털에서 받은 인증키
    public string baseUrl; // API base URL
    public string pageNo; // 페이지 번호
    public string numOfRows; // 한 페이지 결과 수
    public string dataType; // 요청 자료 형식 (XML/JSON)
    public string baseDate; // 발표일자
    public string baseTime; // 발표시각
    public string nx; // 예보지점 X 좌표
    public string ny; // 예보지점 Y 좌표
    public Text resultText; // 결과를 출력할 Text 컴포넌트

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
                            resultText.text += "하늘 상태: " + SkyStatus(value) + "\n";
                            break;
                        case "PTY":
                            resultText.text += "강수 형태: " + PrecipitationType(value) + "\n";
                            break;
                        case "RN1":
                            resultText.text += "강수량: " + value + "mm\n";
                            break;
                        case "SNO":
                            resultText.text += "눈의 양: " + value + "cm\n";
                            break;
                        case "UUU":
                        case "VVV":
                            resultText.text += "바람: " + WindStatus(value) + "\n";
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
                return "맑음";
            case "3":
                return "구름 많음";
            case "4":
                return "흐림";
            default:
                return "기상 정보를 가져오지 못했습니다.";
        }
    }

    string PrecipitationType(string value)
    {
        switch (value)
        {
            case "0":
                return "없음";
            case "1":
                return "비";
            case "2":
                return "비/눈";
            case "3":
                return "눈";
            default:
                return "기상 정보를 가져오지 못했습니다.";
        }
    }

    string WindStatus(string value)
    {
        float windValue = float.Parse(value);
        if (windValue > 0)
        {
            return "동쪽 또는 북쪽으로 " + Mathf.Abs(windValue) + "m/s";
        }
        else
        {
            return "서쪽 또는 남쪽으로 " + Mathf.Abs(windValue) + "m/s";
        }
    }
}
