using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card4_SelfDestruct : PlayerCard {

    public override bool applyEffect(GridManager gm, PlayerMenu pm) {
        // Need to highlight the spots of all the pawns the current player owns
        // Destroys everything adjacent to the selected pawn
        return true;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
