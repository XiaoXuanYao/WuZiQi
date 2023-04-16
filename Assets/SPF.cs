using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SPF : MonoBehaviour
{

    [Tooltip("unit: millisecond")]
    public static float SPFTime = 30;
    public float SPFTimeShow = 30;

    DateTime LastUpdate = DateTime.Now;
    List<float> HistorySPFTime = new();
    void Update()
    {
        TimeSpan DTime = DateTime.Now - LastUpdate;
        LastUpdate = DateTime.Now;
        if (HistorySPFTime.Count >= 60)
        {
            float All = 0;
            for(int i = 0; i < HistorySPFTime.Count; i += 2)
            {
                All += HistorySPFTime[i];
            }
            SPFTime = SPFTimeShow = (int)(All / HistorySPFTime.Count * 2 * 100) / 100F;
            if (SPFTime > 100)
            {
                SPFTime = SPFTimeShow = 100;
            }
            HistorySPFTime.RemoveAt(0);
        }
        HistorySPFTime.Add((float)DTime.TotalMilliseconds);
    }

}
