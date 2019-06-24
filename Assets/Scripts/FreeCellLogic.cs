using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FreeCellLogic : MonoBehaviour
{
    public enum Suits
    {
        Hearts,
        Spades,
        Clubs,
        Diamonds
    };

    private static readonly int[] Values = new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13};
    private Card Card;

    
    public Sprite[] cardAssignments;
    public GameObject genericCardPrefab;
    public float yOffset = 0.3f;
    public float zOffset = 0.02f;
    public static Dictionary<string, int> cardIndexMap;
    public static List<Tuple<Suits, int>> unshuffledDeck;
    public float xPlacementOffset = 0.65f;
    public float yPlacementOffset = 1f;
    public GameObject introSplash;
    public GameObject endSplash;
    
    
   
    
    // For aligning the cards
    public GameObject[] freeCellsPos;
    public GameObject[] solutionCellsPos;
    public GameObject[] fieldCellsPos;
    
    public List<GameObject> freeCells;
    public List<Stack<GameObject>> fieldCellsV2;
    public List<Stack<GameObject>> solutionCells;
    public List<List<GameObject>> cardObjects = new List<List<GameObject>>();

    // Stack to hold the cards; one stack per column in field cells section
    private Stack<GameObject> field0V2 = new Stack<GameObject>();
    private Stack<GameObject> field1V2 = new Stack<GameObject>();
    private Stack<GameObject> field2V2 = new Stack<GameObject>();
    private Stack<GameObject> field3V2 = new Stack<GameObject>();
    private Stack<GameObject> field4V2 = new Stack<GameObject>();
    private Stack<GameObject> field5V2 = new Stack<GameObject>();
    private Stack<GameObject> field6V2 = new Stack<GameObject>();
    private Stack<GameObject> field7V2 = new Stack<GameObject>();
    
    // To represent the card piles we have passed to the solution
    private Stack<GameObject> solution0 = new Stack<GameObject>(); // Index 0 = Clubs
    private Stack<GameObject> solution1 = new Stack<GameObject>(); // 1 = Spades
    private Stack<GameObject> solution2 = new Stack<GameObject>(); // 2 = Diamonds
    private Stack<GameObject> solution3 = new Stack<GameObject>(); // 3 = Heats


    
    // Start is called before the first frame update
    void Start()
    {
        introSplash.GetComponent<Renderer>().enabled = true;
        endSplash.GetComponent<Renderer>().enabled = false;
        ToggleChildTextVisibility(endSplash, false);
        fieldCellsV2 = new List<Stack<GameObject>>()
        {
            field0V2, field1V2, field2V2, field3V2,
            field4V2, field5V2, field6V2, field7V2
        };

        solutionCells = new List<Stack<GameObject>>()
        {
            solution0, solution1, solution2, solution3
        };
        freeCells = new List<GameObject>(4)
        {
            new GameObject(), new GameObject(), new GameObject(), new GameObject()
        };
        StartCoroutine(disableIntroSplash());
        
        PlayGame();
       
    }

    private void ToggleChildTextVisibility(GameObject obj, Boolean isVisible)
    {
        foreach (var text in obj.GetComponentsInChildren<Text>())
        {
            text.GetComponent<Text>().enabled = isVisible;
        }
    }


    IEnumerator disableIntroSplash()
    {
        yield return new WaitForSeconds(2f);
        introSplash.GetComponent<Renderer>().enabled = false;
        ToggleChildTextVisibility(introSplash, false);
    }
    
    private void enableEndSplash()
    {
        endSplash.GetComponent<Renderer>().enabled = true;
        ToggleChildTextVisibility(endSplash, true);
    }

    // Update is called once per frame
    void Update()
    {
        // Check for winning condition
        if (isGameOver())
        {
            enableEndSplash();
        }
    }

    private bool isGameOver()
    {
        for (int i = 0; i < 4; i++)
        {
            if (solutionCells[i].Count < 13)
            {
                
                return false;
            }
        }
        Debug.Log("GAME OVER!");
        return true;
    }

    // Used to generate the suit-value pairs used to help updating the card sprites
    public static List<Tuple<Suits, int>> GenerateSuitValuePairsDeck()
    {
        List<Tuple<Suits, int>> generatedDeck = new List<Tuple<Suits, int>>();
        foreach (Suits s in Enum.GetValues(typeof(Suits)))
        {
            foreach (int i in Values)
            {
                Tuple<Suits, int> z = Tuple.Create(s, i);
                generatedDeck.Add(z);
            }
        }

        return generatedDeck;
    }
    
    // Generated the deck
    public List<GameObject> GenerateDeck()
    {
        List<GameObject> generatedDeck = new List<GameObject>();
        foreach (Suits suit in Enum.GetValues(typeof(Suits)))
        {
            foreach (int value in Values)
            {
                GameObject generatedCard = Instantiate(genericCardPrefab,
                    new Vector3(0, 0, -30f), Quaternion.identity);
                generatedCard.name = value + " of " + suit;
                generatedCard.AddComponent<Card>();
                generatedCard.GetComponent<Card>().suit = suit;
                generatedCard.GetComponent<Card>().val = value;
                generatedCard.GetComponent<Card>().column = 0; 
     
 
                generatedDeck.Add(generatedCard);
            }
        }

        return generatedDeck;
    }
    

    public void PlayGame()
    {
        unshuffledDeck = GenerateSuitValuePairsDeck();
        cardIndexMap = generateCardIndexMap(unshuffledDeck);
        List<GameObject> newDeckV2 = GenerateDeck();
        Shuffle(newDeckV2);
        StartCoroutine(DistributeCards(newDeckV2));
    }

    void Shuffle<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            int k = random.Next(n);
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }
    
    public Dictionary<string, int> generateCardIndexMap(List<Tuple<Suits, int>> deck)
    {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        int index = 0;
        foreach (Tuple<Suits, int> card in deck)
        {
            dictionary.Add(card.Item2.ToString() + card.Item1, index);
            index++;
        }
        return dictionary;
    }

    // Puts cards on the field
    IEnumerator DistributeCards(List<GameObject> deck)
    {
        // distribute first 
        for (int i = 0; i < 8; i++)
        {
            float zInitial = 0.3f;
            float yInitial = 0f;
            
            for (int j = 0; j < 6; j++)
            {
                yield return new WaitForSeconds(0.01f);
                fieldCellsV2[i].Push(deck.Last());
                deck.Last().GetComponent<Card>().column = i;
                deck.Last().transform.position = new Vector3(fieldCellsPos[i].transform.position.x, fieldCellsPos[i].transform.position.y - yInitial,
                    fieldCellsPos[i].transform.position.z - zInitial);

                if (i >= 4 && j == 5)
                {
                    deck.Last().GetComponent<Card>().isClickable = true;
                }
                deck.RemoveAt(deck.Count - 1);
                yInitial = yOffset * fieldCellsV2[i].Count;
                zInitial += zOffset;
                
                
            }
            
            // distribute remaining 4 cards
            if (i < 4)
            {
                yield return new WaitForSeconds(0.01f);
                deck.Last().transform.position = new Vector3(fieldCellsV2[i].Peek().transform.position.x, fieldCellsV2[i].Peek().transform.position.y - yOffset,
                    fieldCellsV2[i].Peek().transform.position.z - zOffset);
                deck.Last().GetComponent<Card>().column = i;
                deck.Last().GetComponent<Card>().isClickable = true;
                fieldCellsV2[i].Push(deck.Last());
                deck.RemoveAt(deck.Count - 1); 
            }
            
        }
    }

    public void HandleCardClick(GameObject clickedCard)
    {
        Card cardComponent = clickedCard.GetComponent<Card>();
        if (cardComponent.isClickable)
        {
            // check which freecells are avail (iterate)

            int columnToUpdate = 0;
            

            // Case: Card can be moved to Solution Pile
            if (cardComponent.suit == Suits.Clubs)
            {
                if (solutionCells[0].Count == 0 && cardComponent.val == 1)
                {
                    HandleSolutionPlacement(clickedCard, cardComponent, 0);
                }
                // Case: Current card value is 1 greater than card at top of solution pile
                else if (solutionCells[0].Count > 0 && 
                         cardComponent.val == solutionCells[0].Peek().GetComponent<Card>().val+1) // todo elseif can be refactored ^^
                {
                    HandleSolutionPlacement(clickedCard, cardComponent, 0);
                }
                // Case: Card can only be potentially moved to a "Free Cell"
                else if (!cardComponent.isInFreeCellSpace)
                {
                    MoveToFreeCell(clickedCard, cardComponent, columnToUpdate);
                }
            }
            else if (cardComponent.suit == Suits.Spades)
            {
                if (solutionCells[1].Count == 0 && cardComponent.val == 1)
                {
                    HandleSolutionPlacement(clickedCard, cardComponent, 1);
                }
                // Case: Current card value is 1 greater than card at top of solution pile
                else if (solutionCells[1].Count > 0 && 
                         cardComponent.val == solutionCells[1].Peek().GetComponent<Card>().val+1)
                {
                    HandleSolutionPlacement(clickedCard, cardComponent, 1);
                }
                // Case: Card can only be potentially moved to a "Free Cell"
                else if (!cardComponent.isInFreeCellSpace)
                {
                    MoveToFreeCell(clickedCard, cardComponent, columnToUpdate);
                }
            }
            else if (cardComponent.suit == Suits.Diamonds)
            {
                if (solutionCells[2].Count == 0 && cardComponent.val == 1)
                {
                    HandleSolutionPlacement(clickedCard, cardComponent, 2);
                }
                // Case: Current card value is 1 greater than card at top of solution pile
                else if (solutionCells[2].Count > 0 && 
                         cardComponent.val == solutionCells[2].Peek().GetComponent<Card>().val+1)
                {
                    HandleSolutionPlacement(clickedCard, cardComponent, 2);
                }
                // Case: Card can only be potentially moved to a "Free Cell"
                else if (!cardComponent.isInFreeCellSpace)
                {
                    MoveToFreeCell(clickedCard, cardComponent, columnToUpdate);
                }
            }
            else if (cardComponent.suit == Suits.Hearts)
            {
                if (solutionCells[3].Count == 0 && cardComponent.val == 1)
                {
                    HandleSolutionPlacement(clickedCard, cardComponent, 3);
                }
                // Case: Current card value is 1 greater than card at top of solution pile
                else if (solutionCells[3].Count > 0 && 
                         cardComponent.val == solutionCells[3].Peek().GetComponent<Card>().val+1)
                {
                    HandleSolutionPlacement(clickedCard, cardComponent, 3);
                }
                // Case: Card can only be potentially moved to a "Free Cell"
                else if (!cardComponent.isInFreeCellSpace)
                {
                    MoveToFreeCell(clickedCard, cardComponent, columnToUpdate);
                }
            }
        }
    }

    private void MoveToFreeCell(GameObject clickedCard, Card cardComponent, int columnToUpdate)
    {
        int index = 0;
        foreach (GameObject freeCell in freeCellsPos)
        {
            //Debug.Log(freeCell.GetComponent<FreeCell>().isFree);
            if (freeCell.GetComponent<FreeCell>().isFree)
            {
                clickedCard.transform.position = new Vector3(freeCell.transform.position.x,
                    freeCell.transform.position.y, freeCell.transform.position.z - 0.03f);
                freeCells[columnToUpdate] = clickedCard; // todo superfluous 
                freeCell.GetComponent<FreeCell>().isFree = false;
                clickedCard.GetComponent<Card>().isInFreeCellSpace = true; 
                clickedCard.GetComponent<Card>().isClickable = true;  
                columnToUpdate = clickedCard.GetComponent<Card>().column;
                fieldCellsV2[cardComponent.column].Pop();
                if (fieldCellsV2[cardComponent.column].Count > 0)
                {
                    GameObject lastCard = fieldCellsV2[cardComponent.column].Peek();
                    lastCard.GetComponent<Card>().isClickable = true;
                }
                
                cardComponent.column = index;
                
                break;
            }
            index++;
        }
    }
    
    private void HandleSolutionPlacement(GameObject clickedCard, Card cardComponent, int suitIndex)
    {
        if (solutionCells[suitIndex].Count() == 0) {
            clickedCard.transform.position = new Vector3(solutionCellsPos[suitIndex].transform.position.x,
                solutionCellsPos[suitIndex].transform.position.y, solutionCellsPos[suitIndex].transform.position.z - 0.3f);
        }
        else
        {
            Vector3 topMostCardPos = solutionCells[suitIndex].Peek().transform.position;
            clickedCard.transform.position = new Vector3(topMostCardPos.x,
                topMostCardPos.y-0.03f, topMostCardPos.z - 0.03f);
        }
        solutionCells[suitIndex].Push(clickedCard);
        cardComponent.isClickable = false;

        if (cardComponent.isInFreeCellSpace) // TODO MUST UPDATE COL IN FREE CELL SPACE
        {
            freeCellsPos[cardComponent.column].GetComponent<FreeCell>().isFree = true;
        }
        else
        {
            fieldCellsV2[cardComponent.column].Pop();
            if (fieldCellsV2[cardComponent.column].Count > 0)
            {
                GameObject lastCard = fieldCellsV2[cardComponent.column].Peek();
                lastCard.GetComponent<Card>().isClickable = true;
            }
           
            
        }
    }

    public void MoveCardAround(GameObject clickedCard)
    {
        if (clickedCard != null && clickedCard.GetComponent<Card>().isClickable)
        {
            clickedCard.transform.position = 
                Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 8));

        }
        
    }

    public void SnapCard(Vector3 mousePosition, GameObject currentHeldCard, Vector3 originalCardPos)
    {
        for (int i = 0; i < fieldCellsV2.Count; i++)
        {
            if (fieldCellsV2[i].Count > 0)
            {
                Vector3 cardPos = fieldCellsV2[i].Peek().transform.position;
                
                
                
                if (isMouseInRangeOfSnappablePile(mousePosition, cardPos))
                {
                    GameObject cardObjectAtTop = fieldCellsV2[i].Peek();
                    Card cardAtTop = cardObjectAtTop.GetComponent<Card>();
                    Card cardToPlace = currentHeldCard.GetComponent<Card>();
                    if (
                        ((IsBlack(cardAtTop) && IsRed(cardToPlace)) 
                        || (IsBlack(cardToPlace) && IsRed(cardAtTop)))
                        && 
                        cardAtTop.val - 1 == cardToPlace.val)
                    {
                        // Set top of stack in free cells pile to not be clickable
                        fieldCellsV2[i].Peek().GetComponent<Card>().isClickable = false;
                        fieldCellsV2[i].Push(currentHeldCard);
                        currentHeldCard.transform.position = new Vector3(cardPos.x,
                            cardPos.y - yOffset, cardPos.z - zOffset);
                        
                        // update prev DataStruct    
                        if (cardToPlace.isInFreeCellSpace)
                        {
                            freeCellsPos[cardToPlace.column].GetComponent<FreeCell>().isFree = true;
                        }
                        else
                        {
                            fieldCellsV2[cardToPlace.column].Pop();
                            fieldCellsV2[cardToPlace.column].Peek().GetComponent<Card>().isClickable = true;
                        }
                        // update column
                        cardToPlace.column = i;
                        // update bool
                        cardToPlace.isInFreeCellSpace = false;
                        return; 

                    }
                    
                }
            }
            // Case: FieldCell stack is empty
            else if (fieldCellsV2[i].Count == 0)
            {
                Vector3 cardPos = fieldCellsPos[i].transform.position;
                if (isMouseInRangeOfSnappablePile(mousePosition, cardPos))
                {
                    //Debug.Log("TARGET ACQUIRED - Empty stack");
                    fieldCellsV2[i].Push(currentHeldCard);
                    currentHeldCard.transform.position = new Vector3(cardPos.x,
                        cardPos.y, cardPos.z - zOffset);
                    Card cardToPlace = currentHeldCard.GetComponent<Card>();
                    // update prev DataStruct    
                    if (cardToPlace.isInFreeCellSpace)
                    {
                        freeCellsPos[cardToPlace.column].GetComponent<FreeCell>().isFree = true;
                    }
                    else
                    {
                        fieldCellsV2[cardToPlace.column].Pop();
                        fieldCellsV2[cardToPlace.column].Peek().GetComponent<Card>().isClickable = true;
                    }
                    // update column
                    cardToPlace.column = i;
                    // update bool
                    cardToPlace.isInFreeCellSpace = false;
                    return; 
                    
                }
            }
        }

       
        // Case: trying to snap to solution cell:
        for (int i = 0; i < solutionCells.Count; i++)
        {
            //Debug.Log("PLACING SOLUTION DRAG FORM");
            Vector3 placementPos = solutionCellsPos[i].transform.position;
            Card cardComponent = currentHeldCard.GetComponent<Card>();
            int suitVal;
            if (cardComponent.suit == Suits.Clubs) suitVal = 0;
            else if (cardComponent.suit == Suits.Spades) suitVal = 1;
            else if (cardComponent.suit == Suits.Diamonds) suitVal = 2;
            else suitVal = 3;
            
            if (isMouseInRangeOfSnappablePile(mousePosition, placementPos) && suitVal == i)
            {
                //Debug.Log("***IS SNAPPABLE***");
                if ((solutionCells[0].Count == 0 && cardComponent.val == 1)
                    || solutionCells[0].Count > 0 && 
                    cardComponent.val == solutionCells[0].Peek().GetComponent<Card>().val+1)
                {
                    //Debug.Log("***PLACING***");
                    HandleSolutionPlacement(currentHeldCard, cardComponent, suitVal);
                    return;
                }

            }
            
        }
        


        // else return to original spot.
        currentHeldCard.transform.position = originalCardPos;
    }

    private bool isMouseInRangeOfSnappablePile(Vector3 mousePosition, Vector3 cardPos)
    {
        return (mousePosition.x < cardPos.x + xPlacementOffset
                && mousePosition.x > cardPos.x - xPlacementOffset)
               && mousePosition.y < cardPos.y + yPlacementOffset
               && mousePosition.y > cardPos.y - yPlacementOffset;
    }

    private static bool IsRed(Card card)
    {
        return (card.suit == Suits.Hearts || card.suit == Suits.Diamonds);
    }

    private bool IsBlack(Card card)
    {
        return (card.suit == Suits.Clubs || card.suit == Suits.Spades);
    }
}
