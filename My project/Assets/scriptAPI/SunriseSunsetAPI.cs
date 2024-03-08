using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Xml;
using UnityEngine.UI; // UI ������Ʈ�� �����
using TMPro;

public class SunriseSunsetAPI : MonoBehaviour
{
    public TMP_Text sunriseText; // Inspector���� �Ҵ�
    public TMP_Text sunsetText; // Inspector���� �Ҵ�

    private string ddbaseUrl = "http://apis.data.go.kr/B090041/openapi/service/RiseSetInfoService/getAreaRiseSetInfo";

    void Start()
    {
        StartCoroutine(GetSunriseSunset("APP6ROxmgP6c%2Bn%2BHTQ5Run55txbWqk0yDPvRAA4dTliOR4hulYi2jeFmmebFB7WcUiZeHDtqeo1yVb1WBfZIzQ%3D%3D", "20240306", "����"));
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
                // XML ������ ó��
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webRequest.downloadHandler.text);
                XmlNode sunriseNode = xmlDoc.SelectSingleNode("//sunrise");
                XmlNode sunsetNode = xmlDoc.SelectSingleNode("//sunset");

                // UI Text�� ǥ��
                if (sunriseNode != null && sunsetNode != null)
                {
                    sunriseText.text = "���� �ð�: " + sunriseNode.InnerText.Trim();
                    sunsetText.text = "�ϸ� �ð�: " + sunsetNode.InnerText.Trim();
                }
                else
                {
                    Debug.Log("���� �Ǵ� �ϸ� ������ ã�� �� �����ϴ�.");
                }
            }
        }
    }
}
