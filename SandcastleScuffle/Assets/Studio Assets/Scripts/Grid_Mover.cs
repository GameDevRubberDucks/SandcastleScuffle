using UnityEngine;
using System.Collections;

public enum Grid_MoveDir
{
    None,

    Up,
    Down,
    Left,
    Right,

    Count
}

public class Grid_Mover : MonoBehaviour
{
    //--- Public Variables ---//
    public float m_movementDuration;



    //--- Private Variables ---//
    private Grid_Controller m_grid;
    private Grid_Square m_currentSquare;



    //--- Methods ---//
    public void PlaceOnGrid(Grid_Controller _grid, Vector2 _startCoord)
    {
        // Store the data
        m_grid = _grid;

        // Move to the starting coordinate
        MoveTo(_startCoord);
    }

    public void MoveTo(Vector2 _newGridLoc)
    {
        // Grab the new square from the grid
        m_currentSquare = m_grid.GetGridSquare(_newGridLoc);

        // Move to the square's position
        //this.transform.position = m_currentSquare.transform.position;
        StopAllCoroutines();
        StartCoroutine(AnimateMovement(this.transform.position, m_currentSquare.transform.position, m_movementDuration));
    }

    public void Move(Grid_MoveDir _direction, int _distance = 1)
    {
        // Determine the new coordinate
        Vector2 newGridLoc = NextGridLocation(m_currentSquare.GridCoord, _direction, _distance);

        // Move to the new coordinate
        MoveTo(newGridLoc);
    }

    public Vector2 NextGridLocation(Vector2 _startLoc, Grid_MoveDir _direction, int _distance = 1)
    {
        // Get the current square's coordinate
        Vector2 newGridLoc = _startLoc;

        // Calculate the new coordinate by moving in the given direction
        switch (_direction)
        {
            case Grid_MoveDir.Right:
                newGridLoc += (Vector2.right * _distance);
                break;

            case Grid_MoveDir.Down:
                newGridLoc += (Vector2.down * _distance);
                break;

            case Grid_MoveDir.Left:
                newGridLoc += (Vector2.left * _distance);
                break;

            case Grid_MoveDir.Up:
            default:
                newGridLoc += (Vector2.up * _distance);
                break;
        }

        // Return the new value
        return newGridLoc;
    }

    public Grid_Square DetermineNextGridSquare(Grid_MoveDir _direction, int _distance = 1)
    {
        // Determine the new coordinate
        Vector2 newGridLoc = NextGridLocation(m_currentSquare.GridCoord, _direction, _distance);

        // Determine which square it falls under and return it
        return m_grid.GetGridSquare(newGridLoc);
    }



    //--- Utility Functions ---//
    private IEnumerator AnimateMovement(Vector3 _startPos, Vector3 _endPos, float _duration)
    {
        int numFrames = Mathf.CeilToInt(_duration / Time.deltaTime);

        float distanceToTravel = Vector3.Distance(_startPos, _endPos);
        float distancePerFrame = distanceToTravel / (float)numFrames;

        Vector3 movementDir = Vector3.Normalize(_endPos - _startPos);

        for (int i = 0; i < numFrames; i++)
        {
            this.transform.position += (movementDir * distancePerFrame);
            yield return new WaitForEndOfFrame();
        }
    }



    //--- Setters and Getters ---//
    public Grid_Square CurrentSquare
    {
        get => m_currentSquare;
    }
}
