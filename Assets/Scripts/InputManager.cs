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

                Debug.Log("MousePos = " + mousePosition);
               
            }
            currentHeldCard = null;
            
        }
    }
    
  
/*
    void GetMouseClickOLDVERSION()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentHeldCard != null)
            {
                currentHeldCard.transform.position = originalCardPos;
                isMouseBeingHeld = false;
                currentHeldCard = null;
            }
            else
            {
                Vector3 mousePosition =
                    Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -15));
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit)
                {
                    if (hit.collider.CompareTag("Field"))
                    {
                        ClickedField(); // TODO
                    }
                    else if (hit.collider.CompareTag("Free"))
                    {
                        ClickedFree(); // REMOVE?
                    }
                    else if (hit.collider.CompareTag("Solution"))
                    {
                        ClickedSolution(); // Remove?
                    }
                    else if (hit.collider.CompareTag("Card"))
                    {
                        Debug.Log("Clicked a Card");
                    }
                }
            }
            
            
        }

        // for dragging cards
        if (Input.GetMouseButton(0))
        {
            if (!isMouseBeingHeld)
            {
                originalMousePos = Input.mousePosition;
                isMouseBeingHeld = true;
            }
            else if (Input.mousePosition != originalMousePos)
            {
                GameObject hitCard;
                Vector3 mousePosition =
                    Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -15));
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit)
                {
                    if (hit.collider.CompareTag("Card"))
                    {
                        hitCard = hit.collider.gameObject;
                        if (currentHeldCard == null)
                        {
                            currentHeldCard = hitCard;
                            originalCardPos = hitCard.transform.position;
                        }

                    }
                }

                DraggedCard(currentHeldCard);
            }
        }
        
    }
    */

    private void DraggedCard(GameObject clickedCard)
    {
        freeCellLogicScript.MoveCardAround(clickedCard);
    }

    private void ClickedCard(GameObject clickedCard)
    {
        //Debug.Log("Clicked a Card");
        freeCellLogicScript.HandleCardClick(clickedCard);
    }

    private void ClickedSolution()
    {
        Debug.Log("Clicked a Solution Square");
    }

    private void ClickedFree()
    {
        Debug.Log("Clicked a Free Square");
    }

    private void ClickedField()
    {
        Debug.Log("Clicked a Field Square");
    }

    private GameObject retrieveNearbyGameObject()
    {
        // todo implement
        return null;
    }
}
