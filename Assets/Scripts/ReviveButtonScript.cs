using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveButtonScript : MonoBehaviour
{
    private bool clickedRevive = false;

    private GameObject gameManager;
    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController");
    }

    public void OnReviveBtn()
    {
        clickedRevive = true;
    }

    public void Update()
    {
        if (clickedRevive && Input.GetMouseButtonDown(0))
        {
            gameManager.GetComponent<GameManager>().getPlayerMenu().GetComponent<PlayerMenu>().RevivePawn();
            clickedRevive = false;
        }
    }
}
