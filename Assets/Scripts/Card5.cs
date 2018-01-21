using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Card 5: Beguile. 
/// </summary>
public class Card5 : PlayerCard {

    //wrest control from player menu
    //phase 1: pick a pawn.
    //phase 2: pick an enemy pawn.
    //then restore control to player menu

    private CardPhase currentPhase;
    GameObject pawn1;
    GameObject pawn2;

    /// <summary>
    /// an enum for the card phases
    /// </summary>
    private enum CardPhase
    {
        phase1, phase2, action
    }

    /// <summary>
    /// Click Behavior for each phase
    /// </summary>
    /// <param name="gm">grid manager</param>
    /// <param name="pm">player menu</param>
    /// <returns>theoretically you need to return whether it applied</returns>
    public override bool applyEffect() //click behavior
    {
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D playerHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("UnitsLayer"));
        RaycastHit2D floorHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("Default"));
        if (currentPhase == CardPhase.phase1 && gm.IsIlluminated(floorHitter.transform.position) && playerHitter) //click a pawn
        {
            pawn1 = playerHitter.transform.gameObject;
            TogglePhase(CardPhase.phase2);
        }
        if (currentPhase == CardPhase.phase2 && gm.IsIlluminated(floorHitter.transform.position) && playerHitter) //highlight other pawns
        {
            TogglePhase(CardPhase.action);
            pawn2 = playerHitter.transform.gameObject;
            Vector3 tempPosition = pawn2.transform.position;
            pawn2.transform.position = pawn1.transform.position;
            pawn1.transform.position = tempPosition;
        }
        return true;
    }

    void TogglePhase(CardPhase phase)
    {
        currentPhase = phase;
        if (phase == CardPhase.phase1) //highlight all allied units
        {
            Debug.Log(pm.GetPlayerSide());
            gm.IllumTeam(pm.GetPlayerSide(),Highlight.Plain);
        }
        else if (phase == CardPhase.phase2)
        {
            gm.DellumPositions();
            Team playerSide = (pm.GetPlayerSide() == Team.Red) ? Team.Blue : Team.Red;
            gm.IllumTeam(playerSide, Highlight.Plain);
        }
        else if (phase == CardPhase.action)
        {
            gm.DellumPositions();
            RestorePM();
            hm.DiscardCard(this.gameObject);
        }
    }

    private void OnMouseDown()
    {//we disable behavior of playermenu, begin this update behavior
     //  GameObject GameController = GameObject.FindGameObjectWithTag("GameController");
        GameManager gam = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        pm = transform.parent.parent.GetComponent<PlayerMenu>();
        hm = transform.parent.GetComponent<HandManager>();
        if (gam.getPhase() == Phase.Game && pm.enabled && (pm.playerSide == gam.playersTurn) && hm.GetCardInCharge() == null) //currently just checks player menu. We probably add something to handmanager to make sure u cant click other cards
        {
            pm.enabled = false;
            hm.SetCardInCharge(this.gameObject);
            TogglePhase(CardPhase.phase1);
        }
    }

private void Start()    //THIS IS TEMPORARY SO THAT BOTH PLAYERS' BUTTONS ARENT JUST IN THE SAME PLACE
{
 /*
        PlayerMenu pm = transform.parent.parent.GetComponent<PlayerMenu>();
        if (pm.GetPlayerSide() == Team.Blue)
        {
            transform.position = transform.position + new Vector3(0, 7, 0);
        }*/
}

// Update is called once per frame
void Update () {
        if (GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().getPhase() == Phase.Game)
        {
            if (transform.parent.GetComponent<HandManager>().GetCardInCharge() == this.gameObject) //if this is in charge, gm,pm, and hm, have surely been set
            {
                if (Input.GetMouseButtonDown(0))
                {
                    applyEffect();
                }
                if (Input.GetMouseButtonDown(1)) //if we want to cancel, we don't want to go to togglePhase b/c we want to keep the card
                {
                    gm.DellumPositions();
                    RestorePM();
                }
            }
        }
}
}
