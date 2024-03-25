using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ratiotest : MonoBehaviour
{
    public static ratiotest instance;

    // 각 씬에 대한 해상도 설정
    public int scene1Width = 2560;
    public int scene1Height = 1600;
    public int scene2Width = 2560;
    public int scene2Height = 800;

    void Awake()
    {
        // 싱글톤 패턴 사용
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        // 씬 로드될 때마다 적절한 해상도 설정
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (sceneName == "Start Scene")
        {
            SetResolution(scene1Width, scene1Height);
            Debug.Log("시작씬");
        }
        else if (sceneName == "Furniture")
        {
            SetResolution(scene2Width, scene2Height);
            Debug.Log("메인");
        }
    }

    void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, false);
    }
}
