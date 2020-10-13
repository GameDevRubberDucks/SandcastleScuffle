using UnityEngine;

public class Grid_Square_Base : MonoBehaviour
{
    public void TakeDamage()
    {
        Debug.Log(Team + "'s base has been damaged");
    }

    public Crab_Team Team
    {
        get => GetComponent<Grid_Square>().Team;
    }
}
