using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuoZi : MonoBehaviour
{

    public static int Type = 0;
    public static bool CouldLuo = false;

    public static int[][] Board = new int[15][];

    public static void Luo(Vector2 Pos)
    {
        int[] Pos2 = new int[2] { (int)Mathf.Round(Pos.x), (int)Mathf.Round(Pos.y) };
        if (Mathf.Abs(Pos2[0]) > 7 || Mathf.Abs(Pos2[1]) > 7 || Board[Pos2[0] + 7][Pos2[1] + 7] != -1)
        {
            ShowMessage.Message("此处已有棋子……");
            return;
        }
        CouldLuo = false;
        Transform T = Instantiate(GameObject.Find("Main").transform.Find("Circle").transform,
                    GameObject.Find("Main").transform);
        if (Type == 0)
            T.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        else
            T.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        T.transform.position = new Vector3(Pos2[0], Pos2[1], -0.1F);
        T.gameObject.SetActive(true);

        if (TCP.thisType == "server") TCP.serverWrite("LuoZi|" + Pos2[0] + "," + Pos2[1]);
        else if (TCP.thisType == "client") TCP.clientWrite("LuoZi|" + Pos2[0] + "," + Pos2[1]);

        Board[Pos2[0] + 7][Pos2[1] + 7] = Type;

        if (Check(new int[2] { Pos2[0] + 7, Pos2[1] + 7 }))
        {
            MainFunctions.FinishGame(Type);
            if (TCP.thisType == "server") TCP.serverWrite("win|" + Type);
            else if (TCP.thisType == "client") TCP.clientWrite("win|" + Type);
        }
    }

    public static void AnotherLuo(Vector2 Pos)
    {
        Transform T = Instantiate(GameObject.Find("Main").transform.Find("Circle").transform,
                    GameObject.Find("Main").transform);
        if (1 - Type == 0)
            T.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        else
            T.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        int[] Pos2 = new int[2] { (int)Mathf.Round(Pos.x), (int)Mathf.Round(Pos.y) };
        T.transform.position = new Vector3(Pos2[0], Pos2[1], -0.1F);
        T.gameObject.SetActive(true);
        Board[Pos2[0] + 7][Pos2[1] + 7] = 1 - Type;
        CouldLuo = true;
    }

    public static void ReSet()
    {
        Transform P = GameObject.Find("Main").transform;
        for (int i = P.childCount - 1; i > 0; i--)
        {
            Destroy(P.GetChild(i).gameObject);
        }
        for (int i = 0; i < 15; i++)
        {
            Board[i] = new int[15];
            for (int j = 0; j < 15; j++)
                Board[i][j] = -1;
        }
    }

    public static bool Check(int[] P)
    {
        int[][] Dir = new int[4][] { new int[2] { 0, 1 }, new int[2] { 1, 0 },
            new int[2] { 1, 1 }, new int[2] { -1, 1 } };
        for (int i = 0; i < 4; i++)
        {
            int Num = 1;
            int[] P0 = new int[2] { P[0] + Dir[i][0], P[1] + Dir[i][1] };
            while (!(P0[0] < 0 || P0[0] > 14 || P0[1] < 0 || P0[1] > 14) && Board[P0[0]][P0[1]] == Type)
            {
                Num++;
                P0 = new int[2] { P0[0] + Dir[i][0], P0[1] + Dir[i][1] };
            }
            P0 = new int[2] { P[0] - Dir[i][0], P[1] - Dir[i][1] };
            while (!(P0[0] < 0 || P0[0] > 14 || P0[1] < 0 || P0[1] > 14) && Board[P0[0]][P0[1]] == Type)
            {
                Num++;
                P0 = new int[2] { P0[0] - Dir[i][0], P0[1] - Dir[i][1] };
            }
            if (Num >= 5)
                return true;
        }
        return false;
    }

}
