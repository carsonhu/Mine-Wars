using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Represents a hand of cards

public class HandManager : MonoBehaviour {
    List<GameObject> hand = new List<GameObject>();
    DeckManager deck;
    int maxHandSize = 8;
    float[] xRange = { 0, 8 }; //range of x-values to distribute cards along
    float yPoses = -2.2f; //yPosition for cards to be at.
    GameObject cardInCharge = null; //the currently active card.
    // Use this for initialization
    void Start() {
        deck = GameObject.FindGameObjectWithTag("Deck").GetComponent<DeckManager>();
    }

    /// <summary> returns true if hand is full</summary>
    public bool IsHandFull()
    {
        return hand.Count >= maxHandSize;
    }

    /// <summary> returns current hand size</summary>
    public int GetHandSize()
    {
        return hand.Count;
    }

    /// <summary> returns currently active card</summary>
    public GameObject GetCardInCharge()
    {        return cardInCharge;    }

    /// <summary> set the currently active card</summary>
    public void SetCardInCharge(GameObject card)
    {
        cardInCharge = card;
    }

    /// <summary>When the player's turn is over, call this to toggle visibility of player's hand.</summary>
    /// <param name="active">True: visible, False: invisible</param>
    public void ToggleHand(bool active) 
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(active);
        }
    }

    /// <summary>Insert a card into player's hand</summary>
    public void InsertCard(GameObject card)
    {
        card.SetActive(true);
        this.hand.Add(card);
        card.transform.parent = this.transform;
        ArrangeCards();
    }

    /// <summary>Discard a card from player's hand</summary>
    public void DiscardCard(GameObject card)
    {
        this.hand.Remove(card);
        deck.DiscardCard(card);
        ArrangeCards();
    }

    /// <summary>Arranges cards. Should be called whenever a card is inserted or removed.</summary>
    void ArrangeCards()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            float currentXPos = (xRange[1] - xRange[0]) / hand.Count * i; //distribute cards along the xRange
            hand[i].transform.position = new Vector3(currentXPos, yPoses);
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
