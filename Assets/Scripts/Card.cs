using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int val;
    public FreeCellLogic.Suits suit;
    public Sprite cardFace;
    
    // fields for interactability
    public bool isMoveable = false;
    public bool isClickable = false;
    public bool isInFreeCellSpace = false;
    public int column = 0;

    // public Sprite sprite;
    private SpriteRenderer _spriteRenderer;
    private Selectable selectable;
    private FreeCellLogic freeCellLogicScript;

    public bool isCardDataUpdated = false;

    private bool isSpriteAssigned = false;
    // public GameObject thisCard;

    // Start is called before the first frame update
    void Start()
    {
        // todo cannot have this in start?
        // string indexKeyword = 
        
    }

    private void Update()
    {
        //Debug.Log("====");
        //Debug.Log(isCardDataUpdated);
        //Debug.Log(isSpriteAssigned);
        if (isCardDataUpdated && !isSpriteAssigned)
        {
            UpdateCardFace();
        }
    }

    // todo remove useless
    public void UpdateCardProps(FreeCellLogic.Suits suit, int val)
    {
        suit = suit;
        val = val;
        isCardDataUpdated = true;
    }

    public void UpdateCardFace()
    {
        int index = FreeCellLogic.cardIndexMap[val.ToString()+suit];
        freeCellLogicScript = FindObjectOfType<FreeCellLogic>();
        //Debug.Log("CARD = " + suit + val + " and index = " + index);
        //Debug.Log(freeCellLogicScript.cardAssignments + "******");

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = freeCellLogicScript.cardAssignments[index];


        //selectable = GetComponent<Selectable>(); // todo add clickable logic


    }
}
