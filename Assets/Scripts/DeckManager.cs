using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents a deck of cards
public class DeckManager : MonoBehaviour {
    
	List<PlayerCard> deck;
	List<PlayerCard> graveyard;

    // Use this for initialization
    void Start () {
        // TODO: create copies of each card 
		deck.Add(new Card4_SelfDestruct());
		deck.Add (new Card5 ());
	}
	
    void drawCard(HandManager hm)
    {
		if (deck.Count > 0) {
			hm.insertCard (deck [0]);
			deck.RemoveAt (0);
		} else {
			// recreate deck from discarded cards
			recreateDeck();
		}
    }

	// this function should by called whatever class uses & discards a card
	public void discardCard(PlayerCard card) {  
		graveyard.Add (card);
	}

	void recreateDeck() {
		// shuffle graveyard, insertback into deck - randomly select/remove and Add to deck
		while (graveyard.Count > 0) {
			int index = Random.Range (0, graveyard.Count);
			deck.Add (graveyard [index]);
			graveyard.RemoveAt (index);
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
