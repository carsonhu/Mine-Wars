using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents a deck of cards
public class DeckManager : MonoBehaviour {
    List<PlayerCard> deck;
    // Use this for initialization
    void Start () {
		
	}
	
    void drawCard(HandManager hm)
    {
        hm.insertCard(deck[0]);
        deck.RemoveAt(0);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
