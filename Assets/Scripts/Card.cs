using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int val;  // TODO reconsider scope
    public FreeCellLogic.Suits suit;
    
    // fields for interactability
    public bool isClickable = false;
    public bool isInFreeCellSpace = false;
    public int column = 0;

    // public Sprite sprite;
    private SpriteRenderer _spriteRenderer;
    private Selectable selectable;
    private FreeCellLogic freeCellLogicScript;

    private bool isSpriteAssigned = false;

    private void Update()
    {
        if (!isSpriteAssigned)
        {
            UpdateCardFace();
        }
    }

    public void UpdateCardFace()
    {
        int index = FreeCellLogic.cardIndexMap[val.ToString()+suit];
        freeCellLogicScript = FindObjectOfType<FreeCellLogic>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = freeCellLogicScript.cardAssignments[index];
    }
}
