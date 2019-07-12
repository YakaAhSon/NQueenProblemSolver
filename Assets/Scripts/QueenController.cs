using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenController : MonoBehaviour
{
    private Vector2Int myPosition;
    
    /// <summary>
    /// delete the gameobject 
    /// </summary>
    public void Remove()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Set/Get the row and collumn position of the queen
    /// </summary>
    /// <remarks>
    /// This is only used as storage the position of the queen, but irrelevent with how it is rendered, <see cref="CheckerBoardController.SetQueens"/>
    /// </remarks>
    public Vector2Int MyPosition
    {
        get
        {
            if (myPosition == null)
            {
                throw new Exception("Queen's position is null");
            }
            else
            {
                return myPosition;
            }
        }
        set
        {
            myPosition = value;
        }
    }

    public void SetMyPosition(int row, int collumn)
    {
        myPosition = new Vector2Int(row, collumn);
    }

    
}
