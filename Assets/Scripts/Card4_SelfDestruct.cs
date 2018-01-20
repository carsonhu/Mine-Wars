using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Card4_SelfDestruct : PlayerCard {

    //wrest control from player menu
    //phase 1: pick a pawn
    //action: destroy all adjacent pawns next to the selected pawn
    //then restore control to player menu

    private CardPhase currentPhase;
    private bool isInCharge = false; //tells the player menu that this card is in charge now. We probably move this stuff to the hand manager?
    GridManager gm;
    GameObject pawn1;

    private enum CardPhase
    {
        phase1, action
    }

    public override bool applyEffect(GridManager gm, PlayerMenu pm) {
        // Need to highlight the spots of all the pawns the current player owns
        // Destroys everything adjacent to the selected pawn

        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D playerHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("UnitsLayer"));
        RaycastHit2D floorHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("Default"));
        if (currentPhase == CardPhase.phase1 && gm.IsIlluminated(floorHitter.transform.position) && playerHitter) //click a pawn
        {
            pawn1 = playerHitter.transform.gameObject;
            TogglePhase(CardPhase.action, gm, pm);
        }
        if (currentPhase == CardPhase.action) //highlight other pawns
        {
            // TODO: 
            // Check for UP DOWN RIGHT LEFT and remove the pawns there.
            // Remove rocks

            gm.tilePositions[playerHitter.transform.position].GetComponent<Tile>().AddBomb();
            gm.tilePositions[playerHitter.transform.position].GetComponent<Tile>().SetTile("plain");
            gm.RemovePawn(pawn1);
        }
        return true;
    }

    void RestorePM(PlayerMenu pm)
    {
        if (isInCharge)
        {
            isInCharge = false;
            if (!pm.enabled)
                pm.enabled = true;
        }
    }

    void TogglePhase(CardPhase phase, GridManager gm, PlayerMenu pm)
    {
        currentPhase = phase;
        if (phase == CardPhase.phase1) //highlight all allied units
        {
            Debug.Log(pm.GetPlayerSide());
            gm.IllumTeam(pm.GetPlayerSide(), Highlight.Plain);
        }
        else if (phase == CardPhase.action)
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
        if (!isInCharge && pm.enabled && (pm.playerSide == gam.playersTurn)) //currently just checks player menu. We probably add something to handmanager to make sure u cant click other cards
        {
            isInCharge = true;
            pm.enabled = false;
            TogglePhase(CardPhase.phase1, gm, pm);
        }
    }

    private void Start()    //THIS IS TEMPORARY SO THAT BOTH PLAYERS' BUTTONS ARENT JUST IN THE SAME PLACE
    {
        PlayerMenu pm = transform.parent.GetComponent<PlayerMenu>();
        if (pm.GetPlayerSide() == Team.Blue)
        {
            transform.position = transform.position + new Vector3(0, 7, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isInCharge)
        {
            GridManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
            PlayerMenu pm = transform.parent.GetComponent<PlayerMenu>();
            applyEffect(gm, pm);
        }
        if (Input.GetMouseButtonDown(1))
        {
            TogglePhase(CardPhase.action, GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>(), transform.parent.GetComponent<PlayerMenu>());
        }

    }
}
