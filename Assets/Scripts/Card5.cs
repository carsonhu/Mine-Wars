using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Card 5: Beguile. 
/// </summary>
public class Card5 : PlayerCard
{

    //wrest control from player menu
    //phase 1: pick a pawn.
    //phase 2: pick an enemy pawn.
    //then restore control to player menu

    private CardPhase currentPhase;
    GameObject pawn1;
    GameObject pawn2;

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
            // Arrow During Switching of Pawns
            //===============================================================
            Vector3 anchorPos = new Vector3(pawn1.transform.position.x, pawn1.transform.position.y, 0);
            Vector3 currentPos = new Vector3(pawn2.transform.position.x, pawn2.transform.position.y, 0);
            Vector3 midPointVector = (currentPos + anchorPos) / 2;
            GameObject Arrow = Instantiate(Resources.Load("Arrow", typeof(GameObject)), midPointVector, Quaternion.identity) as GameObject;
            Vector3 relative = currentPos - anchorPos;
            float maggy = relative.magnitude;
            Arrow.transform.localScale = new Vector3(maggy / 3, 1, 0);
            Quaternion rotationVector = Quaternion.LookRotation(relative);
            rotationVector.z = 0;
            rotationVector.w = 0;
            Arrow.transform.rotation = rotationVector;
            //===============================================================
            pawn2.transform.position = pawn1.transform.position;
            pawn1.transform.position = tempPosition;
            // Destroy Arrow
            Destroy(Arrow, 1.0f);
        }
        return true;
    }

    void TogglePhase(CardPhase phase)
    {
        currentPhase = phase;
        if (phase == CardPhase.phase1) //highlight all allied units
        {
            Debug.Log(pm.GetPlayerSide());
            gm.IllumTeam(pm.GetPlayerSide(), Highlight.Plain);
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