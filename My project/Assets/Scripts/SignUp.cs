using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Net.Sockets;
using TMPro;
using System.Text;
using System;
using UnityEngine.SceneManagement;

public class SignUp : MonoBehaviour
{
    [SerializeField]
    TMP_InputField id_input;
    [SerializeField]
    TMP_InputField password_input;
    [SerializeField]
    TMP_InputField name_input;
    [SerializeField]
    GameObject SignUp_Tab;

    TcpClient client;
    string serverIP = "127.0.0.1";
    string type;
    int port = 8001;
    bool socketReady = false;
    byte[] receiveBuffer = new byte[1024];
    byte[] dataLength;
    NetworkStream stream;

    public void Sign_Up()
    {
        if (socketReady) return;
        try
        {
            client = new TcpClient(serverIP, port);
            if (client.Connected)
            {
                type = "signup";
                string jsonData = "{\"Type\":\"" + type + "\",\"ID\":\"" +id_input.text + "\",\"password\":\"" + password_input.text + "\",\"name\":\"" + name_input.text + "\"}";
                byte[] bytes = Encoding.UTF8.GetBytes(jsonData);

                stream = client.GetStream();

                dataLength = BitConverter.GetBytes(bytes.Length);
                stream.Write(dataLength, 0, dataLength.Length);

                stream.Write(bytes, 0, bytes.Length);
                Debug.Log("Data sent: " + jsonData);
                socketReady = true;


                int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
                Debug.Log(bytesRead);
                string receivedData = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
                Debug.Log("Received from server: " + receivedData);
                if (receivedData == "success")
                    SignUp_Tab.SetActive(false);
                else if (receivedData == "fail")
                    Debug.Log("fail");

            }

        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }
}
