using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//to be used with the TurnMenu

public class TurnEndScript : MonoBehaviour {

    PlayerMenu playerMenu;
    GameObject floor;
    GameObject active;
	// Use this for initialization
	void Start () {
		
	}

    public void SetPlayerMenu(PlayerMenu p, GameObject activeObj, GameObject objectAtPos)
    {
        active = activeObj;
        playerMenu = p;
        floor = objectAtPos;
    }

    public void OnDestroyBtn()
    {
        if (playerMenu.actionPoints >= 2)
        {
            playerMenu.DestroyObject(active,floor);
        }

        playerMenu.ClosePanel();
    }

    public void OnCaptureBtn()
    {
        if (playerMenu.actionPoints >= 3)
        {
            playerMenu.CaptureObject(floor);
        }

        playerMenu.ClosePanel();
    }	
	// Update is called once per frame
	void Update () {
		
	}
}
