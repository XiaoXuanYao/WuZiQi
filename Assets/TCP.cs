using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Text;
using TMPro;

public class TCP : MonoBehaviour
{

    public static Thread serverThread, clientThread, serverWriteThread, clientWriteThread;

    public static IPAddress IP;
    public static int Port;
    public static IPEndPoint ClientEndPoint;
    public static Socket Server, Client;
    public static string thisType = "null";

    public static Queue<KeyValuePair<string, string>> sMessages = new Queue<KeyValuePair<string, string>>()
        , cMessages = new Queue<KeyValuePair<string, string>>();

    public static void Init()
    {
        Port = int.Parse(GameObject.Find("Canvas").transform
            .Find("SignIn").Find("Port").GetComponent<TMP_InputField>().text);
        try
        {
            IP = IPAddress.Parse(GameObject.Find("Canvas").transform
                .Find("SignIn").Find("IP").GetComponent<TMP_InputField>().text);
            ClientEndPoint = new IPEndPoint(IP, Port);
        }
        catch { }
    }

    public static void StartServer()
    {
        Init();
        GameObject.Find("Canvas").transform.Find("SignIn").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Start").gameObject.SetActive(true);
        thisType = "server";
        serverThread = new Thread(() => StartServer(true));
        serverThread.Start();
    }
    public static void StartServer(bool ok)
    {
        try
        {
            ShowMessage.Message("server | 开启服务器");
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Server.Bind(new IPEndPoint(IPAddress.Any, Port));
            Server.Listen(1);
            ShowMessage.Message("server | 等待连接");
            Server = Server.Accept();
            ShowMessage.Message("server | 连接成功");
            Server.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            serverWriteThread = new Thread(() => serverWrite());
            serverWriteThread.Start();
            serverRead();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public static void StartClient()
    {
        Init();
        GameObject.Find("Canvas").transform.Find("SignIn").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Start").gameObject.SetActive(true);
        thisType = "client";
        try
        {
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Client.Connect(ClientEndPoint);
            ShowMessage.Message("client | 连接成功");
            Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            clientWriteThread = new Thread(() => clientWrite());
            clientWriteThread.Start();
            clientThread = new Thread(() => clientRead());
            clientThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }


    public static void serverRead()
    {
        while (true)
        {
            byte[] receive = new byte[1024];
            Server.Receive(receive, 0, receive.Length, 0);
            if (receive.Length > 0)
            {
                string S = Encoding.UTF8.GetString(receive);
                S.Replace(" ", "");
                while (S.Split(']').Length > 1)
                {
                    Debug.Log("server | 接受到：" + S);
                    string S0 = S.Split(']')[0].Split('[')[1];
                    S = S.Substring(S0.Length + 2, S.Length - S0.Length - 2);
                    string Tag = S0.Split('|')[0];
                    string Message = S0.Split('|')[1];
                    switch (Tag)
                    {
                        case "ok":
                            sWriteOk = true;
                            break;
                        default:
                            serverWrite("ok|1");
                            sMessages.Enqueue(new KeyValuePair<string, string>(Tag, Message));
                            break;
                    }
                }
            }
        }
    }

    public static void serverWrite(string mes)
    {
        serverMessages.Enqueue(mes);
    }

    public static Queue<string> serverMessages = new Queue<string>();
    public static bool sWriteOk = true;
    public static void serverWrite()
    {
        while (true)
        {
            if (serverMessages.Count == 0)
                Thread.Sleep(10);
            else
            {
                string S = serverMessages.Dequeue();
                Debug.Log("server | 发送：" + S);
                if (S != "ok|1") sWriteOk = false;
                Server.Send(Encoding.UTF8.GetBytes("[" + S + "]"));
                while (!sWriteOk) Thread.Sleep(5);
            }
        }
    }


    public static void clientRead()
    {
        while (true)
        {
            byte[] receive = new byte[1024];
            Client.Receive(receive, 0, receive.Length, 0);
            if (receive.Length > 0)
            {
                string S = Encoding.UTF8.GetString(receive);
                S.Replace(" ", "");
                while (S.Split(']').Length > 1)
                {
                    Debug.Log("client | 接受到：" + S);
                    string S0 = S.Split(']')[0].Split('[')[1];
                    S = S.Substring(S0.Length + 2, S.Length - S0.Length - 2);
                    string Tag = S0.Split('|')[0];
                    string Message = S0.Split('|')[1];
                    switch (Tag)
                    {
                        case "ok":
                            cWriteOk = true;
                            break;
                        default:
                            clientWrite("ok|1");
                            cMessages.Enqueue(new KeyValuePair<string, string>(Tag, Message));
                            break;
                    }
                }
            }
        }
    }

    public static void clientWrite(string mes)
    {
        clientMessages.Enqueue(mes);
    }

    public static Queue<string> clientMessages = new Queue<string>();
    public static bool cWriteOk = true;
    public static void clientWrite()
    {
        while (true)
        {
            if (clientMessages.Count == 0)
                Thread.Sleep(10);
            else
            {
                string S = clientMessages.Dequeue();
                Debug.Log("client | 发送：" + S);
                if (S != "ok|1") sWriteOk = false;
                Client.Send(Encoding.UTF8.GetBytes("[" + S + "]"));
                while (!cWriteOk) Thread.Sleep(5);
            }
        }
    }


    private void OnDestroy()
    {
        try { Server.Close(); } catch { }
        try { serverWriteThread.Abort(); } catch { }
        try { serverThread.Abort(); } catch { }
        try { Client.Close(); } catch { }
        try { clientWriteThread.Abort(); } catch { }
        try { clientThread.Abort(); } catch { }
    }

}
