using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class UIController : MonoBehaviour
{
    // Simple UI objects, which are controlled directly
    public InputField m_NQueensInput;
    public Button m_StartStopButton;
    public Text m_UsedTimeText;
    public Text m_InfoText;

    // Complecated UI objects, which are controlled by a controller
    public ProgressionBarController m_ProgressionBar;
    public SubtiitleController m_Subtitle;

    private bool solverRunning = false;

    /// <summary>
    /// A stopwatch timer to show how much time has been used by the solver thread
    /// </summary>
    private Stopwatch timer = new Stopwatch();

    // Update is called once per frame
    // update timer when solver is running
    void Update()
    {
        if (solverRunning)
        {
            float seconds = timer.ElapsedMilliseconds / 1000f;
            m_UsedTimeText.text = "Used Time: " + seconds.ToString() + "s";
            m_InfoText.text = "Solver is running.";
        }
    }

    /// <summary>
    /// Tell UI objects that the solver start running;
    /// Disable queen number input text;
    /// Set "stop" button;
    /// Show progress bar
    /// </summary>
    /// <seealso cref="GameManager.StartSolver"/>
    public void OnSolverStart(int n_queens)
    {
        solverRunning = true;
        timer.Restart();

        m_NQueensInput.readOnly = true;
        m_StartStopButton.GetComponentInChildren<Text>().text = "Stop";

        m_InfoText.text = "Solver is running.";
        m_ProgressionBar.Show(n_queens);
    }


    /// <summary>
    /// Tell UI objects that the solver stops running
    /// </summary>
    private void OnSolverStop()
    {
        solverRunning = false;
        timer.Stop();
        m_NQueensInput.readOnly = false;
        m_StartStopButton.GetComponentInChildren<Text>().text = "Start";
    }

    /// <summary>
    /// Tell UI objects that the solver is cancelled
    /// </summary>
    public void OnSolverCancelled()
    {
        OnSolverStop();
        m_InfoText.text = "Solver is cancelled.";
    }

    /// <summary>
    /// Tell UI objects that the solver is finished
    /// </summary>
    public void OnSolverFinish()
    {
        OnSolverStop();
        m_InfoText.text = "Solution is found.";
    }

}
