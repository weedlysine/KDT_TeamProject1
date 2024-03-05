using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SunriseSunsetAPI : MonoBehaviour
{
    public InputField serviceKeyInput; // ����Ű �Է� �ʵ�
    public InputField locdateInput; // ��¥ �Է� �ʵ�
    public InputField longitudeInput; // �浵 �Է� �ʵ�
    public InputField latitudeInput; // ���� �Է� �ʵ�
    public Text resultText; // ��� ��� �ؽ�Ʈ

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
            resultText.text = "API ��û ����: " + www.error;
        }
        else
        {
            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(www.downloadHandler.text);
            string sunrise = xmlDoc.GetElementsByTagName("sunrise")[0].InnerText;
            string sunset = xmlDoc.GetElementsByTagName("sunset")[0].InnerText;
            resultText.text = $"����ð�: {sunrise}, �ϸ��ð�: {sunset}";
        }
    }
}
