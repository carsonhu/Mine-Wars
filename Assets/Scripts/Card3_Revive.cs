using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HEROES NEVER DIE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! :D
// MY SERVANTS NEVER DIE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! :D
// HUGE REZ FTW AYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY! :D

public class Card3_Revive : PlayerCard
{

    //wrest control from player menu
    //phase 1: Check pawn number is less than 5, pick a spot that is not adjacent to enemy rock.
    //action: Instantiate a pawn in the spot
    //then restore control to player menu

    // Revive a friendly pawn that has died this game, placing it anywhere on the map except for in spaces adjacent to enemy rocks

    private CardPhase currentPhase;
    private bool isInCharge = false; //tells the player menu that this card is in charge now. We probably move this stuff to the hand manager?
                                     //   GridManager gm;
    private bool usedRez = false;

    private enum CardPhase
    {
        phase1, Revived, NoRevive
    }

    public override void firstAction()
    {
        TogglePhase(CardPhase.phase1);
    }

    public override bool applyEffect()
    {
        if (pm.GetPlayerSide() == Team.Red && gm.GetPawnCount(Team.Red) < 5 || pm.GetPlayerSide() == Team.Blue && gm.GetPawnCount(Team.Blue) < 5)
        {
            TileType enemyRock;
            if (pm.GetPlayerSide() == Team.Red)
                enemyRock = TileType.BlueRock;
            else
                enemyRock = TileType.RedRock;
                
            Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D playerHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("UnitsLayer"));
            RaycastHit2D floorHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("Default"));
            //Check the position to see if it's free (grid-wise and not-grid-wise).
            //If so, add a pawn on there!
            Debug.Log("Getting ready for a huge rez :)");
            if (floorHitter && !playerHitter) //we hit a floor
            {
                if (floorHitter.collider.tag == "plain")
                {
                    if (!gm.IsAdjacentToObject(enemyRock, floorHitter.transform.position))
                    {
                        // Might probably consider highlighting the available positions in the future
                        gm.AddPawn(pm.playerSide, floorHitter.transform.position);
                        TogglePhase(CardPhase.Revived);
                    }
                }
            }
        }
        else
        {
            Debug.Log("You have no one to rez yo!");
            TogglePhase(CardPhase.NoRevive);
        }
        
        return true;
    }

    void TogglePhase(CardPhase phase)
    {
        currentPhase = phase;
        if (phase == CardPhase.phase1) //highlight all allied units
        {
            //Debug.Log(pm.GetPlayerSide());
           // gm.IllumTeam(pm.GetPlayerSide(), Highlight.Plain);
        }
        else if (phase == CardPhase.Revived)
        {
            //gm.DellumPositions();
            RestorePM();
            hm.DiscardCard(this.gameObject);
        }
        else if (phase == CardPhase.NoRevive)
        {
            RestorePM();
        }
    }

}