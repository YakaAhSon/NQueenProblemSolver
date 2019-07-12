using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartStopButtonCallback : MonoBehaviour
{

    public GameManager m_GameManager;

    /// <summary>
    /// Check the state of the solver, then inform game manager to start or stop the solver
    /// </summary>
    public void OnStartStopButtonClick()
    {
        if (m_GameManager.SolverIsRunning())
        {
            m_GameManager.CancelSolver();
        }
        else
        {
            m_GameManager.StartSolver();
        }
    }
}
