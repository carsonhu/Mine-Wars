using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Represents a hand of cards

public class HandManager : MonoBehaviour {
    List<PlayerCard> hand;
    // Use this for initialization
    void Start() {

    }

    public void insertCard(PlayerCard card)
    {
        this.hand.Add(card);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
