using UnityEngine;
using System.Collections.Generic;

public class Crab_Manager : MonoBehaviour
{
    //--- Public Variables ---//
    public GameObject m_crabPrefab;
    public Grid_Controller m_grid;
    public Transform m_crabParent;



    //--- Private Variables ---//
    private List<Crab_Controller> m_leftTeamCrabs;
    private List<Crab_Controller> m_rightTeamCrabs;
    private Dictionary<Grid_Square, Crab_Controller> m_crabMappings;



    //--- Unity Methods ---//
    private void Awake()
    {
        // Init the private variables
        m_leftTeamCrabs = new List<Crab_Controller>();
        m_rightTeamCrabs = new List<Crab_Controller>();
    }

    private void Update()
    {
        // TEMP: Debug controls to spawn and move the crabs around
        if (Input.GetKeyDown(KeyCode.LeftBracket))
            SpawnNewCrab(Crab_Team.Left_Team);
        else if (Input.GetKeyDown(KeyCode.RightBracket))
            SpawnNewCrab(Crab_Team.Right_Team);

        if (Input.GetKeyDown(KeyCode.Semicolon))
            MoveCrabs(Crab_Team.Left_Team);
        else if (Input.GetKeyDown(KeyCode.Quote))
            MoveCrabs(Crab_Team.Right_Team);
    }



    //--- Methods ---//
    public Crab_Controller SpawnNewCrab(Crab_Team _crabTeam)
    {
        // Determine the type for the new crab
        int crabTypeIndex = Random.Range(0, (int)Crab_Type.Num_Types);
        Crab_Type crabType = (Crab_Type)crabTypeIndex;

        // Determine the starting location for the crab
        Grid_Square crabStartSquare = m_grid.GetRandCastleSquare(_crabTeam == Crab_Team.Left_Team);
        Vector3 startSquareWorldPos = m_grid.GetWorldPosFromCoord(crabStartSquare.GridCoord);

        // Instantiate a new crab
        GameObject crabObj = Instantiate(m_crabPrefab, startSquareWorldPos, Quaternion.identity, m_crabParent);

        // Setup the crab's internal data
        Crab_Controller crabScript = crabObj.GetComponent<Crab_Controller>();
        crabScript.Init(crabType, _crabTeam, crabStartSquare, m_grid);

        // Store the crab in the relevant list since it exists in the world now
        if (_crabTeam == Crab_Team.Left_Team)
            m_leftTeamCrabs.Add(crabScript);
        else
            m_rightTeamCrabs.Add(crabScript);

        // Return the newly spawned crab
        return crabScript;
    }

    public void MoveCrabs(Crab_Team _crabTeam)
    {
        if (_crabTeam == Crab_Team.Left_Team)
            MoveCrabsInList(m_leftTeamCrabs);
        else
            MoveCrabsInList(m_rightTeamCrabs);
    }

    public void SetUpCrabGrid(List<Grid_Row> _gridRows)
    {
        // Initialize the mapping
        m_crabMappings = new Dictionary<Grid_Square, Crab_Controller>();

        // Add all of the squares to the list
        foreach(Grid_Row row in _gridRows)
        {
            foreach(Grid_Square square in row.m_squares)
            {
                m_crabMappings.Add(square, null);
            }
        }
    }

    public Crab_Controller DetermineCrabFightWinner(Crab_Controller _crabA, Crab_Controller _crabB)
    {
        // Return the winner of the fight or null if both crabs have died
        if (_crabA.CrabType == Crab_Type.Rock)
        {
            switch (_crabB.CrabType)
            {
                case (Crab_Type.Rock):
                    return null;

                case (Crab_Type.Paper):
                    return _crabB;

                case (Crab_Type.Scissors):
                    return _crabA;
            }
        }
        else if (_crabA.CrabType == Crab_Type.Paper)
        {
            switch (_crabB.CrabType)
            {
                case (Crab_Type.Rock):
                    return _crabA;

                case (Crab_Type.Paper):
                    return null;

                case (Crab_Type.Scissors):
                    return _crabB;
            }
        }
        else if (_crabA.CrabType == Crab_Type.Scissors)
        {
            switch (_crabB.CrabType)
            {
                case (Crab_Type.Rock):
                    return _crabB;

                case (Crab_Type.Paper):
                    return _crabA;

                case (Crab_Type.Scissors):
                    return null;
            }
        }

        return null;
    }

    public System.Collections.IEnumerator HandleCrabDeath(Crab_Controller _crab)
    {
        // Remove it from the grid
        Grid_Square crabSquare = _crab.GridMover.CurrentSquare;
        SetCrabAtSquare(crabSquare, null);

        // Need to wait until the end of frame before removing from the lists
        // This is because the list is likely iterating at the moment from the MoveCrabsInList() function
        // Changing the list now will cause an exception in that function
        yield return new WaitForEndOfFrame();

        // Remove it from the relevant list
        if (m_leftTeamCrabs.Contains(_crab)) m_leftTeamCrabs.Remove(_crab);
        if (m_rightTeamCrabs.Contains(_crab)) m_rightTeamCrabs.Remove(_crab);
    }



    //--- Setters and Getters ---//
    public void SetCrabAtSquare(Grid_Square _square, Crab_Controller _crab)
    {
        m_crabMappings[_square] = _crab;
    }

    public Crab_Controller GetCrabAtSquare(Grid_Square _square)
    {
        return m_crabMappings[_square];
    }



    //--- Utility Functions ---//
    private void MoveCrabsInList(List<Crab_Controller> _crabs)
    {
        // Iterate through all of the crabs and move them
        // This method for moving means that the oldest crabs move first
        foreach (var crab in _crabs)
            crab.PerformMovements();
    }
}
