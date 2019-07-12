using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public UIController m_UiController;
    
    public CheckerBoardController m_CheckerBoard;

    /// <summary>
    /// The checkbox of the using of randomly initalized <see cref="positionCandidates"/>
    /// </summary>
    /// <remarks>
    /// This is only checked before the running of the algorithm.
    /// Randomly shuffle the candidate collumn list, so that we can get different results each time we press the "Start" button;
    /// It is also faster to find a solution by shuffleing, but the reason is unclear;
    /// </remarks>
    public Toggle m_UseRandomToggle;


    public int m_OriginalNQueens;

    public  NQueenSolver m_NQueensSolver;


    /// <summary>
    /// boolean value indicating the status of the application;
    /// true: solver is running. We cannot set the number of queens, button shows "stop"
    /// false: ideling, we can set the number of queens, or start computing
    /// </summary>
    private bool solverRunning;

    /// <summary>
    /// Number of queens
    /// </summary>
    private int nQueens;

    void Start()
    {
        nQueens = m_OriginalNQueens;

        SetNQueens(nQueens);
        solverRunning = false;
    }

    /// <summary>
    /// Update UI;
    /// Check the solver's state and manipulate UI accordingly;
    /// </summary>
    void Update()
    {

        // If the solver just stopped
        if (solverRunning && !m_NQueensSolver.IsRunning())
        {
            solverRunning = false;

            Vector2Int[] solverResult = m_NQueensSolver.FetchResult();

            if (solverResult != null)
            { 
                // if a solution is found:
                // 1. change UI
                // 2. show the solution on the checker board
                m_UiController.OnSolverFinish();

                m_CheckerBoard.SetQueens(solverResult);
            }
            else
            {
                // if calculation is cancelled:
                // only change UI
                m_UiController.OnSolverCancelled();
            }

        }

    }

    /// <summary>
    /// Set the number of queens; Will Call the Checkerboard to change size
    /// </summary>
    /// <remarks>
    /// Can only be called when solver is not running;
    /// </remarks>
    /// <param name="n_queens">Number of queens</param>
    public void SetNQueens(int n_queens)
    {
        if (solverRunning)
        {
            m_UiController.m_Subtitle.Print("Don't change Queen number while solver is running.");
            return;
        }
        nQueens = n_queens;
        m_CheckerBoard.SetNQueens(nQueens);
    }
    


    /// <summary>
    /// Start the solver thread;
    /// Set UI;
    /// </summary>
    /// <seealso cref="NQueenSolver.StartSolver(int)"/>
    public void StartSolver()
    {
        if(solverRunning)
            throw new System.Exception("Trying to run multiple solvers simutanuously");

        solverRunning = true;

        m_UiController.OnSolverStart(nQueens);

        m_NQueensSolver.StartSolver(nQueens, m_UseRandomToggle.isOn);
    }

    /// <summary>
    /// Send a cancelation signal to the solver
    /// </summary>
    public void CancelSolver()
    {
        m_NQueensSolver.Cancel();
    }


    /// <summary>
    /// Exit button callback
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }

    /// <summary>
    /// showing if the solver is running or not
    /// </summary>
    /// <returns></returns>
    public bool SolverIsRunning()
    {
        return solverRunning;
    }
}
