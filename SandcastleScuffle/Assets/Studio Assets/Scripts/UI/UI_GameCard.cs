using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class UI_GameCard : MonoBehaviour
{

    public bool isMouseHover;

    Vector2 ogVPos;
    Vector2 newVPos;

    // Start is called before the first frame update
    void Start()
    {
        updatePos();
        isMouseHover = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMouseHover)
        {
            cardReading();
        }
        else
        {
            //lerp to new position
            transform.position = Vector2.Lerp(transform.position, ogVPos, 1.0f);
        }
        //reset
        isMouseHover = false;
    }

    //doesn't work
   public void cardReading()
    {

        //lerp to new position
        transform.position = Vector2.Lerp(transform.position, newVPos, 1.0f);
    }

    public void updatePos()
    {
        newVPos = new Vector2(transform.position.x, transform.position.y + 3.0f);
        ogVPos = new Vector2(transform.position.x, transform.position.y);
    }

}
