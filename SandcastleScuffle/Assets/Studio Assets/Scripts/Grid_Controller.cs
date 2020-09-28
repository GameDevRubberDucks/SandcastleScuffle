using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Grid_Row
{
    [SerializeField]
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
    public float m_gridSquareWorldSize;
    public int m_numRows;
    public int m_pathLength;
    public int m_castleCountPerSide;



    //--- Private Variables ---//
    private Crab_Manager m_crabManager;
    [HideInInspector][SerializeField] private Vector3 m_bottomLeftWorldPos;
    [HideInInspector][SerializeField] private Vector2 m_gridDimensionCount;
    [HideInInspector][SerializeField] private List<Grid_Row> m_rows;



    //--- Unity Methods ---//
    private void Start()
    {
        // Set up the mappings for the crabs
        m_crabManager = FindObjectOfType<Crab_Manager>();
        m_crabManager.SetUpCrabGrid(m_rows);
    }



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

        // Calculate the new full grid counts (ie: the number of discrete cells NOT in world space), including both the paths and the sandcastles
        m_gridDimensionCount.x = m_pathLength + (2.0f * m_castleCountPerSide);
        m_gridDimensionCount.y = m_numRows;

        // Calculate the new bottom left position in world space
        CalculateBottomLeftWorldPos();

        // Keep track of which grid coordinate we are on
        Vector2 nextGridCoord = new Vector2(0.0f, m_gridDimensionCount.y);

        // Generate all of the individual squares
        // Start from the top and work down so that the bottom left is 0,0
        for (int row = (int)m_gridDimensionCount.y - 1; row >= 0; row--)
        {
            // Move to the next row
            nextGridCoord = new Vector2(0.0f, (float)row);

            // Create a new row script and object
            var newRow = new Grid_Row();
            var newRowObj = new GameObject("Row " + row.ToString());
            newRowObj.transform.parent = m_squareParent;

            // Add the first set of sandcastles
            for (int castleNum = 0; castleNum < m_castleCountPerSide; castleNum++)
                newRow.AddSquare(SpawnSquare(m_castlePrefab, newRowObj.transform, ref nextGridCoord));

            // Add the paths in the middle
            for (int col = 0; col < m_pathLength; col++)
                newRow.AddSquare(SpawnSquare(m_pathPrefab, newRowObj.transform, ref nextGridCoord));

            // Add the last set of sandcastles
            for (int castleNum = 0; castleNum < m_castleCountPerSide; castleNum++)
                newRow.AddSquare(SpawnSquare(m_castlePrefab, newRowObj.transform, ref nextGridCoord));

            // Add the row to the grid
            m_rows.Add(newRow);
        }

        // Finally, reverse the rows within the array so they match the order in the scene (descending order)
        // This way, the coordinates of each tile can be used to directly access the arrays
        m_rows.Reverse();

        // Send the list of grid squares to the crab controller to manage
        Crab_Manager crabManager = FindObjectOfType<Crab_Manager>();
        crabManager.SetUpCrabGrid(m_rows);
    }



    //--- Getters ---//
    public Vector3 GetWorldPosFromCoord(Vector2 _gridCoord)
    {
        // Offset from the bottom left position
        Vector2 scaledGridPos = (_gridCoord * m_gridSquareWorldSize);
        return new Vector3(scaledGridPos.x, scaledGridPos.y, 0.0f) + m_bottomLeftWorldPos;
    }

    public Grid_Square GetGridSquare(Vector2 _gridCoord)
    {
        // Wrap the coordinate if it is out of bounds of the grid
        _gridCoord = WrapGridCoord(_gridCoord);

        // Convert the coordinate components to array indices
        int rowIdx = (int)_gridCoord.y;
        int colIdx = (int)_gridCoord.x;

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

        // Add the square's coordinate to its name
        string baseName = newSquareObj.name;
        string coordStr = "(" + _gridCoord.x.ToString("F0") + "," + _gridCoord.y.ToString("F0") + ") - ";
        newSquareObj.name = coordStr + baseName;

        // Grab the square script off the object and return it
        var squareComp = newSquareObj.GetComponent<Grid_Square>();

        // Set the square's coordinate
        squareComp.GridCoord = _gridCoord;

        // Move to the next grid position horizontally
        _gridCoord.x++;

        // Return the square 
        return squareComp;
    }

    private Vector2 WrapGridCoord(Vector2 _gridCoord)
    {
        // Wrap the x-component first
        if (_gridCoord.x < 0.0f)
            _gridCoord.x += m_gridDimensionCount.x;
        else if (_gridCoord.x >= m_gridDimensionCount.x)
            _gridCoord.x -= m_gridDimensionCount.x;

        // Wrap the y-component next
        if (_gridCoord.y < 0.0f)
            _gridCoord.y += m_gridDimensionCount.y;
        else if (_gridCoord.y >= m_gridDimensionCount.y)
            _gridCoord.y -= m_gridDimensionCount.y;

        // Return the wrapped value
        return _gridCoord;
    }

    private void CalculateBottomLeftWorldPos()
    {
        // We will start from the center and move away to reach the bottom left corner
        Vector3 centerPosition = this.transform.position;
        m_bottomLeftWorldPos = centerPosition;

        // Determine the horizontal amount required to center first
        if (m_gridDimensionCount.x % 2 == 0)
        {
            // If even number, the two central squares should be evenly spaced around the center
            int halfUnitCount = ((int)m_gridDimensionCount.x / 2) - 1;
            m_bottomLeftWorldPos.x -= ((halfUnitCount * m_gridSquareWorldSize) + (m_gridSquareWorldSize / 2.0f));
        }
        else
        {
            // If odd, the center square should be directly on top of the center
            int halfUnitCount = ((int)m_gridDimensionCount.x - 1) / 2;
            m_bottomLeftWorldPos.x -= (halfUnitCount * m_gridSquareWorldSize);
        }

        // Center vertically next
        if (m_gridDimensionCount.y % 2 == 0)
        {
            // If even number, the two central squares should be evenly spaced around the center
            int halfUnitCount = ((int)m_gridDimensionCount.y / 2) - 1;
            m_bottomLeftWorldPos.y -= ((halfUnitCount * m_gridSquareWorldSize) + (m_gridSquareWorldSize / 2.0f));
        }
        else
        {
            // If odd, the center square should be directly on top of the center
            int halfUnitCount = ((int)m_gridDimensionCount.y - 1) / 2;
            m_bottomLeftWorldPos.y -= (halfUnitCount * m_gridSquareWorldSize);
        }
    }
}
