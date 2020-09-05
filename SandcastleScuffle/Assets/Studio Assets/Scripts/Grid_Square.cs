using UnityEngine;

public enum Map_SquareType
{
    Path,
    Sandcastle
}

public class Grid_Square : MonoBehaviour
{
    //--- Public Variables ---//
    public Map_SquareType m_squareType;

    //--- Private Variables ---//
    private Vector2 m_coord;
}
