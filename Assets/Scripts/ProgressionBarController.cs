using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionBarController : MonoBehaviour
{

    public GameObject m_BarPrefab;

    /// <summary>
    /// parameters to compute the number of bars to show, <see cref="GetMaxBarIndex"/>
    /// </summary>
    public int m_HideNBars;
    public int m_MinNBars;
    public int m_MaxnBars;

    private bool showProgressionBar = false;

    private int queenNumber = 0;

    /// <summary>
    /// number of bars to show, <see cref="GetMaxBarIndex"/>
    /// </summary>
    private int barNumber = 0;

    /// <summary>
    /// container of the values of progress bars;
    /// </summary>
    /// <seealso cref="SetProgress(int, float)"/>
    /// <seealso cref="GetProgress(int)"/>
    private float[] progresses= { };
    
    /// <summary>
    /// container of bar objects
    /// </summary>
    private GameObject[] bars;


    void Update()
    {
        // read values from this.progresses and set them to the bars
        if (showProgressionBar)
            for(int i = 0;i<= GetMaxBarIndex(); i++)
            {
                float progress = GetProgress(i);
                bars[i].GetComponent<Slider>().value = progress;
            }
    }

    /// <summary>
    /// toggle showing bars, as well as set the number of queens;
    /// Will create new slider objects
    /// </summary>
    /// <param name="queen_number">number of queens</param>
    public void Show(int queen_number)
    {
        showProgressionBar = true;
        queenNumber = queen_number;

        barNumber = GetMaxBarIndex()+1;

        // init progress container
        progresses = new float[barNumber];
        for (int i = 0; i < barNumber; i++)
            progresses[i] = 0f;

        // remove old sliders
        ClearBars();

        // create new sliders
        bars = new GameObject[barNumber];
        for (int i = 0; i < barNumber; i++)
        {
            bars[i] = Instantiate(m_BarPrefab);
            bars[i].transform.SetParent(transform, false);
            bars[i].transform.Translate(0f, -i * 20f, 0f);
            bars[i].GetComponentInChildren<Text>().text = "Queen " + i.ToString();

            UpdateProgress(i, 0f);
        }
    }

    /// <summary>
    /// thread safe method to store the progress of one bar/slider/queen to the container.
    /// </summary>
    /// <param name="row">index of the bar/slider, AKA the row of the queen</param>
    /// <param name="value">0~1 progress</param>
    public void SetProgress(int row, float value)
    {
        if(row>= barNumber)
            return;
        lock (progresses)
        {
            progresses[row] = value;
        }
    }

    /// <summary>
    /// thread safe method to read the progress of one bar/slider/queen from the container
    /// </summary>
    /// <param name="row">ndex of the bar/slider, AKA the row of the queen</param>
    /// <returns>0~1 progress value</returns>
    private float GetProgress(int row)
    {
        if (!showProgressionBar)
        {
            return 0f;
        }
        float progress;
        lock (progresses)
        {
            progress = progresses[row];
        }
        return progress;
    }

    /// <summary>
    /// Compute the max bar index based on the public variables and number of queens
    /// </summary>
    /// <returns></returns>
    public int GetMaxBarIndex()
    {
        int barNumber = queenNumber - m_HideNBars;
        barNumber = Mathf.Max(barNumber, m_MinNBars);

        barNumber = Mathf.Min(barNumber, m_MaxnBars);

        barNumber = Mathf.Min(barNumber, queenNumber);

        return barNumber -1;
    }

    /// <summary>
    /// update progress bar in UI
    /// </summary>
    /// <param name="queen_index">index of the bar/slider, row of the queen</param>
    /// <param name="prog">progress value</param>
    private void UpdateProgress(int queen_index, float prog)
    {
        if (!showProgressionBar)
            return;
        bars[queen_index].GetComponent<Slider>().value = prog;
    }

    /// <summary>
    /// Hide all progress bars;
    /// </summary>
    public void Hide()
    {
        showProgressionBar = false;
        ClearBars();
    }

    /// <summary>
    /// Remove current sliders
    /// </summary>
    private void ClearBars()
    {
        if (bars != null)
        {
            for (int i = 0; i < bars.Length; i++)
                Destroy(bars[i].gameObject);
            bars = null;
        }
    }
}
