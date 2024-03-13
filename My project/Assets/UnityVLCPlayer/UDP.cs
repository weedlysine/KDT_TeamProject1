using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Text;

public class UDP : MonoBehaviour
{
    private UdpClient udpClient;
    private IPEndPoint endPoint;
    // Start is called before the first frame update
    void Start()
    {
        // 소켓 설정
        udpClient = new UdpClient(8000);
        endPoint = new IPEndPoint(IPAddress.Any, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (udpClient.Available > 0)
        {
            byte[] receivedData = udpClient.Receive(ref endPoint);
            string message = Encoding.UTF8.GetString(receivedData);
            Debug.Log("수신된 데이터: " + message);
        }
    }

    void OnDestroy()
    {
        // 소켓 닫기
        udpClient.Close();
    }
}
