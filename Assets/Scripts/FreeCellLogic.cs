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
    public static List<GameObject> unshuffledDeckV2;
    public float xPlacementOffset = 0.65f;
    public float yPlacementOffset = 1f;
    public GameObject introSplash;
    public GameObject endSplash;
    
    
   
    
    // For aligning the cards
    public GameObject[] freeCellsPos;
    public GameObject[] solutionCellsPos;
    public GameObject[] fieldCellsPos;
    
    public List<GameObject> freeCells;
    public List<Tuple<Suits, int>>[] fieldCells;
    public List<Stack<GameObject>> fieldCellsV2;
    public List<Stack<GameObject>> solutionCells;
    public List<List<GameObject>> cardObjects = new List<List<GameObject>>();

    
    private List<Tuple<Suits, int>> field0 = new List<Tuple<Suits, int>>();
    private List<Tuple<Suits, int>> field1 = new List<Tuple<Suits, int>>();
    private List<Tuple<Suits, int>> field2 = new List<Tuple<Suits, int>>();
    private List<Tuple<Suits, int>> field3 = new List<Tuple<Suits, int>>();
    private List<Tuple<Suits, int>> field4 = new List<Tuple<Suits, int>>();
    private List<Tuple<Suits, int>> field5 = new List<Tuple<Suits, int>>();
    private List<Tuple<Suits, int>> field6 = new List<Tuple<Suits, int>>();
    private List<Tuple<Suits, int>> field7 = new List<Tuple<Suits, int>>();
    
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
        fieldCells = new[] {field0, field1, field2, field3, field4, field5, field6, field7}; // todo obsolete
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
        StartCoroutine(fadeIntroSplash());
        
        PlayGame();
       
    }

    private void ToggleChildTextVisibility(GameObject obj, Boolean isVisible)
    {
        foreach (var text in obj.GetComponentsInChildren<Text>())
        {
            text.GetComponent<Text>().enabled = isVisible;
        }
    }


    IEnumerator fadeIntroSplash()
    {
        yield return new WaitForSeconds(2f);
        introSplash.GetComponent<Renderer>().enabled = false;
        ToggleChildTextVisibility(introSplash, false);
    }
    
    private void toggleEndSplash()
    {
       
        endSplash.GetComponent<Renderer>().enabled = true;
        ToggleChildTextVisibility(endSplash, true);
    }

    // Update is called once per frame
    void Update()
    {
        // Winning condition
        if (isGameOver())
        {
            toggleEndSplash();
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

    public static List<Tuple<Suits, int>> GenerateDeck()
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
    
    
    public List<GameObject> GenerateDeckV2()
    {
        List<GameObject> generatedDeck = new List<GameObject>();
        foreach (Suits suit in Enum.GetValues(typeof(Suits)))
        {
            foreach (int value in Values)
            {
                //Card c = new Card();

                GameObject generatedCard = Instantiate(genericCardPrefab,
                    new Vector3(0, 0, -30f), Quaternion.identity); // todo fix
                generatedCard.name = value + " of " + suit;
                generatedCard.AddComponent<Card>();
                generatedCard.GetComponent<Card>().suit = suit;
                generatedCard.GetComponent<Card>().val = value;
                generatedCard.GetComponent<Card>().column = 0; // todo: must be updated in future !!!
                generatedCard.GetComponent<Card>().isCardDataUpdated = true;  // todo obsolete??
     
 
                generatedDeck.Add(generatedCard);
            }
        }

        return generatedDeck;
    }
    

    public void PlayGame()
    {
        List<Tuple<Suits, int>> newDeck = GenerateDeck();
        unshuffledDeck = GenerateDeck();

        List<GameObject> newDeckV2 = GenerateDeckV2();
        //unshuffledDeckV2 = GenerateDeckV2(); // todo potentially superfluous 
        Shuffle(newDeckV2);
        
        /*
        foreach (GameObject g in newDeckV2)
        {
            Card cardInfo = g.GetComponent<Card>();
            Debug.Log(cardInfo.suit + " " + cardInfo.val);
        }
        */
        
        
        
        // todo must have deck as card class...
        
        
        cardIndexMap = generateCardIndexMap(unshuffledDeck); // TODO REMOVE?
        foreach (KeyValuePair<string, int> d in cardIndexMap)
        {
            //Debug.Log(d);
        }
        
        //DistributeCards(newDeck);
        StartCoroutine(DistributeCardsV2(newDeckV2));

        //StartCoroutine(DealCards(newDeck));
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

    /*
    IEnumerator DealCards(List<Tuple<Suits, int>> deck)
    {
        for (int i = 0; i < 8; i++)
        {
            float zInitial = 0.3f;
            float yInitial = 0f;
            List<GameObject> fieldGameObj = new List<GameObject>();

            int counter = 1;
            
            foreach (Tuple<Suits, int> card in fieldCells[i])
            {
                yield return new WaitForSeconds(0.01f);
                GameObject generatedCard = Instantiate(genericCardPrefab,
                    new Vector3(fieldCellsPos[i].transform.position.x, fieldCellsPos[i].transform.position.y - yInitial,
                        fieldCellsPos[i].transform.position.z - zInitial),
                    Quaternion.identity);
                generatedCard.name = card.Item2 + " of " + card.Item1;
                generatedCard.AddComponent<Card>();
                generatedCard.GetComponent<Card>().suit = card.Item1;
                generatedCard.GetComponent<Card>().val = card.Item2;
                generatedCard.GetComponent<Card>().column = i;
                generatedCard.GetComponent<Card>().isCardDataUpdated = true;
                
                //generatedCard.GetComponent<Card>().sprite = generatedCard.GetComponent<SpriteRenderer>().sprite;
                
                if (counter == fieldCells[i].Count)
                {
                    generatedCard.GetComponent<Card>().isClickable = true;
                }
                counter++;
                yInitial += yOffset;
                zInitial += zOffset;
                
                fieldGameObj.Add(generatedCard);
            }

            cardObjects.Add(fieldGameObj);
        }
    }
*/
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

    /*
    private void DistributeCards(List<Tuple<Suits, int>> deck)
    {
        // distribute first 
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                fieldCells[i].Add(deck.Last());
                deck.RemoveAt(deck.Count - 1);
            }
        }
        
        // distribute remaining 4 cards
        for (int i = 0; i < 4; i++)
        {
            fieldCells[i].Add(deck.Last());
            deck.RemoveAt(deck.Count - 1);
        }
    }
    */
    
    /*
     * Attaches card objects to Stacks (per column), and updates each
     * card position as per initial "dealing"
     */
    IEnumerator DistributeCardsV2(List<GameObject> deck)
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

                // deck.Last().GetComponent<Card>().isClickable = true; // turn on for testing
                if (i >= 4 && j == 5)
                {
                    deck.Last().GetComponent<Card>().isClickable = true;
                }
                
                deck.RemoveAt(deck.Count - 1); // todo remove?
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
                deck.RemoveAt(deck.Count - 1); // todo remove?
            }
            
        }
    }

    public void HandleCardClick(GameObject clickedCard)
    {
        Card cardComponent = clickedCard.GetComponent<Card>();
        if (cardComponent.isClickable)
        {
            Debug.Log("Clicked a clickable card!");
            // check which freecells are avail (iterate) (reminder; these cannot stack!)

            int columnToUpdate = 0;
            

            // Case: Card can be moved to Solution Pile
            if (cardComponent.suit == Suits.Clubs)
            {
                if (solutionCells[0].Count == 0 && cardComponent.val == 1)
                {
                    Debug.Log("FOUND AN ACE!!");
                    HandleSolutionPlacement(clickedCard, cardComponent, 0);
                }
                // Case: Current card value is 1 greater than card at top of solution pile
                else if (solutionCells[0].Count > 0 && 
                         cardComponent.val == solutionCells[0].Peek().GetComponent<Card>().val+1) // todo elseif can be refactored ^^
                {
                    Debug.Log("HANDLING NEXT CARD TO SEND");
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
                    Debug.Log("FOUND AN ACE!!");
                    HandleSolutionPlacement(clickedCard, cardComponent, 1);
                }
                // Case: Current card value is 1 greater than card at top of solution pile
                else if (solutionCells[1].Count > 0 && 
                         cardComponent.val == solutionCells[1].Peek().GetComponent<Card>().val+1)
                {
                    Debug.Log("HANDLING NEXT CARD TO SEND");
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
                    Debug.Log("FOUND AN ACE!!");
                    HandleSolutionPlacement(clickedCard, cardComponent, 2);
                }
                // Case: Current card value is 1 greater than card at top of solution pile
                else if (solutionCells[2].Count > 0 && 
                         cardComponent.val == solutionCells[2].Peek().GetComponent<Card>().val+1)
                {
                    Debug.Log("HANDLING NEXT CARD TO SEND");
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
                    Debug.Log("FOUND AN ACE!!");
                    HandleSolutionPlacement(clickedCard, cardComponent, 3);
                }
                // Case: Current card value is 1 greater than card at top of solution pile
                else if (solutionCells[3].Count > 0 && 
                         cardComponent.val == solutionCells[3].Peek().GetComponent<Card>().val+1)
                {
                    Debug.Log("HANDLING NEXT CARD TO SEND");
                    HandleSolutionPlacement(clickedCard, cardComponent, 3);
                }
                // Case: Card can only be potentially moved to a "Free Cell"
                else if (!cardComponent.isInFreeCellSpace)
                {
                    MoveToFreeCell(clickedCard, cardComponent, columnToUpdate);
                }
            }
        }
        // Case: Card is in Free Cell space:
        else if (cardComponent.isClickable && cardComponent.isInFreeCellSpace)
        {
            // TODO IMPLEMENT
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
                clickedCard.GetComponent<Card>().isClickable = true;  // todo changed this.. must modify logic (in handler)!!!
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

        /*
        int indexToRemove = fieldCells[columnToUpdate].Count - 1; // remove
        fieldCells[columnToUpdate].RemoveAt(indexToRemove);  // remove
        */
        //fieldCells[columnToUpdate][indexToRemove-1] 

        
        // make next card up clickable
        //cardObjects[columnToUpdate].RemoveAt(indexToRemove);
        //GameObject cardToUpdate = cardObjects[columnToUpdate][indexToRemove - 1];
        //cardToUpdate.GetComponent<Card>().isClickable = true;
        
        
    }
    
    private void HandleSolutionPlacement(GameObject clickedCard, Card cardComponent, int suitIndex)
    {
        
        // update card position TODO !!!!!!! 
        Debug.Log(" ** solutionCells Count = " + solutionCells[suitIndex].Count());
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
           
            else
            {
                // todo do i need to add logic here? fieldSquare should be clickable now (no obstruction from card)...
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
                    Debug.Log("TARGET ACQUIRED");
                    Debug.Log("TARGET 23123123323123213ACQUIRED");
                    GameObject cardObjectAtTop = fieldCellsV2[i].Peek();
                    Card cardAtTop = cardObjectAtTop.GetComponent<Card>();
                    Card cardToPlace = currentHeldCard.GetComponent<Card>();
                    if (
                        ((IsBlack(cardAtTop) && IsRed(cardToPlace)) 
                        || (IsBlack(cardToPlace) && IsRed(cardAtTop)))
                        && 
                        cardAtTop.val - 1 == cardToPlace.val)
                    {
                        Debug.Log("TARGET cond met");
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
                    Debug.Log("TARGET ACQUIRED - Empty stack");
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
            Debug.Log("PLACING SOLUTION DRAG FORM");
            Vector3 placementPos = solutionCellsPos[i].transform.position;
            Card cardComponent = currentHeldCard.GetComponent<Card>();
            int suitVal;
            if (cardComponent.suit == Suits.Clubs) suitVal = 0;
            else if (cardComponent.suit == Suits.Spades) suitVal = 1;
            else if (cardComponent.suit == Suits.Diamonds) suitVal = 2;
            else suitVal = 3;
            
            if (isMouseInRangeOfSnappablePile(mousePosition, placementPos) && suitVal == i)
            {
                if ((solutionCells[0].Count == 0 && cardComponent.val == 1)
                    || solutionCells[0].Count > 0 && 
                    cardComponent.val == solutionCells[0].Peek().GetComponent<Card>().val+1)
                {
                    
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
