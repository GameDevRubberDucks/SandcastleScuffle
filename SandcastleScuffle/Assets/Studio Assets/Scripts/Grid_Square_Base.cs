using UnityEngine;

public class Grid_Square_Base : MonoBehaviour
{
    private Base_Controller m_baseController;

    private void Awake()
    {
        // Init the private variables
        m_baseController = FindObjectOfType<Base_Controller>();
    }

    public void TakeDamage()
    {
        m_baseController.TakeDamage(Team);
    }

    public Crab_Team Team
    {
        get => GetComponent<Grid_Square>().Team;
    }
}
