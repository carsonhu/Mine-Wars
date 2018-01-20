using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Represents a hand of cards

public class HandManager : MonoBehaviour {
    List<PlayerCard> hand;
    int[] cardPoses;
    float[] xRange = { 0, 8 };
    float yPoses = -2.2f;
    // Use this for initialization
    void Start() {

    }

    public void insertCard(PlayerCard card)
    {
        this.hand.Add(card);
        card.transform.parent = this.transform;
        ArrangeCards();
    }

    void ArrangeCards()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            float currentXPos = (xRange[1] - xRange[0]) / hand.Count * i;
            hand[i].transform.position = new Vector3(currentXPos, yPoses);
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
