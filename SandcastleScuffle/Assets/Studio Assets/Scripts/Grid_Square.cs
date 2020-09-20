using UnityEngine;

[System.Serializable]
public enum Grid_SquareType
{
    Path,
    Sandcastle
}

[System.Serializable]
public class Grid_Square : MonoBehaviour
{
    //--- Public Variables ---//
    public Grid_SquareType m_squareType;



    //--- Private Variables ---//
    [SerializeField] private Grid_MoveDir m_moveDir;
    [SerializeField] private Vector2 m_coord;



    //--- Setters and Getters ---//
    public Grid_MoveDir MoveDir
    {
        set => m_moveDir = value;
        get => m_moveDir;
    }

    public Vector2 GridCoord
    {
        set => m_coord = value;
        get => m_coord;
    }
}
