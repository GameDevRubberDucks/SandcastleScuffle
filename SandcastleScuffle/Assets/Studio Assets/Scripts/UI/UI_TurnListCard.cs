using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Transactions;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public enum CardState
{
    CURRENT,
    NEXT,
    FUTURE,
    END

}
public enum CardEvent
{
    PLAYER_MOVE,
    AI_MOVE,
    ENVIROMENT,
    OTHER,
    END
}

public class UI_TurnListCard : MonoBehaviour
{
    //public vars
    //enum of different card types and effects
    [Header("Card Type")]
    public CardState cardState;
    public CardEvent cardEvent;

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
        // get background component
        bgColour = GetComponent<Image>();

        //get sprite component from icon object
        icon = iconComponent.GetComponent<Image>();

        //setup background color array
        stateBackgroundColours = new Color[3] { currentTurn, nextTurn, futureTurn };

        //update card UIs
        CardUpdate();

    }

    // Update is called once per frame
    void Update()
    {
        CardUpdate();
    }

    void CardUpdate()
    {
        //set the backgorund colour base on the state chosen for the card.
        bgColour.color = stateBackgroundColours[(int)cardState];

        //if the event enum is out of range then just return function.
        if ((int)cardEvent >= eventIcons.Length)
        {
            return;
        }
        else
        {
            //set the icon of the card based on the event it represents
            icon.sprite = eventIcons[(int)cardEvent];
        }
    }
}
