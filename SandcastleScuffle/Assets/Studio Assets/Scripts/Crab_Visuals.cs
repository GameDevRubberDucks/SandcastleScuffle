using UnityEngine;

public class Crab_Visuals : MonoBehaviour
{
    //--- Public Variables ---//
    [Header("Scene References")]
    public SpriteRenderer m_sprRend;
    public SpriteRenderer m_stoppedSprite;

    [Header("Team Colours")]
    public Color m_leftTeamColour;
    public Color m_rightTeamColour;

    [Header("Sprite Variations")]
    public Sprite m_rockSprite;
    public Sprite m_paperSprite;
    public Sprite m_scissorsSprite;

    [Header("Death Feedback")]
    public Sprite m_deadSprite;
    public float m_deathDuration;

    [Header("Stopped Feedback")]
    public float m_stoppedFeedbackDuration;



    //--- Methods ---//
    public void SetTeamVisuals(Crab_Team _newTeam)
    {
        // Apply the colours
        m_sprRend.color = (_newTeam == Crab_Team.Left_Team) ? m_leftTeamColour : m_rightTeamColour;
        m_stoppedSprite.color = m_sprRend.color;

        // Flip the visuals so the crabs face the main direction they are moving
        m_sprRend.transform.localScale = (_newTeam == Crab_Team.Left_Team) ? Vector3.one : new Vector3(-1.0f, 1.0f, 1.0f);
        m_stoppedSprite.transform.localScale = m_sprRend.transform.localScale;
    }

    public void SetTypeVisuals(Crab_Type _newType)
    {
        switch(_newType)
        {
            case Crab_Type.Rock:
                m_sprRend.sprite = m_rockSprite;
                break;

            case Crab_Type.Paper:
                m_sprRend.sprite = m_paperSprite;
                break;

            case Crab_Type.Scissors:
            default:
                m_sprRend.sprite = m_scissorsSprite;
                break;
        }
    }

    public void ShowDeathSprite()
    {
        m_sprRend.sprite = m_deadSprite;
    }

    public void ShowStoppedSprite()
    {
        m_stoppedSprite.gameObject.SetActive(true);

        Invoke("HideStoppedSprite", m_stoppedFeedbackDuration);
    }



    //--- Utility Functions ---//
    private void HideStoppedSprite()
    {
        m_stoppedSprite.gameObject.SetActive(false);
    }
}
