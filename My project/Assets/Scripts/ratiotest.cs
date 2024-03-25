using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ratiotest : MonoBehaviour
{
    public static ratiotest instance;

    // �� ���� ���� �ػ� ����
    public int scene1Width = 2560;
    public int scene1Height = 1600;
    public int scene2Width = 2560;
    public int scene2Height = 800;

    void Awake()
    {
        // �̱��� ���� ���
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        // �� �ε�� ������ ������ �ػ� ����
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (sceneName == "Start Scene")
        {
            SetResolution(scene1Width, scene1Height);
            Debug.Log("���۾�");
        }
        else if (sceneName == "Furniture")
        {
            SetResolution(scene2Width, scene2Height);
            Debug.Log("����");
        }
    }

    void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, false);
    }
}
