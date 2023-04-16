using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public float StartOpacity0 = 0; //单位 1
    public float EndOpacity0 = 1;   //单位 1
    public float FadeTime0 = 1;     //单位 秒
    public float[] Message = new float[] { 0, 0, 0 };  //对应 { StartOpacity , EndOpacity , FadeTime }
    public float Opacity;

    float OnceFadeColor = 0;
    bool is3DObject0 = false;

    public static void NewFade(string GameObjectFile, float StartOpacity, float EndOpacity, float FadeTime, bool is3DObject = false, bool FadeChilds = true)
    {
        Transform Obj = GameObject.Find(GameObjectFile.Split('/')[0]).transform;
        for (int i = 1; i < GameObjectFile.Split('/').Length; i++)
        {
            Obj = Obj.Find(GameObjectFile.Split('/')[i]);
        }
        if (Obj.GetComponent<Fade>())
        {
            Destroy(Obj.GetComponent<Fade>());
        }
        Obj.gameObject.SetActive(true);
        Fade AddFade = Obj.gameObject.AddComponent<Fade>();
        AddFade.Opacity = StartOpacity;
        AddFade.Message = new float[] { StartOpacity, EndOpacity, FadeTime };
        AddFade.is3DObject0 = is3DObject;
        AddFade.FinishPrepare();

        if (is3DObject && FadeChilds)
        {
            List<string> ObjectList = GetAllChild(StringToTransform(GameObjectFile));
            foreach (string Object in ObjectList)
            {
                Fade.NewFade(Object, StartOpacity, EndOpacity, FadeTime, true);
            }
        }
    }
    public static void NewFade(Transform GameObject, float StartOpacity, float EndOpacity, float FadeTime, bool is3DObject = false, bool FadeChilds = true)
    {
        GameObject.gameObject.SetActive(true);
        Fade AddFade = GameObject.gameObject.AddComponent<Fade>();
        AddFade.Opacity = StartOpacity;
        AddFade.Message = new float[] { StartOpacity, EndOpacity, FadeTime };
        AddFade.is3DObject0 = is3DObject;
        AddFade.FinishPrepare();

        if (is3DObject && FadeChilds)
        {
            List<string> ObjectList = GetAllChild(GameObject);
            foreach (string Object in ObjectList)
            {
                Fade.NewFade(Object, StartOpacity, EndOpacity, FadeTime, true);
            }
        }
    }

    public static List<string> GetAllChild(Transform Parent)
    {
        List<string> ChildString = new();
        for (int i = 0; i < Parent.childCount; i++)
        {
            ChildString.Add(Fade.TransformToString(Parent.GetChild(i)));
            if (Parent.GetChild(i).childCount > 0)
            {
                List<string> ChildInChildString = GetAllChild(Parent.GetChild(i));
                foreach (string Str in ChildInChildString)
                {
                    ChildString.Add(Str);
                }
            }
        }
        return ChildString;
    }

    public static List<GameObject> GetAllChildGameObject(Transform Parent)
    {
        List<GameObject> ChildString = new();
        for (int i = 0; i < Parent.childCount; i++)
        {
            ChildString.Add(Parent.GetChild(i).gameObject);
            if (Parent.GetChild(i).childCount > 0)
            {
                List<GameObject> ChildInChildString = GetAllChildGameObject(Parent.GetChild(i));
                foreach (GameObject Obj in ChildInChildString)
                {
                    ChildString.Add(Obj);
                }
            }
        }
        return ChildString;
    }

    public void FinishPrepare()
    {
        if (Message != new float[] { 0, 0, 0 })
        {
            StartOpacity0 = Message[0];
            EndOpacity0 = Message[1];
            FadeTime0 = Message[2];
        }
        if (StartOpacity0 == 0)
        {
            this.gameObject.SetActive(true);
        }
        if (!this.GetComponent<CanvasGroup>() && !this.GetComponent<SpriteRenderer>() && is3DObject0 == false)
        {
            this.gameObject.AddComponent<CanvasGroup>();
            this.GetComponent<CanvasGroup>().alpha = StartOpacity0;
        }

        if (this.GetComponent<CanvasGroup>())
        {
            this.GetComponent<CanvasGroup>().alpha = Opacity;
        }
        else if (this.GetComponent<SpriteRenderer>())
        {
            this.GetComponent<SpriteRenderer>().color = new Color(this.GetComponent<SpriteRenderer>().color.r,
                this.GetComponent<SpriteRenderer>().color.g, this.GetComponent<SpriteRenderer>().color.b, Opacity);
        }
        else if (is3DObject0 && this.GetComponent<Renderer>())
        {
            this.GetComponent<Renderer>().material.color = new Color(this.GetComponent<Renderer>().material.color.r,
                this.GetComponent<Renderer>().material.color.g, this.GetComponent<Renderer>().material.color.b, Opacity);
        }
        else
        {
            Destroy(this);
        }
        float FadeOpacity = EndOpacity0 - StartOpacity0;
        OnceFadeColor = FadeOpacity / FadeTime0;
        ChangeOpacity();
    }

    void Update()
    {
        if (OnceFadeColor != 0)
        {
            Opacity += OnceFadeColor * SPF.SPFTime / 1000;
            ChangeOpacity();
            if ((Opacity >= EndOpacity0 && StartOpacity0 < EndOpacity0) || (Opacity <= EndOpacity0 && StartOpacity0 > EndOpacity0))
            {
                Opacity = EndOpacity0;
                ChangeOpacity();
                Fade[] MyComponent = this.GetComponents<Fade>();
                if (MyComponent.Length > 1)
                {
                    MyComponent[1].FinishPrepare();
                }
                if (EndOpacity0 == 0)
                {
                    this.gameObject.SetActive(false);
                }
                Destroy(MyComponent[0]);
            }
        }
    }
    void ChangeOpacity()
    {
        if (this.GetComponent<CanvasGroup>())
        {
            this.GetComponent<CanvasGroup>().alpha = Opacity;
        }
        else if (this.GetComponent<SpriteRenderer>())
        {
            this.GetComponent<SpriteRenderer>().color = new Color(this.GetComponent<SpriteRenderer>().color.r
                , this.GetComponent<SpriteRenderer>().color.g, this.GetComponent<SpriteRenderer>().color.b, Opacity);
        }
        else if (is3DObject0 && this.GetComponent<Renderer>())
        {
            Material material = this.GetComponent<Renderer>().material;
            material.color = new Color(material.color.r, material.color.g, material.color.b, Opacity);
            this.GetComponent<Renderer>().material = material;
        }
    }
    public static string TransformToString(Transform Obj)
    {
        string Mes = Obj.name;
        Obj = Obj.parent;
        while (Obj)
        {
            Mes = Obj.name + "/" + Mes;
            if (Obj)
            {
                Obj = Obj.parent;
            }
            else
            {
                break;
            }
        }
        return Mes;
    }
    public static Transform StringToTransform(string ObjFileName)
    {
        Transform Obj = GameObject.Find(ObjFileName.Split('/')[0]).transform;
        for (int i = 1; i < ObjFileName.Split('/').Length; i++)
        {
            Obj = Obj.Find(ObjFileName.Split('/')[i]);
        }
        return Obj;
    }
}