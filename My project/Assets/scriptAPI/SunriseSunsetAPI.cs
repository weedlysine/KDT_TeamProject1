using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SunriseSunsetAPI : MonoBehaviour
{
    public InputField serviceKeyInput; // 서비스키 입력 필드
    public InputField locdateInput; // 날짜 입력 필드
    public InputField longitudeInput; // 경도 입력 필드
    public InputField latitudeInput; // 위도 입력 필드
    public Text resultText; // 결과 출력 텍스트

    private string baseUrl = "http://apis.data.go.kr/B090041/openapi/service/RiseSetInfoService/getAreaRiseSetInfo";

    public void FetchData()
    {
        string url = $"{baseUrl}?ServiceKey={serviceKeyInput.text}&locdate={locdateInput.text}&longitude={longitudeInput.text}&latitude={latitudeInput.text}&dnYn=Y";
        StartCoroutine(GetSunriseSunsetData(url));
    }

    IEnumerator GetSunriseSunsetData(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            resultText.text = "API 요청 실패: " + www.error;
        }
        else
        {
            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(www.downloadHandler.text);
            string sunrise = xmlDoc.GetElementsByTagName("sunrise")[0].InnerText;
            string sunset = xmlDoc.GetElementsByTagName("sunset")[0].InnerText;
            resultText.text = $"일출시간: {sunrise}, 일몰시간: {sunset}";
        }
    }
}
