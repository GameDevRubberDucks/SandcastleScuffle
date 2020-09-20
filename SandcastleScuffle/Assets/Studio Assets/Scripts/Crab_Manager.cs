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



    //--- Utility Functions ---//
    private void MoveCrabsInList(List<Crab_Controller> _crabs)
    {
        // Iterate through all of the crabs and move them
        // This method for moving means that the oldest crabs move first
        foreach (var crab in _crabs)
            crab.PerformMovements();
    }
}
