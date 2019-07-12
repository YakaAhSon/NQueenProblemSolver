using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;


public class NQueenSolver : MonoBehaviour
{
    /// <summary>
    /// Need a reference to UI controller to set the progress bar
    /// </summary>
    public UIController m_UiController;

    private int queenNumber = 0;

    private Thread solverThread;

    /// <summary>
    /// A flag of cancelling the current task; This is set by the main thread and read by the solver thread.
    /// </summary>
    /// <remarks>
    /// Since it's a boolean value, and only written by main thread, so it is safe.
    /// It is checked before each step of the solver.
    /// </remarks>
    /// <seealso cref="Cancel"/>
    /// <seealso cref="SearchQueenPosition(int)"/>
    private bool cancelationFlag;

    /// <summary>
    /// A boolean value indicating that calculation is done, and there is a solution waiting to be fetched, stored in <see cref="queenPositions"/> 
    /// </summary>
    /// <seealso cref="PushResult"/>
    /// <seealso cref="FetchResult"/>
    private bool resultWaintingToBeFetched = false;





    /// <summary>
    /// Temporarilly used when the solver is running;
    /// Container of positions of the queens. When <see cref="solution"/> is true, this contains a valis solution
    /// </summary>
    /// <seealso cref="SearchQueenPosition(int)"/>
    /// <seealso cref="FetchResult"/>
    private int[] queenPositions= { };

    /// <summary>
    /// Temporarilly used when the solver is running;
    /// container of possible remaining candidates of collumns.
    /// </summary>
    /// <remarks>
    /// This container is initialized before solver starts, either randomly or in order, and maintained during the process of solution searching;
    /// All possible candidates are stord in the first (queenNumber-row) values, so traversing all collumns is avoided;
    /// </remarks>
    /// <seealso cref="SearchQueenPosition(int)"/>
    private int[] positionCandidates;

    /// <summary>
    /// Temporarilly used when the solver is running;
    /// the table to record the use of main diagnals. 
    /// </summary>
    /// <remarks>
    /// Use this table to check validity on the main diagnal in O(1) time
    /// </remarks>
    /// <seealso cref="SearchQueenPosition(int)"/>
    private bool[] usedMainDiagnal;

    /// <summary>
    /// Temporarilly used when the solver is running;
    /// the table to record the use of paradiagnals. 
    /// </summary>
    /// <remarks>
    /// Use this table to check validity on the para diagnal in O(1) time
    /// </remarks>
    /// <seealso cref="SearchQueenPosition(int)"/>
    private bool[] usedParaDiagnal;

    /// <summary>
    /// Start a solver thread to find one solution of queen_number queen problem
    /// </summary>
    /// <param name="queen_number">number of queens</param>
    public void StartSolver(int queen_number, bool randomStart)
    {
        if (IsRunning())
            throw new System.Exception("Trying to run multiple solvers simutanuously");

        resultWaintingToBeFetched = false;

        queenNumber = queen_number;
        
        cancelationFlag = false;


        // initialize solver's variables
        // initialize queen positions array
        queenPositions = new int[queenNumber];

        // initialize candidate positions array
        positionCandidates = new int[queenNumber];
        for (int i = 0; i < queenNumber; i++)
            positionCandidates[i] = i;

        // shuffle candidate positions on request
        if(randomStart) for (int i = queenNumber - 1; i >= 0; i--)
        {
            int idx = Random.Range(0, i + 1);
            int temp = positionCandidates[i];
            positionCandidates[i] = positionCandidates[idx];
            positionCandidates[idx] = temp;
        }

        // initialize diagnal used tables
        usedMainDiagnal = new bool[queenNumber * 2 - 1];
        usedParaDiagnal = new bool[queenNumber * 2 - 1];
        for (int i = 0; i < queenNumber * 2 - 1; i++)
        {
            usedMainDiagnal[i] = false;
            usedParaDiagnal[i] = false;
        }

        solverThread = new Thread(SolverMain);
        solverThread.Start();

    }

    /// <summary>
    /// Auxiliary function to compute the main diagnal of a square
    /// </summary>
    /// <param name="row">row</param>
    /// <param name="collumn">collumn</param>
    /// <returns>index of the square's main diagnal</returns>
    private int GetMainDiagnal(int row, int collumn)
    {
        return row - collumn + queenNumber - 1;
    }


