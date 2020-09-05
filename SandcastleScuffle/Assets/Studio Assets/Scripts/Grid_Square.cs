using UnityEngine;

public enum Grid_SquareType
{
    Path,
    Sandcastle
}

public class Grid_Square : MonoBehaviour
{
    //--- Public Variables ---//
    public Grid_SquareType m_squareType;

    //--- Private Variables ---//
    private Vector2 m_coord;

    //--- Setters and Getters ---//
    public Vector2 GridCoord
    {
        set => m_coord = value;
        get => m_coord;
    }
}
