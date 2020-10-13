using UnityEngine;

public class Grid_Square_Castle : MonoBehaviour
{
    public Color m_destroyedColour;

    private SpriteRenderer m_spriteRenderer;
    private bool m_isDestroyed;

    private void Awake()
    {
        // Init the private variables
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_isDestroyed = false;
    }

    public void MarkAsDestroyed()
    {
        m_isDestroyed = true;
        m_spriteRenderer.color = m_destroyedColour;
    }

    public bool IsDestroyed
    {
        get => m_isDestroyed;
    }

    public Crab_Team Team
    {
        get => GetComponent<Grid_Square>().Team;
    }
}
