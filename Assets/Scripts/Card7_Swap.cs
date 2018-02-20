using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Card 5: Beguile. 
/// </summary>
public class Card7_Swap : PlayerCard
{

    //wrest control from player menu
    //phase 1: pick a tile.
    //phase 2: pick an enemy tile.
    //then restore control to player menu

    private CardPhase currentPhase;
    GameObject tile1;
    GameObject tile2;

    /// <summary> an enum for the card phases </summary>
    private enum CardPhase
    {
        phase1, phase2, action
    }

    public override void firstAction()
    {
        TogglePhase(CardPhase.phase1);
    }

    /// <summary> Click Behavior for each phase </summary>
    /// <param name="gm">grid manager</param>
    /// <param name="pm">player menu</param>
    /// <returns>theoretically you need to return whether it applied</returns>
    public override bool applyEffect() //click behavior
    {
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D playerHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("UnitsLayer"));
        RaycastHit2D floorHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("Default"));
        if (currentPhase == CardPhase.phase1 && gm.IsIlluminated(floorHitter.transform.position) && !playerHitter)
        {
            //select an empty tile    
        }
        return true;
    }

    void TogglePhase(CardPhase phase)
    {
        currentPhase = phase;
        if (phase == CardPhase.phase1) //highlight all allied rocks
        {
            Debug.Log(pm.GetPlayerSide());
            gm.IllumTeam(pm.GetPlayerSide(), Highlight.Plain); //highlight EVERYTHING
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
}