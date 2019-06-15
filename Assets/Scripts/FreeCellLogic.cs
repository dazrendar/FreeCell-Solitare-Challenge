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
    public Sprite[] cardAssignments;
    public GameObject genericCardPrefab;
    public float yOffset = 0.3f;
    public float zOffset = 0.02f;
    public static Dictionary<string, int> cardIndexMap;
    public static List<Tuple<Suits, int>> unshuffledDeck;
   
    
    // For aligning the cards
    public GameObject[] freeCellsPos;
    public GameObject[] solutionCellsPos;
    public GameObject[] fieldCellsPos;
    
    public List<Tuple<Suits, int>> freeCells;
    public List<Tuple<Suits, int>> solutionCells;
    public List<Tuple<Suits, int>>[] fieldCells;
    public List<List<GameObject>> cardObjects = new List<List<GameObject>>();

    private List<Tuple<Suits, int>> field0 = new List<Tuple<Suits, int>>();
    private List<Tuple<Suits, int>> field1 = new List<Tuple<Suits, int>>();
    private List<Tuple<Suits, int>> field2 = new List<Tuple<Suits, int>>();
    private List<Tuple<Suits, int>> field3 = new List<Tuple<Suits, int>>();
    private List<Tuple<Suits, int>> field4 = new List<Tuple<Suits, int>>();
    private List<Tuple<Suits, int>> field5 = new List<Tuple<Suits, int>>();
    private List<Tuple<Suits, int>> field6 = new List<Tuple<Suits, int>>();
    private List<Tuple<Suits, int>> field7 = new List<Tuple<Suits, int>>();


    
    // Start is called before the first frame update
    void Start()
    {
        fieldCells = new[] {field0, field1, field2, field3, field4, field5, field6, field7};
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

    public void PlayGame()
    {
        List<Tuple<Suits, int>> newDeck = GenerateDeck();
        unshuffledDeck = GenerateDeck();
        cardIndexMap = generateCardIndexMap(unshuffledDeck);
        Shuffle(newDeck);
        int count = 0;
        foreach (KeyValuePair<string, int> d in cardIndexMap)
        {
            //Debug.Log(d);
        }
        foreach (Tuple<Suits, int> t in newDeck)
        {
            count++;
            //Debug.Log(t);
        }
        Debug.Log("This deck has # of cards: " + count); // sanity check for 52 cards
        DistributeCards(newDeck);
        StartCoroutine(DealCards(newDeck));
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
