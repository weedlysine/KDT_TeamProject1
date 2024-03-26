using bosqmode.libvlc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CCTV_Control : MonoBehaviour
{
    public GameObject[] cctv_list;
    bool cctv_changing;
    //public string[] cctv_url;
    int cctv_num = 1000;
    VLCPlayerMono vlc;
    // Start is called before the first frame update
    void Start()
    {
        //vlc = gameObject.GetComponent<VLCPlayerMono>();
        //vlc.enabled = true;
        //vlc.url = cctv_url[cctv_num % cctv_list.Length];
        //cctv_list[cctv_num % cctv_list.Length].SetActive(true);
    }

    // Update is called once per frame

    public void buttonclick_left()
    {
        StartCoroutine(cctv_change(-1));
    }

    public void buttonclick_right()
    {
        StartCoroutine(cctv_change(1));
    }

    public void return_origin()
    {
        cctv_list[cctv_num % cctv_list.Length].SetActive(false);
        cctv_num = 1000;
        cctv_list[0].SetActive(true);
    }

    public IEnumerator cctv_change (int a)
    {
        cctv_changing = true;
        cctv_list[cctv_num % cctv_list.Length].SetActive(false);
        cctv_num += a;
        if(a ==1)
        {
            if (cctv_num % cctv_list.Length == 0)
                cctv_num++;
        }
        else
        {
            if (cctv_num % cctv_list.Length == 0)
                cctv_num--;
        }
        //vlc.url = cctv_url[cctv_num % cctv_list.Length];
        cctv_list[cctv_num % cctv_list.Length].SetActive(true);
        yield return new WaitForSeconds(1);
        //vlc.playerUpdate();
        cctv_changing = false;
    }
    public IEnumerator cctv_change_tmp(int a)
    {
        cctv_changing = true;
        cctv_list[cctv_num % cctv_list.Length].SetActive(false);
        cctv_list[a].SetActive(true);
        yield return new WaitForSeconds(1);
        cctv_changing = false;
    }

    public void RunPythonScript()
    {
        try
        {
            Process psi = new Process();
            psi.StartInfo.FileName = "C:/Users/user/AppData/Local/Microsoft/WindowsApps/PythonSoftwareFoundation.Python.3.11_qbz5n2kfra8p0/python.exe";
            UnityEngine.Debug.Log("����1");
            // ������ ���ø����̼� �Ǵ� ����
            psi.StartInfo.Arguments = "C:/Users/user/Documents/GitHub/KDT_TeamProject1/My project/Assets/Scripts/tmptmp.py";
            UnityEngine.Debug.Log("����2");
            // ���� ���۽� ����� �μ�
            psi.StartInfo.CreateNoWindow = true;
            // ��â �ȶ����
            psi.StartInfo.UseShellExecute = false;
            // ���μ����� �����Ҷ� �ü�� ���� �������
            psi.Start();
            UnityEngine.Debug.Log("����3");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Unable to launch app: " + e.Message);
        }
    }
}
