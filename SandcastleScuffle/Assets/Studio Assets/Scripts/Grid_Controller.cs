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
    public GameObject m_basePrefab;
    public float m_gridSquareWorldSize;
    public int m_numRows;
    public int m_pathLength;
    public int m_castleCountPerSide;



    //--- Private Variables ---//
    private Crab_Manager m_crabManager;
    [HideInInspector][SerializeField] private Vector3 m_bottomLeftWorldPos;
    [HideInInspector][SerializeField] private Vector2 m_gridDimensionCount;
    [HideInInspector][SerializeField] private List<Grid_Row> m_rows;
    [HideInInspector][SerializeField] private List<Grid_Square> m_leftCastles;
    [HideInInspector][SerializeField] private List<Grid_Square> m_rightCastles;



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

        // Reset the list of rows and squares
        m_rows = new List<Grid_Row>();
        m_leftCastles = new List<Grid_Square>();
        m_rightCastles = new List<Grid_Square>();
    }

    public void GenerateGrid()
    {
        // Clear the existing grid
        ClearGrid();

        // Calculate the new full grid counts (ie: the number of discrete cells NOT in world space), including the paths, the sandcastles, AND the bases
        m_gridDimensionCount.x = m_pathLength + (2.0f * m_castleCountPerSide) + (2.0f);
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

            // Add the first base
            var leftBase = SpawnSquare(m_basePrefab, newRowObj.transform, Crab_Team.Left_Team, ref nextGridCoord);
            newRow.AddSquare(leftBase);

            // Add the first set of sandcastles
            for (int castleNum = 0; castleNum < m_castleCountPerSide; castleNum++)
            {
                var castleSquare = SpawnSquare(m_castlePrefab, newRowObj.transform, Crab_Team.Left_Team, ref nextGridCoord);
                newRow.AddSquare(castleSquare);
                m_leftCastles.Add(castleSquare);
            }

            // Add the paths in the middle
            for (int col = 0; col < m_pathLength; col++)
                newRow.AddSquare(SpawnSquare(m_pathPrefab, newRowObj.transform, Crab_Team.Neutral, ref nextGridCoord));

            // Add the last set of sandcastles
            for (int castleNum = 0; castleNum < m_castleCountPerSide; castleNum++)
            {
                var castleSquare = SpawnSquare(m_castlePrefab, newRowObj.transform, Crab_Team.Right_Team, ref nextGridCoord);
                newRow.AddSquare(castleSquare);
                m_rightCastles.Add(castleSquare);
            }

            // Add the last base
            var rightBase = SpawnSquare(m_basePrefab, newRowObj.transform, Crab_Team.Right_Team, ref nextGridCoord);
            newRow.AddSquare(rightBase);

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
        // Determine which castle list to grab from
        var castleList = (_leftSide) ? m_leftCastles : m_rightCastles;

        // Create a list of the square that are free of crabs
        List<Grid_Square> freeCastles = new List<Grid_Square>();
        foreach (var castle in castleList)
        {
            if (m_crabManager.GetCrabAtSquare(castle) == null)
                freeCastles.Add(castle);
        }

        // If there are no free castles, return empty
        if (freeCastles.Count == 0)
            return null;

        // Randomly select one of the free castles and return it
        int castleIdx = Random.Range(0, freeCastles.Count);
        return freeCastles[castleIdx];
    }



    //--- Utility Methods ---//
    private Grid_Square SpawnSquare(GameObject _squarePrefab, Transform _rowParent, Crab_Team _team, ref Vector2 _gridCoord)
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

        // Set the square's team (neutral for path squares, left or right for castle and base squares)
        squareComp.Team = _team;

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
