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
    private Crab_Manager m_crabManager;
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
        m_crabManager = FindObjectOfType<Crab_Manager>();
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

        // Get the square that this crab would be moving to 
        Grid_Square nextSquare = m_gridMover.DetermineNextGridSquare(dirToMove);

        // Determine what crab is currently at the square in question
        Crab_Controller crabAtSquare = m_crabManager.GetCrabAtSquare(nextSquare);

        // If the square is empty, we can just move
        if (crabAtSquare == null)
        {
            // Tell the manager that this crab has moved so the current space is now null and the new space has this crab at it
            m_crabManager.SetCrabAtSquare(m_gridMover.CurrentSquare, null);
            m_crabManager.SetCrabAtSquare(nextSquare, this);

            // Move to the new square
            m_gridMover.Move(dirToMove);
        }
        else
        {
            // If it is a friendly crab, we should stay still so don't do anything
            // If it is an enemy crab, we should fight it and see who wins
            if (crabAtSquare.m_crabTeam == this.m_crabTeam)
            {
                // It is a friendly crab so we should show the feedback to say that this crab will not move
                m_visuals.ShowStoppedSprite();
            }
            else
            {
                // Determine the winner of the fight
                Crab_Controller crabFightWinner = m_crabManager.DetermineCrabFightWinner(this, crabAtSquare);

                // If we are the winning crab, move to the new square and destroy the other one
                // If we are the losing crab, destroy us
                // If it is a tie, destroy both
                if (crabFightWinner == this)
                {
                    // Tell the manager that this crab has moved so the current space is now null and the new space has this crab at it
                    m_crabManager.SetCrabAtSquare(m_gridMover.CurrentSquare, null);
                    m_crabManager.SetCrabAtSquare(nextSquare, this);

                    // Move to the new square
                    m_gridMover.Move(dirToMove);

                    // Kill the other crab
                    crabAtSquare.Die();
                }
                else if (crabFightWinner == crabAtSquare)
                {
                    // Kill this crab
                    Die();
                }
                else if (crabFightWinner == null)
                {
                    // Kill both crabs
                    crabAtSquare.Die();
                    Die();
                }
                else
                {
                    Debug.LogError("Unexpected crab winner. Was not crabA, crabB, or null? [" + crabFightWinner.ToString() + "]");
                }
            }
        }       
    }

    public void Die()
    {
        // Tell the manager this crab has died so its square is now unoccupied
        StartCoroutine(m_crabManager.HandleCrabDeath(this));

        // Update the visuals for the crab
        m_visuals.ShowDeathSprite();

        // Destroy this object
        Destroy(this.gameObject, m_visuals.m_deathDuration);
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
