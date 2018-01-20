using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Represents a hand of cards

public class HandManager : MonoBehaviour {
    List<PlayerCard> hand;
    int[] cardPoses;
    // Use this for initialization
    void Start() {
        
    }

    public void insertCard(PlayerCard card)
    {
        this.hand.Add(card);
        card.transform.parent = this.transform;
    }

    void ArrangeCards()
    {
        for(int i = 0; i < hand.Count; i++)
        {
            
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
