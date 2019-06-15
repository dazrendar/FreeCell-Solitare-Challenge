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
    
    // Start is called before the first frame update
    void Start()
    {
        freeCellLogicScript = FindObjectOfType<FreeCellLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        GetMouseClick();
    }

    void GetMouseClick()
    {
        if (Input.GetMouseButtonUp(0))
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
                        ClickedField();
                    }
                    else if (hit.collider.CompareTag("Free"))
                    {
                        ClickedFree();
                    }
                    else if (hit.collider.CompareTag("Solution"))
                    {
                        ClickedSolution();
                    }
                    else if (hit.collider.CompareTag("Card"))
                    {
                        ClickedCard(hit.collider.gameObject);
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

    private void DraggedCard(GameObject clickedCard)
    {
        freeCellLogicScript.MoveCardAround(clickedCard);
    }

    private void ClickedCard(GameObject clickedCard)
    {
        //Debug.Log("Clicked a Card");
        freeCellLogicScript.TryToMoveCardToEmptySlot(clickedCard);
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
}
