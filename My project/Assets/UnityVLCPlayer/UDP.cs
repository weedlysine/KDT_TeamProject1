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

    // Start is called before the first frame update
    void Start()
    {
        // 소켓 설정
        udpClient = new UdpClient(listenPort);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        //receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        while (isRunning)
        {
            try
            {
                // 데이터를 수신하고 송신자의 IP와 포트 번호를 얻음
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpClient.Receive(ref anyIP);

                // 수신한 데이터를 문자열로 변환하여 출력
                string message = Encoding.UTF8.GetString(data);
                Debug.Log("Received data: " + message);
                MainThreadDispatcher.ExecuteOnMainThread(() =>
                {
                    GameObject alert_box = Instantiate(alert_tab,new Vector3(1710, 125,0),Quaternion.identity, parent);
                    alert_box.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = message;
                    alert_box.GetComponent<Button>().onClick.AddListener(() => {
                        Debug.Log("클릭됨");
                        StartCoroutine(gameObject.GetComponent<CCTV_Control>().cctv_change(2));
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
        // 소켓 닫기
        isRunning = false;
        if (udpClient != null)
            udpClient.Close();
    }
}
