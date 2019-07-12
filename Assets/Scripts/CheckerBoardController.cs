using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerBoardController : MonoBehaviour
{
    public GameObject m_CheckerBoardWhiteSquarePrefab;
    public GameObject m_CheckerBoardBlackSquarePrefab;

    public Material m_HightlightMaterial;

    public GameObject m_QueenPrefab;

    private GameObject[,] checkerBoardSquares= { };

    private QueenController[] queens;

    /// <summary>
    /// temporary container of the highlighted squares
    /// </summary>
    /// <remarks>
    /// Use this tempory storage to avoid traversing all squares every frame
    /// </remarks>
    /// <seealso cref="ClearHighLightSquares"/>
    /// <seealso cref="HightlightSquare(int, int)"/>
    private List<System.Tuple<GameObject, Material>> highlightSquares = new List<System.Tuple<GameObject, Material>>();

    private int queenNumber;

    void Update()
    {
        // Reset the material of all squares
        ClearHighLightSquares();

        // highlight the squares which can be attacked by the pointed queen
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.tag == "queen")
            {
                Vector2Int queenPos = hit.transform.GetComponent<QueenController>().MyPosition;
                Vector2Int[] directions ={
                        new Vector2Int(-1,-1),
                        new Vector2Int(-1,0),
                        new Vector2Int(-1,1),
                        new Vector2Int(0,1),
                        new Vector2Int(0,-1),
                        new Vector2Int(1,-1),
                        new Vector2Int(1,0),
                        new Vector2Int(1,1)
                    };

                foreach (Vector2Int direction in directions)
                {
                    Vector2Int squarePos = queenPos + direction;
                    while (squarePos.x >= 0 && squarePos.y >= 0 && squarePos.x < queenNumber && squarePos.y < queenNumber)
                    {
                        HightlightSquare(squarePos.x, squarePos.y);
                        squarePos += direction;
                    }

                }

            }
        }
    }
    
    /// <summary>
    /// Set the number of queens;
    /// Will also resize the checker board to fit the number of queens.
    /// </summary>
    /// <param name="queen_number">number of queens</param>
    public void SetNQueens(int queen_number)
    {
        queenNumber = queen_number;

        // clean old squares
        if (checkerBoardSquares != null)
            for (int row = 0; row < checkerBoardSquares.GetLength(0); row++)
                for (int collumn = 0; collumn < checkerBoardSquares.GetLength(1); collumn++)
                    if (checkerBoardSquares[row, collumn] != null)
                        Destroy(checkerBoardSquares[row, collumn].gameObject);

        // create new squares
        checkerBoardSquares = new GameObject[queenNumber, queenNumber];

        for (int row = 0; row < queenNumber; row++)
            for (int collumn = 0; collumn < queenNumber; collumn++)
            {
                GameObject newSquare;
                if ((row + collumn) % 2 == 0)
                    newSquare = Instantiate(m_CheckerBoardWhiteSquarePrefab);
                else
                    newSquare = Instantiate(m_CheckerBoardBlackSquarePrefab);

                newSquare.transform.Translate(collumn, 0f, queenNumber - row - 1);
                newSquare.transform.SetParent(transform, false);

                checkerBoardSquares[row, collumn] = newSquare;
            }

        // scale the new checker board to fit the camera
        float scale = 8f / queenNumber * 1.4f;
        transform.localScale = new Vector3(scale, scale, scale);

        // set default positions of queens
        SetQueens();
    }

    /// <summary>
    /// Set the position of queens
    /// </summary>
    /// <param name="queen_positions">position of queens, queen_position.Length has to be identical to the actual number of queens.</param>
    public void SetQueens(Vector2Int[] queen_positions)
    {
        if (queen_positions.Length != queenNumber)
            throw new System.Exception("Invalid number of queens to render.");
        
        // clear old queens
        if (queens != null)
            for (int i = 0; i < queens.Length; i++)
                if (queens[i] != null)
                    Destroy(queens[i].gameObject);

        // create new queens
        queens = new QueenController[queenNumber];

        for(int i = 0; i < queenNumber; i++)
        {
            GameObject newQueen = Instantiate(m_QueenPrefab);

            newQueen.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            newQueen.transform.SetParent(checkerBoardSquares[queen_positions[i].x, queen_positions[i].y].transform, false);
            
            queens[i] = newQueen.GetComponent<QueenController>();
            queens[i].SetMyPosition(queen_positions[i].x, queen_positions[i].y);
        }
    }

    /// <summary>
    /// Set all queens to be at the first row
    /// </summary>
    /// <seealso cref="SetQueens(Vector2Int[])"/>
    public void SetQueens()
    {
        Vector2Int[] queen_positions = new Vector2Int[queenNumber];
        for(int i = 0; i < queenNumber; i++)
        {
            queen_positions[i] = new Vector2Int(0, i);
        }
        SetQueens(queen_positions);
    }

    /// <summary>
    /// Set all squares to their original colors;
    /// </summary>
    /// <seealso cref="HightlightSquare(int, int)"/>
    private void ClearHighLightSquares()
    {
        if (highlightSquares == null)
            return;
        
        foreach(System.Tuple<GameObject, Material> i in highlightSquares)
        {
            i.Item1.GetComponent<Renderer>().material = i.Item2;
        }

        highlightSquares.Clear();
    }

    /// <summary>
    /// Set the square's material to be hightlight
    /// </summary>
    /// <param name="row">row of the square to be set</param>
    /// <param name="collumn">collumn of the square to be set</param>
    /// <seealso cref="ClearHighLightSquares"/>
    private void HightlightSquare(int row, int collumn)
    {
        GameObject square = checkerBoardSquares[row, collumn];
        highlightSquares.Add(new System.Tuple<GameObject, Material>(square, square.GetComponent<Renderer>().material));
        square.GetComponent<Renderer>().material = m_HightlightMaterial;
    }
}
