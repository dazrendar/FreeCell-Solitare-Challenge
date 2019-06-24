using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private FreeCellLogic freeCellLogicScript;
    private GameObject currentHeldCard;
    private Vector3 originalMousePos;
    private bool isMouseBeingHeld;
    private Vector3 originalCardPos;
    private float mouseStartTimer = 0f;
    private float mouseUpdateTimer;
    public float mouseClickTimeDelay = 0.22f;
    
    // Start is called before the first frame update
    void Start()
    {
        freeCellLogicScript = FindObjectOfType<FreeCellLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        GetMouseClick();
        mouseUpdateTimer = Time.time - mouseStartTimer;
        
        if (isMouseBeingHeld && mouseUpdateTimer > mouseClickTimeDelay)
        {
            //Debug.Log(mouseUpdateTimer);
            DraggedCard(currentHeldCard);
        }
        
    }

    void GetMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseStartTimer = Time.time;
            isMouseBeingHeld = true;
            Vector3 mousePosition =
                Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -15));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            // 
            if (hit && hit.collider.CompareTag("Card"))
            {

                currentHeldCard = hit.collider.gameObject;
                originalCardPos = currentHeldCard.transform.position;

            }
            
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseBeingHeld = false; // todo remove
            Vector3 mousePosition =
                Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -15));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            
            // Case - Short click:
            if (mouseUpdateTimer < mouseClickTimeDelay)
            {
                
                if (hit)
                {
                    if (hit.collider.CompareTag("Card"))
                    {
                        ClickedCard(hit.collider.gameObject);
                    }
                }
            }
            
            // Case - Held click
            else
            {
                if (currentHeldCard != null)
                {
                    freeCellLogicScript.SnapCard(mousePosition, currentHeldCard, originalCardPos);
                }

            }
            currentHeldCard = null;
            
        }
    }
    
    private void DraggedCard(GameObject clickedCard)
    {
        freeCellLogicScript.MoveCardAround(clickedCard);
    }

    private void ClickedCard(GameObject clickedCard)
    {
        //Debug.Log("Clicked a Card");
        freeCellLogicScript.HandleCardClick(clickedCard);
    }
}
