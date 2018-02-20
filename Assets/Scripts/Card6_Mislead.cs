using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card6_Mislead : PlayerCard
{

    //phase 1: choose a pawn. it is now yours! (for now...)

    private enum CardPhase
    {
        phase1, action
    }
    private CardPhase currentPhase;


    public override bool applyEffect()
    {
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D playerHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("UnitsLayer"));
        RaycastHit2D floorHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("Default"));
        if (currentPhase == CardPhase.phase1 && gm.IsIlluminated(floorHitter.transform.position) && playerHitter) //click a pawn
        {
            GameObject pawn = playerHitter.transform.gameObject;
            gm.AlterPawn(pawn, pm.GetPlayerSide()); //change the pawn's side
            gm.GetComponentInParent<GameManager>().AddMisledPawn(pawn); //game manager now takes care of misled pawns
            TogglePhase(CardPhase.action);
        }
        return true;
    }
    public override void firstAction()
    {
        TogglePhase(CardPhase.phase1);
    }

    void TogglePhase(CardPhase phase)
    {
        currentPhase = phase;
        if (phase == CardPhase.phase1) //highlight all allied units
        {
            Debug.Log(pm.GetNotPlayerSide());
            gm.IllumTeam(pm.GetNotPlayerSide(), Highlight.Plain);
        }
        else if (phase == CardPhase.action)
        {
            gm.DellumPositions();
            RestorePM();
            hm.DiscardCard(this.gameObject);
        }
    }
}
