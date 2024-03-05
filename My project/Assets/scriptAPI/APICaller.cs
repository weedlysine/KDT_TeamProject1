using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class APICaller : MonoBehaviour
{
    public string apiUrl;
    public List<ParameterUI> parameters;
    public float refreshRate;

    private void Start()
    {
        StartCoroutine(CallAPI());
    }

    public void RefreshAllValues()
    {
        StopAllCoroutines();
        StartCoroutine(CallAPI());
    }

    private IEnumerator CallAPI()
    {
        while (true)
        {
            string apiUrlWithParameters = apiUrl + "?";
            foreach (ParameterUI paramUI in parameters)
            {
                if (paramUI.isInput)
                {
                    string inputValue = paramUI.inputUI.inputField != null ? paramUI.inputUI.inputField.text : paramUI.inputUI.directInput; // 입력 필드가 지정되어 있지 않으면 직접 입력한 값을 사용합니다.
                    apiUrlWithParameters += paramUI.parameter + "=" + inputValue + "&";
                }
            }

            using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrlWithParameters))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(webRequest.error);
                }
                else
                {
                    ParseJson(webRequest.downloadHandler.text);
                }

                yield return new WaitForSeconds(refreshRate);
            }
        }
    }

    private void ParseJson(string json)
    {
        Dictionary<string, string> jsonObj = JsonUtility.FromJson<Dictionary<string, string>>(json);

        foreach (ParameterUI paramUI in parameters)
        {
            if (paramUI.isInput)
            {
                continue;
            }

            string value;
            if (jsonObj.TryGetValue(paramUI.parameter, out value))
            {
                if (paramUI.outputUI.uiText)
                {
                    paramUI.outputUI.uiText.text = value;
                }

                float floatValue = float.Parse(value);

                if ((paramUI.outputUI.compareType == CompareType.Greater && floatValue > paramUI.outputUI.compareValue) ||
                    (paramUI.outputUI.compareType == CompareType.Less && floatValue < paramUI.outputUI.compareValue) ||
                    (paramUI.outputUI.compareType == CompareType.Equal && floatValue == paramUI.outputUI.compareValue))
                {
                    Instantiate(paramUI.outputUI.prefabToInstantiate, paramUI.outputUI.instantiatePosition, Quaternion.identity);
                }
            }
        }
    }
}

[System.Serializable]
public class ParameterUI
{
    public string parameter;
    public bool isInput;

    [System.Serializable]
    public class InputUI
    {
        public TMP_InputField inputField;
        public string directInput; // 직접 입력할 값을 저장할 변수를 추가합니다.
    }
    public InputUI inputUI;

    [System.Serializable]
    public class OutputUI
    {
        public TMP_Text uiText;
        public GameObject prefabToInstantiate;
        public Vector3 instantiatePosition;
        public CompareType compareType;
        public float compareValue;
    }
    public OutputUI outputUI;
}

public enum CompareType
{
    Greater,
    Less,
    Equal
}
