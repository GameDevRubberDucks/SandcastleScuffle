using System.Collections;
using UnityEngine;

public enum Crab_Type
{
    Rock,
    Paper,
    Scissors,

    Num_Types
}
public enum Crab_Team
{
    Left_Team,
    Right_Team,

    Num_Teams
}

public class Crab_Controller : MonoBehaviour
{
    //--- Public Variables ---//
    public int m_movementCountPerTurn;



    //--- Private Variables ---//
    private Crab_Visuals m_visuals;
    private Grid_Mover m_gridMover;
    private Crab_Type m_crabType;
    private Crab_Team m_crabTeam;
    private Grid_MoveDir m_defaultMoveDir;
    private bool m_isPoweredUp;



    //--- Methods ---//
    public void Init(Crab_Type _type, Crab_Team _team, Grid_Square _startSquare, Grid_Controller _grid)
    {
        // Init the private variables
        m_visuals = GetComponent<Crab_Visuals>();
        m_gridMover = GetComponent<Grid_Mover>();
        m_gridMover.PlaceOnGrid(_grid, _startSquare.GridCoord);
        m_gridMover.MoveTo(_startSquare.GridCoord);
        CrabType = _type;
        CrabTeam = _team;
        IsPoweredUp = false;
    }

    public void PerformMovements()
    {
        // Move as many times as needed
        for (int i = 0; i < m_movementCountPerTurn; i++)
            MoveToNextSquare();
    }

    public void MoveToNextSquare()
    {
        // Determine if the current square has a card that indicates the next direction
        // If it does, we should move in that direction. Otherwise, we should just move in the crab's default direction
        Grid_MoveDir currentGridMoveDir = m_gridMover.CurrentSquare.MoveDir;
        Grid_MoveDir dirToMove = (currentGridMoveDir != Grid_MoveDir.None) ? currentGridMoveDir : m_defaultMoveDir;

        // Move in the desired direction
        m_gridMover.Move(dirToMove);
    }



    //--- Utility Functions ---//
    private IEnumerator AnimateMovement(Vector3 _startPos, Vector3 _endPos, float _duration)
    {
        int numFrames = Mathf.CeilToInt(_duration / Time.deltaTime);

        float distanceToTravel = Vector3.Distance(_startPos, _endPos);
        Vector3 movementDir = Vector3.Normalize(_endPos - _startPos);

        for (int i = 0; i < numFrames; i++)
        {

            yield return new WaitForEndOfFrame();
        }
    }



    //--- Setters and Getters ---//
    public Crab_Type CrabType
    {
        set
        {
            m_crabType = value;
            m_visuals.SetTypeVisuals(m_crabType);
        }

        get => m_crabType;
    }

    public Crab_Team CrabTeam
    {
        set
        {
            m_crabTeam = value;
            m_visuals.SetTeamVisuals(m_crabTeam);
            m_defaultMoveDir = (m_crabTeam == Crab_Team.Left_Team) ? Grid_MoveDir.Right : Grid_MoveDir.Left;
        }

        get => m_crabTeam;
    }

    public bool IsPoweredUp
    {
        set
        {
            m_isPoweredUp = value;
        }

        get => m_isPoweredUp;
    }

    public Grid_Mover GridMover
    {
        get => m_gridMover;
    }
}
