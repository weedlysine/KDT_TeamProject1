using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogIn : MonoBehaviour
{
    [SerializeField]
    TMP_InputField id_input;
    [SerializeField]
    TMP_InputField password_input;
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
    // Start is called before the first frame update

    public void login()
    {
        if (socketReady) return;
        try
        {
            client = new TcpClient(serverIP, port);
            if (client.Connected)
            {
                type = "login";
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                string MAC = NetworkInterface.GetAllNetworkInterfaces()
                  .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                  .Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault().ToString();
                Debug.Log((host.AddressList[1]).ToString());
                Debug.Log(MAC);
                string jsonData = "{\"Type\":\"" + type + "\",\"ID\":\"" + id_input.text + "\",\"password\":\"" +  password_input.text + "\",\"ip\":\"" + (host.AddressList[1]).ToString() + "\",\"MAC\":\"" + MAC + "\"}";
                byte[] bytes = Encoding.UTF8.GetBytes(jsonData);

                stream = client.GetStream();

                dataLength = BitConverter.GetBytes(bytes.Length);
                stream.Write(dataLength, 0, dataLength.Length);

                stream.Write(bytes, 0, bytes.Length);
                Debug.Log("Data sent: " + jsonData);
                socketReady = true;


                int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
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

    public void SignUp_Button()
    {
        SignUp_Tab.SetActive(true);
    }
}