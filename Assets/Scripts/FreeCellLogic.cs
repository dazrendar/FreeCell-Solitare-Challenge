using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

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
    
    
   
    
    // For aligning the cards
    public GameObject[] freeCellsPos;
    public GameObject[] solutionCellsPos;
    public GameObject[] fieldCellsPos;
    
    public List<Tuple<Suits, int>> freeCells;
    public List<Tuple<Suits, int>> solutionCells;
    public List<Tuple<Suits, int>>[] fieldCells;
    public List<Stack<GameObject>> fieldCellsV2;
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


    
    // Start is called before the first frame update
    void Start()
    {
        fieldCells = new[] {field0, field1, field2, field3, field4, field5, field6, field7};
        fieldCellsV2 = new List<Stack<GameObject>>()
        {field0V2, field1V2, field2V2, field3V2,
            field4V2,
            field5V2,
            field6V2,
            field7V2
        };
        PlayGame();
       
    }

    // Update is called once per frame
    void Update()
    {
        
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
                    Vector3.zero, Quaternion.identity);
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
        
        foreach (GameObject g in newDeckV2)
        {
            Card cardInfo = g.GetComponent<Card>();
            Debug.Log(cardInfo.suit + " " + cardInfo.val);
        }
        
        
        
        // todo must have deck as card class...
        
        
        cardIndexMap = generateCardIndexMap(unshuffledDeck); // TODO REMOVE?
        foreach (KeyValuePair<string, int> d in cardIndexMap)
        {
            //Debug.Log(d);
        }
        
        //DistributeCards(newDeck);
        DistributeCardsV2(newDeckV2);

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
    
    private void DistributeCardsV2(List<GameObject> deck)
    {
        // distribute first 
        for (int i = 0; i < 8; i++)
        {
            float zInitial = 0.3f;
            float yInitial = 0f;
            
            for (int j = 0; j < 6; j++)
            {
                fieldCellsV2[i].Push(deck.Last());
                deck.Last().transform.position = new Vector3(fieldCellsPos[i].transform.position.x, fieldCellsPos[i].transform.position.y - yInitial,
                    fieldCellsPos[i].transform.position.z - zInitial);

                
                if (i >= 4 && j == 5)
                {
                    deck.Last().GetComponent<Card>().isClickable = true;
                }
                
                deck.RemoveAt(deck.Count - 1); // todo remove?
                yInitial = yOffset * fieldCellsV2[i].Count;
                zInitial += zOffset;
            }
        }
        
        // distribute remaining 4 cards
        for (int i = 0; i < 4; i++)
        {
            deck.Last().transform.position = new Vector3(fieldCellsV2[i].Peek().transform.position.x, fieldCellsV2[i].Peek().transform.position.y - yOffset,
                fieldCellsV2[i].Peek().transform.position.z - zOffset);
            deck.Last().GetComponent<Card>().isClickable = true;
            fieldCellsV2[i].Push(deck.Last());
            deck.RemoveAt(deck.Count - 1); // todo remove?
        }
    }

    public void TryToMoveCardToEmptySlot(GameObject clickedCard)
    {
        if (clickedCard.GetComponent<Card>().isClickable)
        {
            Debug.Log("Clicked a clickable card!");
            // check which freecells are avail (iterate) (reminder; these cannot stack!)

            int columnToUpdate = 0;
            
            foreach (GameObject freeCell in freeCellsPos)
            {
                //Debug.Log(freeCell.GetComponent<FreeCell>().isFree);
                if (freeCell.GetComponent<FreeCell>().isFree)
                {
                    clickedCard.transform.position = new Vector3(freeCell.transform.position.x, freeCell.transform.position.y, freeCell.transform.position.z - 0.3f);
                    freeCell.GetComponent<FreeCell>().isFree = false;
                    clickedCard.GetComponent<Card>().isInFreeCellSpace = true;
                    clickedCard.GetComponent<Card>().isClickable = false;
                    columnToUpdate = clickedCard.GetComponent<Card>().column; 
                    break;
                }
            }

            int indexToRemove = fieldCells[columnToUpdate].Count - 1;
            fieldCells[columnToUpdate].RemoveAt(indexToRemove);
            //fieldCells[columnToUpdate][indexToRemove-1] 

            // make next card up clickable
            cardObjects[columnToUpdate].RemoveAt(indexToRemove);
            GameObject cardToUpdate = cardObjects[columnToUpdate][indexToRemove - 1];
            cardToUpdate.GetComponent<Card>().isClickable = true;
        }
        else
        {
            Debug.Log("cannot click this card!");
        }
    }

    public void MoveCardAround(GameObject clickedCard)
    {
        clickedCard.transform.position = 
            Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 8));
        
    }
}
