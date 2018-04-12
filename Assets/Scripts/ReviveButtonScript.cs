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
        PlayerMenu pm = gameManager.GetComponent<GameManager>().getPlayerMenu().GetComponent<PlayerMenu>();
        GridManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        if (pm.GetPlayerSide() == Team.Red && gm.GetPawnCount(Team.Red) < 5 || pm.GetPlayerSide() == Team.Blue && gm.GetPawnCount(Team.Blue) < 5)
        {
            if (pm.actionPoints == 3)
            {
                clickedRevive = true;
            }
            else
            {
                Debug.Log("You don't have enough action points!!!!!");
            }
        }
        else
        {
            Debug.Log("You have no one to rez yo! Stop trolling.");
        }
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
