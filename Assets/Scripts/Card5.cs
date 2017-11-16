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
    private bool isInCharge = false; //tells the player menu that this card is in charge now. We probably move this stuff to the hand manager?
    GridManager gm;
    GameObject pawn1;
    GameObject pawn2;

    private enum CardPhase
    {
        phase1, phase2, action
    }

    public override bool applyEffect(GridManager gm, PlayerMenu pm)
    {
        throw new NotImplementedException();
    }
    /*
public override bool applyEffect(GridManager gm, PlayerMenu pm) //click behavior
{
   Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
   RaycastHit2D playerHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("UnitsLayer"));
   RaycastHit2D floorHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("Default"));
   if (currentPhase == CardPhase.phase1 && gm.IsIlluminated(floorHitter.transform.position) && playerHitter) //click a pawn
   {
       pawn1 = playerHitter.transform.gameObject;
       TogglePhase(CardPhase.phase2, gm, pm);
   }
   if(currentPhase == CardPhase.phase2 && gm.IsIlluminated(floorHitter.transform.position) && playerHitter) //highlight other pawns
   {
       TogglePhase(CardPhase.action, gm, pm);
       pawn2 = playerHitter.transform.gameObject;
       Vector3 tempPosition = pawn2.transform.position;
       pawn2.transform.position = pawn1.transform.position;
       pawn1.transform.position = tempPosition;
   }
   return true;
}

void RestorePM(PlayerMenu pm)
{
   if (isInCharge)
   {
       isInCharge = false;
       pm.TogglePlayerMenu();
   }
}

void TogglePhase(CardPhase phase, GridManager gm, PlayerMenu pm)
{
   currentPhase = phase;
   if (phase == CardPhase.phase1) //highlight all allied units
   {
       Debug.Log(pm.GetPlayerSide());
       gm.IllumPositions(gm.GetPawnPositions(pm.GetPlayerSide()), Highlights.Plain);
   }else if(phase == CardPhase.phase2)
   {
       gm.DellumPositions();
       Team playerSide = (pm.GetPlayerSide() == Team.Red) ? Team.Blue : Team.Red;
       gm.IllumPositions(gm.GetPawnPositions(playerSide), Highlights.Plain);
   }else if(phase == CardPhase.action)
   {
       gm.DellumPositions();
       RestorePM(pm);
   }
}

private void OnMouseDown()
{//we disable behavior of playermenu, begin this update behavior
 //  GameObject GameController = GameObject.FindGameObjectWithTag("GameController");
   GameManager gam = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
   gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
   PlayerMenu pm = transform.parent.GetComponent<PlayerMenu>();
   if (!isInCharge && pm.IsEnabled() && (pm.playerSide == gam.playersTurn)) //currently just checks player menu
   {
       isInCharge = true;
       pm.TogglePlayerMenu();
       TogglePhase(CardPhase.phase1, gm, pm);
   }
}

private void Start()    //THIS IS TEMPORARY SO THAT BOTH PLAYERS' BUTTONS ARENT JUST IN THE SAME PLACE
{
   PlayerMenu pm = transform.parent.GetComponent<PlayerMenu>();
   if(pm.GetPlayerSide() == Team.Blue)
   {
       transform.position = transform.position + new Vector3(0, 7, 0);
   }
}

// Update is called once per frame
void Update () {


   if (Input.GetMouseButtonDown(0) && isInCharge)
   {
       GridManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
       PlayerMenu pm = transform.parent.GetComponent<PlayerMenu>();
       applyEffect(gm, pm);
   }
}*/
}
