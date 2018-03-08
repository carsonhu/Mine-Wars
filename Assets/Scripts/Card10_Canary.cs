using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card10_Canary : PlayerCard
{

    //phase 1: select pawn
    //phase 2: select new bomb location if bomb found. Else, skip to action phase
    //action phase: do yo stuff


    enum CardPhase
    {
        phase1, phase2, action
    }
    private CardPhase currentPhase;

    public override bool applyEffect()
    {
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D playerHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("UnitsLayer"));
        RaycastHit2D floorHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("Default"));
        if (currentPhase == CardPhase.phase1 && gm.IsIlluminated(floorHitter.transform.position) && playerHitter)
        {

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
        if (phase == CardPhase.phase1)
        {
            gm.IllumTeam(pm.GetPlayerSide(), Highlight.Plain);
        }
        else if (phase == CardPhase.phase2)
        {

        }
    }
}