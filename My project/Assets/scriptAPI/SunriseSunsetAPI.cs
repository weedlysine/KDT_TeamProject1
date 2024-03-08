using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Xml;
using UnityEngine.UI; // UI 컴포넌트를 사용하
using TMPro;

public class SunriseSunsetAPI : MonoBehaviour
{
    public TMP_Text sunriseText; // Inspector에서 할당
    public TMP_Text sunsetText; // Inspector에서 할당

    private string ddbaseUrl = "http://apis.data.go.kr/B090041/openapi/service/RiseSetInfoService/getAreaRiseSetInfo";

    void Start()
    {
        StartCoroutine(GetSunriseSunset("APP6ROxmgP6c%2Bn%2BHTQ5Run55txbWqk0yDPvRAA4dTliOR4hulYi2jeFmmebFB7WcUiZeHDtqeo1yVb1WBfZIzQ%3D%3D", "20240306", "성남"));
    }

    IEnumerator GetSunriseSunset(string serviceKey, string locdate, string location)
    {
        string url = $"{ddbaseUrl}?ServiceKey={serviceKey}&locdate={locdate}&location={WWW.EscapeURL(location)}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                // XML 데이터 처리
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webRequest.downloadHandler.text);
                XmlNode sunriseNode = xmlDoc.SelectSingleNode("//sunrise");
                XmlNode sunsetNode = xmlDoc.SelectSingleNode("//sunset");

                // UI Text에 표시
                if (sunriseNode != null && sunsetNode != null)
                {
                    sunriseText.text = "일출 시간: " + sunriseNode.InnerText.Trim();
                    sunsetText.text = "일몰 시간: " + sunsetNode.InnerText.Trim();
                }
                else
                {
                    Debug.Log("일출 또는 일몰 정보를 찾을 수 없습니다.");
                }
            }
        }
    }
}
