using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {

    private GameObject gameManager;
	// Use this for initialization
	void Start () {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
    }

    public void OnEndTurnBtn()
    {
        gameManager.GetComponent<GameManager>().TogglePlayerTurn();
    }
    
}
