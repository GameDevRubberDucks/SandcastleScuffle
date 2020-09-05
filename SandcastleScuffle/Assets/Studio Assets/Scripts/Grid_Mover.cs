using UnityEngine;

public enum Grid_MoveDir
{
    Up,
    Down,
    Left,
    Right
}

public class Grid_Mover : MonoBehaviour
{
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
        this.transform.position = m_currentSquare.transform.position;
    }

    public void Move(Grid_MoveDir _direction, int _distance = 1)
    {
        // Get the current square's coordinate
        Vector2 newGridLoc = m_currentSquare.GridCoord;
        
        // Calculate the new coordinate by moving in the given direction
        switch(_direction)
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

        // Move to the new coordinate
        MoveTo(newGridLoc);
    }
}
