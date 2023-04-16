using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class ShowMessage : MonoBehaviour
{
    static Transform Mess;
    static string Mes = "消息：空";
    static readonly List<string> MessageList = new();
    static bool NewMessage = true;
    public bool Show = true;
    void Awake()
    {
        Mess = GameObject.Find("Canvas").transform.Find("Message").transform.Find("Text");
    }

    void Update()
    {
        if (Show && NewMessage && MessageList.Count > 0)
        {
            Mes = MessageList[0];
            MessageList.Remove(MessageList[0]);
            StartCoroutine(ShowMessage0());
        }
    }
    IEnumerator ShowMessage0()
    {
        Mess.GetComponent<TextMeshProUGUI>().text = Mes;
        NewMessage = false;
        Fade.NewFade("Canvas/Message", 0, 1, 0.2F);
        yield return new WaitForSeconds((float)Mes.Length / 20);
        Fade.NewFade("Canvas/Message", 1, 0, 1.5F);
        yield return new WaitForSeconds(1.2F);
        NewMessage = true;
    }
    public static void Message(string Text)
    {
        MessageList.Add(Text);
    }
    public static void MessageBox(string Text, int BoxWidth = 300, int BoxHeight = 200, int FontSize = 50)
    {
        GameObject.Find("Canvas").transform.Find("MessageBox").transform.Find("Box").transform.Find("Text").GetComponent<Text>().text = Text;
        GameObject.Find("Canvas").transform.Find("MessageBox").transform.Find("Box").GetComponent<RectTransform>()
            .sizeDelta = new Vector2((float)BoxWidth, (float)BoxHeight);
        GameObject.Find("Canvas").transform.Find("MessageBox").transform.Find("Box").transform.Find("Text").GetComponent<RectTransform>()
            .sizeDelta = new Vector2((float)BoxWidth, (float)BoxHeight);
        GameObject.Find("Canvas").transform.Find("MessageBox").transform.Find("Box").transform.Find("Text").GetComponent<Text>().fontSize = FontSize;
        Fade.NewFade("Canvas/MessageBox", 0, 1, 0.5F);
    }
}