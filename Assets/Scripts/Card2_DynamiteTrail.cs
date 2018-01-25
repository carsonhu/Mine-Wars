using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card2_DynamiteTrail : PlayerCard {

    //phase 1: choose a pawn
    //phase 2: choose a direction
    //then act!

    private CardPhase currentPhase;
    GameObject pawn1;
    int range = 3;


    private enum CardPhase
    {
        phase1, phase2, action
    }

    public override void firstAction()
    {
        TogglePhase(CardPhase.phase1);
    }

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
        if (currentPhase == CardPhase.phase2 && gm.IsIlluminated(floorHitter.transform.position))
        {
            //Start Coroutine to throw the dynamite
            Vector3 direction = (floorHitter.transform.position - pawn1.transform.position).normalized;
            GameObject dynamite = Instantiate(Resources.Load("Dynamite") as GameObject) as GameObject;
            dynamite.transform.position = pawn1.transform.position;
            ThrowDynamite(direction, dynamite);
            TogglePhase(CardPhase.action);
        }
        return true;
    }

    void ThrowDynamite(Vector3 dir, GameObject dynamite) //throw dynamite in target direction. If there's a pawn, destroy it and stop. Dynamite is coded as Pawn for now.
    //note: look at smoothmovements in roguelike to do this
    {
        Vector3 pos = dynamite.transform.position;
        Vector3 nextPos = pos + dir;
        //if highlighted, go. If not, stop.
      //  while (true)
      //  {
            if (gm.IsIlluminated(nextPos))
            {
                dynamite.GetComponent<Pawn>().MoveTo(nextPos);
            }
            pos = nextPos;
            nextPos = pos + dir;
     //   }
    }

    void TogglePhase(CardPhase phase)
    {
        currentPhase = phase;
        if (phase == CardPhase.phase1) //highlight all allied units
        {
            Debug.Log(pm.GetPlayerSide());
            gm.IllumTeam(pm.GetPlayerSide(), Highlight.Plain);
        }
        else if (phase == CardPhase.phase2) //we want to highlight in various directions
        {
            gm.DellumPositions();
            gm.IllumDirections(pawn1.transform.position, range);
        }
        else if (phase == CardPhase.action)
        {
            gm.DellumPositions();
            RestorePM();
            hm.DiscardCard(this.gameObject);
        }
    }
}
