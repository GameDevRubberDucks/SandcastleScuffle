using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class UI_HandDeckManager : MonoBehaviour
{
    //public
    public List<Transform> handDeck;
    public float cardSpacingLerpLength;

    //private
    private float Lerptimer;
    private bool cardSpacingAnimationSwitch;


    // Start is called before the first frame update
    void Start()
    {
        Lerptimer = 0;
        cardSpacingAnimationSwitch = false;

        GetCards();

    }

    // Update is called once per frame
    void Update()
    {

        //if card spacing needs to run
        if (cardSpacingAnimationSwitch)
        {
            if (Lerptimer < cardSpacingLerpLength)
            {
                Lerptimer += Time.deltaTime;
                CardSpacing();
            }
            else
                resetTimer();
        }


    }

    //PROTO FUNCTION ONLY:
    //this is only used to get all the hard currently in the scene. this shouldn't be used in the final version
    void GetCards()
    {
        foreach (Transform eachChild in this.transform)
        {
            if (eachChild.GetType() == typeof(Transform))
            {
                handDeck.Add(eachChild);
            }
        }
        //enable lerp animation timer
        cardSpacingAnimationSwitch = true;
    }

    //evenly space out the cards at hand
    void CardSpacing()
    {
        for (int i = 0; i < handDeck.Count - 1; i++)
        {

            //if the next card exist and is not 2 units apart then lerp til 2 units apart
            if (handDeck[i + 1] != null && (Vector2.Distance(handDeck[i].position, handDeck[i + 1].position) != cardSpacingLerpLength))
            {

                //calculate new position based on the last card's position
                Vector2 newPos = new Vector2(handDeck[i].position.x + 2.0f, handDeck[i].position.y);

                //lerp to new position
                handDeck[i + 1].position = Vector2.Lerp(handDeck[i + 1].position, newPos, cardSpacingLerpLength);
                handDeck[i].GetComponent<UI_GameCard>().updatePos();
                handDeck[i+1].GetComponent<UI_GameCard>().updatePos();
            }
        }
    }



    void resetTimer()
    {
        Lerptimer = 0;
        cardSpacingAnimationSwitch = false;
    }


}