    /// <summary>
    /// Auxiliary function to compute the para diagnal of a square
    /// </summary>
    /// <param name="row">row</param>
    /// <param name="collumn">collumn</param>
    /// <returns>index of the square's para diagnal</returns>
    private int GetParaDiagnal(int row, int collumn)
    {
        return row + collumn;
    }
    
    /// <summary>
    /// Recursively search a solution; Return's either when a solution is found or when <see cref="cancelationFlag"/> is set.
    /// Will set progression bar after every step;
    /// This algorithm searches for valid positions of queens row by row, so the queens will not be in the same rows;
    /// In each row, since a list of valid candidate collumns are maintained, so the queens will not be in the same collumns;
    /// so we only have to check the validity of diagnals;
    /// By using 2 tables, validity of diagnals can be checked in O(1) time;
    /// If the diagnals are also valid, we will continue seaching for next row;
    /// Stop when all rows have valid collumns, then push the solution;
    /// Also stop when a cancelation flag is set from main thread; In this the solution will not be pushed;
    /// </summary>
    /// <param name="row">current row to search a position</param>
    /// <returns>true if a solution is found, false when cancelled</returns>
    bool SearchQueenPosition(int row)
    {
        if (row >= queenNumber)
            return true;

        // Since all valid collumn candidates are maintaind as the first (queenNumber - row) values,
        // Only the first several values need to be traversed
        for(int i = 0; i < queenNumber - row; i++)
        {
            m_UiController.m_ProgressionBar.SetProgress(row, ((float)i+1) / (float)(queenNumber - row));

            // find a candidate collumn
            int collumn = positionCandidates[i];
            
            // check validation of diagnals
            int mainDiagnal = GetMainDiagnal(row, collumn);
            if (usedMainDiagnal[mainDiagnal])
                continue;
            int paraDiagnal = GetParaDiagnal(row, collumn);
            if (usedParaDiagnal[paraDiagnal])
                continue;
        
            // claim diagnals
            usedMainDiagnal[mainDiagnal] = true;
            usedParaDiagnal[paraDiagnal] = true;

            // save result
            queenPositions[row] = collumn;

            // maintain positionCandidate, so that all valid collumns should be prefix of the list
            positionCandidates[i] = positionCandidates[queenNumber - row - 1];

            // recursively find solution for next row
            if (SearchQueenPosition(row + 1))
                return true;
            
            // check cancellation flag right after recursion, so that the thread can responde to a cancelation order quickly
            if (cancelationFlag)
                return false;

            // resume position Candidates
            positionCandidates[i] = collumn;

            // release diagnals
            usedMainDiagnal[mainDiagnal] = false;
            usedParaDiagnal[paraDiagnal] = false;
        }
        return false;
    }


    /// <summary>
    /// The entry of the solver thread;
    /// If <see cref="SearchQueenPosition(int)"/> resurns true, then a solution is found so we need to push the solution;
    /// Otherwise, the solver is stop due a cancellation, no solution is found so no need to push result;
    /// </summary>
    void SolverMain()
    {
        if (SearchQueenPosition(0))
            PushResult();
    }


    /// <summary>
    /// Cancle the solver thread
    /// </summary>
    /// <remarks>
    /// Only call this from main thread.
    /// </remarks>
    public void Cancel()
    {
        cancelationFlag = true;
    }

    /// <summary>
    /// Threadsafe method to set the result ready for main threaf to fetch
    /// </summary>
    private void PushResult()
    {
        lock (queenPositions)
        {
            resultWaintingToBeFetched = true;
        }
    }

    /// <summary>
    /// Threadsafe method to fetch the solution
    /// </summary>
    /// <returns>return the solution if a solution is found; return null if the solver is cancelled.</returns>
    public Vector2Int[] FetchResult()
    {
        Vector2Int[] result = null; 
        lock (queenPositions)
        {
            if (resultWaintingToBeFetched)
            {
                result = new Vector2Int[queenPositions.Length];
                for(int i = 0; i < queenPositions.Length; i++)
                {
                    result[i].x = i;
                    result[i].y = queenPositions[i];
                }
                resultWaintingToBeFetched = false;
            }
        }
        return result;
    }

    /// <summary>
    /// Boolean function indicating whether the solver thread is running or not
    /// </summary>
    public bool IsRunning()
    {
        if (solverThread == null)
        {
            return false;
        }
        return solverThread.IsAlive;
    }
}
