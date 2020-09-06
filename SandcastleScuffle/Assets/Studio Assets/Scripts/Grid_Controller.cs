using UnityEngine;
using System.Collections.Generic;

public class Grid_Row
{
    public List<Grid_Square> m_squares;

    public Grid_Row()
    {
        m_squares = new List<Grid_Square>();
    }

    public void AddSquare(Grid_Square _newSquare)
    {
        m_squares.Add(_newSquare);
    }
}

[ExecuteAlways]
public class Grid_Controller : MonoBehaviour
{
    //--- Public Variables ---//
    public Transform m_squareParent;
    public GameObject m_pathPrefab;
    public GameObject m_castlePrefab;
    public Vector2 m_gridPathCounts;
    public float m_gridSquareSize;



    //--- Private Variables ---//
    private Vector2 m_gridDimensions;
    private List<Grid_Row> m_rows;



    //--- Methods ---//
    public void ClearGrid()
    {
        // Destroy all of the existing grid cells
        while (m_squareParent.childCount > 0)
            DestroyImmediate(m_squareParent.GetChild(0).gameObject);

        // Reset the list of rows
        m_rows = new List<Grid_Row>();
    }

    public void GenerateGrid()
    {
        // Clear the existing grid
        ClearGrid();

        // Calculate the new full grid size, including both the paths and the sandcastles
        m_gridDimensions.x = m_gridPathCounts.x + 2;
        m_gridDimensions.y = m_gridPathCounts.y;

        // Keep track of which grid coordinate we are on
        // The grid starts at 0,0 in the top left
        Vector2 nextGridCoord = Vector2.zero;

        // Generate all of the individual squares
        for (int row = 0; row < m_gridPathCounts.y; row++)
        {
            // Create a new row script and object
            var newRow = new Grid_Row();
            var newRowObj = new GameObject("Row " + row.ToString());
            newRowObj.transform.parent = m_squareParent;

            // Add the first sandcastle 
            newRow.AddSquare(SpawnSquare(m_castlePrefab, newRowObj.transform, ref nextGridCoord));

            // Add the paths in the middle
            for (int col = 0; col < m_gridPathCounts.x; col++)
                newRow.AddSquare(SpawnSquare(m_pathPrefab, newRowObj.transform, ref nextGridCoord));

            // Add the last sandcastle
            newRow.AddSquare(SpawnSquare(m_castlePrefab, newRowObj.transform, ref nextGridCoord));

            // Add the row to the grid
            m_rows.Add(newRow);

            // Move down to the next row
            nextGridCoord.x = 0;
            nextGridCoord.y++;
        }
    }



    //--- Getters ---//
    public Vector3 GetWorldPosFromCoord(Vector2 _gridCoord)
    {
        // Convert a grid coord (from top left) to an actual Unity position
        // Have to reverse the y-coordinate so it isn't negative but still goes from top to bottom
        Vector3 topLeftPos = this.transform.position;
        Vector2 scaledGridPos = (_gridCoord * m_gridSquareSize);
        scaledGridPos.y *= -1.0f;
        return new Vector3(scaledGridPos.x, scaledGridPos.y, 0.0f) + topLeftPos;
    }

    public Grid_Square GetGridSquare(Vector2 _gridCoord)
    {
        // Calculate the indices for the lists
        // Use modulo to allow for wrapping and prevent going out of bounds
        int rowIdx = (int)_gridCoord.y % (int)m_gridPathCounts.y;
        int colIdx = (int)_gridCoord.x % (int)m_gridPathCounts.x;

        // Return the grid square at the given index
        return m_rows[rowIdx].m_squares[colIdx];
    }

    public Grid_Square GetRandCastleSquare(bool _leftSide)
    {
        // Randomly select a row
        int rowIdx = Random.Range(0, m_rows.Count);
        var row = m_rows[rowIdx];

        // Return the left or right sandcastle
        // The left sandcastle is always the first square and the right is always the last
        return (_leftSide) ? row.m_squares[0] : row.m_squares[row.m_squares.Count - 1];
    }



    //--- Utility Methods ---//
    private Grid_Square SpawnSquare(GameObject _squarePrefab, Transform _rowParent, ref Vector2 _gridCoord)
    {
        // Calculate the spawn location in Unity coordinates
        Vector3 spawnLoc = GetWorldPosFromCoord(_gridCoord);

        // Instantiate the prefab but keep a reference to it
        var newSquareObj = Instantiate(_squarePrefab, spawnLoc, Quaternion.identity, _rowParent);

        // Grab the square script off the object and return it
        var squareComp = newSquareObj.GetComponent<Grid_Square>();

        // Set the square's coordinate
        squareComp.GridCoord = _gridCoord;

        // Move to the next grid position horizontally
        _gridCoord.x++;

        // Return the square 
        return squareComp;
    }
}
