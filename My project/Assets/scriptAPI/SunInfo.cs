using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Xml;

public class SunInfo : MonoBehaviour
{
    public string date;
    public string location;
    public Text sunriseText;
    public Text sunsetText;

    private string serviceKey = "APP6ROxmgP6c%2Bn%2BHTQ5Run55txbWqk0yDPvRAA4dTliOR4hulYi2jeFmmebFB7WcUiZeHDtqeo1yVb1WBfZIzQ%3D%3D";
    private string url = "http://apis.data.go.kr/B090041/openapi/service/RiseSetInfoService/getAreaRiseSetInfo";

    void Start()
    {
        string finalUrl = $"{url}?ServiceKey={serviceKey}&locdate={date}&location={location}";
        StartCoroutine(GetSunriseSunset(finalUrl));
    }

    IEnumerator GetSunriseSunset(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(request.downloadHandler.text);
                XmlNodeList sunriseNodeList = xmlDoc.GetElementsByTagName("sunrise");
                XmlNodeList sunsetNodeList = xmlDoc.GetElementsByTagName("sunset");

                Debug.Log(request.downloadHandler.text);

                string sunrise = sunriseNodeList[0].InnerText;
                string sunset = sunsetNodeList[0].InnerText;

                sunriseText.text = sunrise;
                sunsetText.text = sunset;

                Debug.Log("Sunrise: " + sunrise);
                Debug.Log("Sunset: " + sunset);
            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }
}
