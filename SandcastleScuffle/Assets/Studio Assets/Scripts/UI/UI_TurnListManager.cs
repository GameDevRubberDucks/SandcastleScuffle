using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UI_TurnListManager : MonoBehaviour
{
    public UI_TurnListCard listcardPrefab;
    public GameObject content;
    public List<UI_TurnListCard> turnList;

    // Start is called before the first frame update
    void Start()
    {
        //TEMP: make a new random list of events
        List_Gen(10);

    }

    void Update()
    {

    }

    // remove the current turn and 
    public void NextTurn()
    {
        if (turnList.Count >= 1)
        {
            //remove the card from the turnlist
            turnList.RemoveAt(0);

            //delete the gameObject assoiated with the card
            DestroyImmediate(content.transform.GetChild(0).gameObject);

            //update the list order
            UpdateListState();
        }
    }

    //add a new turn at the end of the turn list, default version for UI use
    public void AddNewTurn()
    {
        UI_TurnListCard newCard = Instantiate(listcardPrefab);

        //assign a defualt event type
        newCard.turnEvent = TurnEvent.OTHER;

        //parent to the content object 
        newCard.transform.SetParent(content.transform);

        //add the new card to the turnlist;
        turnList.Add(newCard);

        //update the list order
        UpdateListState();

    }

    //variation of add new turn method. This version requires a TurnEvent as an argument. For internal use
    public void AddNewTurn(TurnEvent t_event)
    {
        UI_TurnListCard newCard = Instantiate(listcardPrefab);

        //assign a event type based on argument
        newCard.turnEvent = t_event;

        //parent to the content object 
        newCard.transform.SetParent(content.transform);

        //add the new card to the turnlist;
        turnList.Add(newCard);

        //update the list order
        UpdateListState();
    }

    //return the current turn
    public UI_TurnListCard GetCurrentTurn()
    {
        return turnList[0];
    }

    //just update the first and second items in the list. everything else just stays as future
    public void UpdateListState()
    {
        //update the turn state to current
        if (turnList.Count >= 1)
        {
            turnList[0].turnState = TurnState.CURRENT;
            turnList[0].CardUpdate();
        }

        //update the turn state to next
        if (turnList.Count >= 2)
        {
            turnList[1].turnState = TurnState.NEXT;
            turnList[1].CardUpdate();
        }
    }

    //generate a new random list
    public void List_Gen(int size)
    {
        for (int i = 0; i < size; i++)
        {
            AddNewTurn((TurnEvent)Random.Range(0, 5));
        }
    }
}

