using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine.UI;

public class UDP : MonoBehaviour
{
    public int listenPort = 8000;
    private UdpClient udpClient;
    private IPEndPoint endPoint;
    Thread receiveThread;
    bool isRunning = true;
    public GameObject alert_tab;
    public Transform parent;
    public GameObject cctv;

    // Start is called before the first frame update
    void Start()
    {
        // ���� ����
        udpClient = new UdpClient(listenPort);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        //receiveThread.IsBackground = true;
        receiveThread.Start();
        //GameObject alert_box = Instantiate(alert_tab, new Vector3(2410, -100, 0), Quaternion.identity, parent);
        //alert_box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = message;
    }

    private void ReceiveData()
    {
        while (isRunning)
        {
            try
            {
                // �����͸� �����ϰ� �۽����� IP�� ��Ʈ ��ȣ�� ����
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpClient.Receive(ref anyIP);

                // ������ �����͸� ���ڿ��� ��ȯ�Ͽ� ���
                string message = Encoding.UTF8.GetString(data);
                Debug.Log("Received data: " + message);
                MainThreadDispatcher.ExecuteOnMainThread(() =>
                {
                    GameObject alert_box = Instantiate(alert_tab, new Vector3(2410, -100, 0), Quaternion.identity, parent);
                    string tmp = message + "cctv has detected human";
                    alert_box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = tmp;
                    alert_box.GetComponent<Button>().onClick.AddListener(() => {
                        Debug.Log("Ŭ����");
                        Debug.Log(int.Parse(message));
                        StartCoroutine(cctv.GetComponent<CCTV_Control>().cctv_change_tmp(int.Parse(message)));
                        Destroy(alert_box,.5f);
                        });
                });
            }
            catch (System.Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }


    void OnDestroy()
    {
        // ���� �ݱ�
        isRunning = false;
        if (udpClient != null)
            udpClient.Close();
    }
}
