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


    private string url = "http://apis.data.go.kr/B090041/openapi/service/RiseSetInfoService/getAreaRiseSetInfo?ServiceKey=APP6ROxmgP6c%2Bn%2BHTQ5Run55txbWqk0yDPvRAA4dTliOR4hulYi2jeFmmebFB7WcUiZeHDtqeo1yVb1WBfZIzQ%3D%3D";

    void Start()
    {
        string finalUrl = $"{url}&locdate={date}&location={location}";
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

                Debug.Log("Response: " + request.downloadHandler.text);


                XmlNodeList itemList = xmlDoc.SelectNodes("/response/body/items/item");
                if (itemList.Count > 0)
                {
                    XmlNode sunriseNode = itemList[0].SelectSingleNode("sunrise");
                    XmlNode sunsetNode = itemList[0].SelectSingleNode("sunset");

                    if (sunriseNode != null && sunsetNode != null)
                    {
                        string sunrise = sunriseNode.InnerText;
                        string sunset = sunsetNode.InnerText;

                        sunriseText.text = sunrise;
                        sunsetText.text = sunset;

                        Debug.Log("Sunrise: " + sunrise);
                        Debug.Log("Sunset: " + sunset);
                    } 
                    else
                    {
                        Debug.Log("No sunrise or sunset information found.");
                    }
                }
                else
                {
                    Debug.Log("No item found in the response.");
                }
            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }



}
