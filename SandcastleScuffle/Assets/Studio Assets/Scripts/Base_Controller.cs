using UnityEngine;
using TMPro;

public class Base_Controller : MonoBehaviour
{
    //--- Public Variables ---//
    public int m_startingHP;
    public TextMeshProUGUI m_txtLeftHP;
    public TextMeshProUGUI m_txtRightHP;
    public GameObject m_leftTeamWins;
    public GameObject m_rightTeamWins;



    //--- Private Variables ---//
    private int m_leftHP;
    private int m_rightHP;
    private Crab_Team m_winner;



    //--- Unity Methods ---//
    private void Awake()
    {
        // Init the private variables
        m_leftHP = m_startingHP;
        m_rightHP = m_startingHP;
        m_winner = Crab_Team.Neutral;

        UpdateUI();
    }



    //--- Methods ---//
    public void TakeDamage(Crab_Team _team)
    {
        if (_team == Crab_Team.Left_Team)
            m_leftHP--;
        else if (_team == Crab_Team.Right_Team)
            m_rightHP--;

        if (m_winner == Crab_Team.Neutral && m_leftHP <= 0)
            m_winner = Crab_Team.Right_Team;
        else if (m_winner == Crab_Team.Neutral && m_rightHP <= 0)
            m_winner = Crab_Team.Left_Team;

        UpdateUI();
    }

    public void UpdateUI()
    {
        m_txtLeftHP.text = m_leftHP.ToString();
        m_txtRightHP.text = m_rightHP.ToString();

        m_leftTeamWins.SetActive(m_winner == Crab_Team.Left_Team);
        m_rightTeamWins.SetActive(m_winner == Crab_Team.Right_Team);
    }
}
