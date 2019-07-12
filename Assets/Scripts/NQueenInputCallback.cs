using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NQueenInputCallback : MonoBehaviour
{
    public int m_MaxNQueens;
    public int m_MinNQueens;

    public GameManager m_GameManager;

    private InputField inputField;


    private void Start()
    {
        inputField = GetComponent<InputField>();
        inputField.text = m_GameManager.m_OriginalNQueens.ToString();
    }

    /// <summary>
    /// OnEdit callback of queen number input field;
    /// Will check and fix the number of queens, then invoke game manager
    /// </summary>
    public void OnEdit()
    {
        int nQueens;
        if (!int.TryParse(inputField.text, out nQueens))
        {
            Debug.LogError("Invalid Input Number of queens:" + inputField.text);
            return;
        }

        if (nQueens > m_MaxNQueens)
        {

            nQueens = m_MaxNQueens;
            inputField.text = nQueens.ToString();
        }

        if (nQueens < m_MinNQueens)
        {
            nQueens = m_MinNQueens;
            inputField.text = nQueens.ToString();
        }
        m_GameManager.SetNQueens(nQueens);
    }
}
