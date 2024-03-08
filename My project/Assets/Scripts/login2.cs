using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class login2 : MonoBehaviour
{
    [SerializeField]
    TMP_InputField id_input;
    [SerializeField]
    TMP_InputField password_input;

    TcpClient client;
    string serverIP = "127.0.0.1";
    int port = 8001;
    bool socketReady = false;
    byte[] receiveBuffer = new byte[1024];
    byte[] dataLength;
    NetworkStream stream;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void login()
    {
        if (socketReady) return;
        try
        {
            client = new TcpClient(serverIP, port);
            if (client.Connected)
            {
                string jsonData = "{\"ID\":\"" + id_input.text + "\",\"password\":\"" + password_input.text + "\"}";
                byte[] bytes = Encoding.UTF8.GetBytes(jsonData);
                
                stream = client.GetStream();

                // 데이터 길이를 보내기
                dataLength = BitConverter.GetBytes(bytes.Length);
                stream.Write(dataLength, 0, dataLength.Length);

                // 실제 데이터를 보내기
                stream.Write(bytes, 0, bytes.Length);
                Debug.Log("Data sent: " + jsonData);
                socketReady = true;

                
                int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
                Debug.Log(bytesRead);
                string receivedData = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
                Debug.Log("Received from server: " + receivedData);
                if (receivedData == "true")
                    SceneManager.LoadScene(1);
            }

        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }
}
