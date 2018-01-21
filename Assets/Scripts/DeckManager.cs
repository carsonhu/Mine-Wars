using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents a deck of cards
public class DeckManager : MonoBehaviour {
    
	List<GameObject> deck = new List<GameObject>();
	List<GameObject> graveyard = new List<GameObject>();
    Vector3 OUTOFBOUNDS = new Vector3(9000, 9000, 0);
    // Use this for initialization
    void Start () {
        // TODO: create copies of each card 
        GameObject card4 = Instantiate(Resources.Load("Cards/SelfDestructCard") as GameObject) as GameObject;
        TransferCard(card4, true);
        for (int i = 0; i < 10; i++)
        {
            GameObject card5 = Instantiate(Resources.Load("Cards/BeguileCard") as GameObject) as GameObject;
            TransferCard(card5, true);
        }
	//	deck.Add(new Card4_SelfDestruct());
	//	deck.Add (new Card5 ());
	}

    /// <summary> Transfers a card to deck or graveyard</summary>
    /// <param name="card">Card to transfer</param>
    /// <param name="toDeck">TRUE: DECK. FALSE: GRAVEYARD</param>
    void TransferCard(GameObject card, bool toDeck)
    {
        card.transform.parent = this.transform;
        card.transform.position = OUTOFBOUNDS;
        card.SetActive(false);
        if (toDeck)
            deck.Add(card);
        else
            graveyard.Add(card);
    }
	
    public void DrawCard(HandManager hm)
    {
		if (deck.Count > 0) {
            if (!hm.IsHandFull())
            {
                hm.InsertCard(deck[0]);
                deck.RemoveAt(0);
            }else //if hand is full, move it to graveyard
            {
                graveyard.Add(deck[0]);
                deck.RemoveAt(0);
            }
		} else { //if deck is empty, move graveyard to deck
			RecreateDeck();
		}
    }

    /// <summary>this function should be called whatever class uses & discards a card </summary>
    /// <param name="card">card to discard</param>
    public void DiscardCard(GameObject card) {
        TransferCard(card, false);
	}

	void RecreateDeck() {
        // shuffle graveyard, insertback into deck - randomly select/remove and Add to deck
        List<GameObject> temp = graveyard;
        graveyard = deck;
        deck = temp; //I sure hope this is how pointers in C# work

        Miscellaneous.Shuffle<GameObject>(deck);
        Debug.Log("Deck has been reshuffled");
        /*
		while (graveyard.Count > 0) {
			int index = Random.Range (0, graveyard.Count);
			deck.Add (graveyard [index]);
			graveyard.RemoveAt (index);
		}*/
	}

	// Update is called once per frame
	void Update () {
		
	}
}
