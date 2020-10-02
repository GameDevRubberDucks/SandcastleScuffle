using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Transactions;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public enum TurnState
{
    CURRENT,
    NEXT,
    FUTURE,
    END

}
public enum TurnEvent
{
    PLAYER_MOVE,
    PLAYER_PATHING,
    AI_MOVE,
    AI_PATHING,
    ENVIROMENT,
    OTHER,
    END
}

public class UI_TurnListCard : MonoBehaviour
{
    //public vars
    //enum of different card types and effects
    [Header("Card Type")]
    public TurnState turnState;
    public TurnEvent turnEvent;

    //icon components and related sprites
    [Header("Icon Sprites")]
    public GameObject iconComponent;
    public Sprite[] eventIcons;

    //Card backgroun color depending on their state
    [Header("State Background Colours")]
    public Color currentTurn;
    public Color nextTurn;
    public Color futureTurn;


    //private var
    private Image bgColour;
    private Image icon;
    private Color[] stateBackgroundColours;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {

    }

    //used to make sure that it runs the start function when instatiated 
    private void Awake()
    {

    }


    //init function
    public void Init()
    {
        turnState = TurnState.FUTURE;

        // get background component
        bgColour = GetComponent<Image>();
        bgColour.color = new Color(0.0f, 0.0f, 0.0f);

        //get sprite component from icon object
        icon = iconComponent.GetComponent<Image>();
        icon.sprite = eventIcons[0];

        //setup background color array
        stateBackgroundColours = new Color[3] { currentTurn, nextTurn, futureTurn };

        //update card UIs
        CardUpdate();
    }


    //update the card
    public void CardUpdate()
    {

        //this mostly used to avoid hitting the END enum (used like .length)
        if ((int)turnState < stateBackgroundColours.Length)
        {
            //set the backgorund colour base on the state chosen for the card.
            bgColour.color = stateBackgroundColours[(int)turnState];
        }

        // same use as the above
        if ((int)turnEvent < eventIcons.Length)
        {
            //set the icon of the card based on the event it represents
            icon.sprite = eventIcons[(int)turnEvent];
        }
    }
}
