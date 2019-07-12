using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class SubtiitleController : MonoBehaviour
{
    public int LifeTimeMilliSeconds;
    private Stopwatch timer = new Stopwatch();

    // Update is called once per frame
    void Update()
    {
        if (timer.IsRunning)
            if (timer.ElapsedMilliseconds >= LifeTimeMilliSeconds)
            {
                GetComponent<Text>().text = "";
                timer.Stop();
            }
    }

    /// <summary>
    /// Print subtitle to bottom;
    /// </summary>
    /// <param name="info"></param>
    public void Print(string info)
    {
        GetComponent<Text>().text = info;
        timer.Restart();
    }
}
