using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainFunctions : MonoBehaviour
{

    private void Start()
    {
        for (int i = 0; i < 14; i++)
            for (int j = 0; j < 14; j++)
            {
                Transform T = Instantiate(GameObject.Find("Board").transform.Find("Square").transform,
                    GameObject.Find("Board").transform);
                T.transform.position = new Vector2(i - 6.5F, j - 6.5F);
                T.gameObject.SetActive(true);
            }
        float Scale = Screen.width / 1152F;
        GameObject.Find("Canvas").GetComponent<CanvasScaler>().scaleFactor = Scale;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0) && LuoZi.CouldLuo)
        {
            Vector2 Pos = Input.mousePosition;
            Pos = Camera.main.ScreenToWorldPoint(Pos);
            LuoZi.Luo(Pos);
        }

        #region ¿Í»§¶Ë
        if (TCP.sMessages.Count > 0)
        {
            KeyValuePair<string, string> M = TCP.sMessages.Dequeue();
            switch (M.Key)
            {
                case "Start":
                    StartGame(int.Parse(M.Value));
                    break;
                case "Reset":
                    LuoZi.ReSet();
                    break;
                case "LuoZi":
                    LuoZi.AnotherLuo(new Vector2(int.Parse(M.Value.Split(',')[0]), int.Parse(M.Value.Split(",")[1])));
                    break;
                case "win":
                    FinishGame(int.Parse(M.Value));
                    break;
            }
        }
        #endregion

        #region ·þÎñ¶Ë
        if (TCP.cMessages.Count > 0)
        {
            KeyValuePair<string, string> M = TCP.cMessages.Dequeue();
            switch (M.Key)
            {
                case "Start":
                    StartGame(int.Parse(M.Value));
                    break;
                case "Reset":
                    LuoZi.ReSet();
                    break;
                case "LuoZi":
                    LuoZi.AnotherLuo(new Vector2(int.Parse(M.Value.Split(',')[0]), int.Parse(M.Value.Split(",")[1])));
                    break;
                case "win":
                    FinishGame(int.Parse(M.Value));
                    break;
            }
        }
        #endregion

    }

    public static void StartGame(int Turn = -1)
    {
        LuoZi.ReSet();
        bool ok = true;
        if (Turn == -1)
        {
            Turn = (int)(Random.value * 2);
            ok = false;
        }
        if (Turn == 1)
        {
            LuoZi.Type = 1;
            LuoZi.CouldLuo = true;
        }
        else
            LuoZi.Type = 0;
        if (!ok)
        {
            if (TCP.thisType == "server") TCP.serverWrite("Start|" + (1 - Turn));
            else if (TCP.thisType == "client") TCP.clientWrite("Start|" + (1 - Turn));
        }
        GameObject.Find("Canvas").transform.Find("Start").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Background").gameObject.SetActive(false);
    }

    public static void FinishGame(int Winner)
    {
        if (Winner == 1) ShowMessage.Message("    ºÚÆåÊ¤    ");
        else ShowMessage.Message("    °×ÆåÊ¤    ");
        GameObject.Find("Main").GetComponent<MainFunctions>().Invoke("DisplayStart", 2);
    }

    public void DisplayStart()
    {
        GameObject.Find("Canvas").transform.Find("Background").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("Start").gameObject.SetActive(true);
    }

}
