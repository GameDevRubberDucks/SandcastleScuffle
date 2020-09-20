using UnityEngine;

public class Crab_Visuals : MonoBehaviour
{
    //--- Public Variables ---//
    [Header("Scene References")]
    public SpriteRenderer m_sprRend;

    [Header("Team Colours")]
    public Color m_leftTeamColour;
    public Color m_rightTeamColour;

    [Header("Sprite Variations")]
    public Sprite m_rockSprite;
    public Sprite m_paperSprite;
    public Sprite m_scissorsSprite;



    //--- Methods ---//
    public void SetTeamVisuals(Crab_Team _newTeam)
    {
        m_sprRend.color = (_newTeam == Crab_Team.Left_Team) ? m_leftTeamColour : m_rightTeamColour;
        m_sprRend.transform.localScale = (_newTeam == Crab_Team.Left_Team) ? Vector3.one : new Vector3(-1.0f, 1.0f, 1.0f);
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
}
