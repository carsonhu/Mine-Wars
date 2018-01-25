using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerCard : MonoBehaviour {

    protected GridManager gm;
    protected PlayerMenu pm;
    protected HandManager hm;

    // Applies the effect of the current card
    // Returns true if the effect was applied, false if it failed (ie - revive with no dead pawns)
    public abstract bool applyEffect();
    public abstract void firstAction(); //first phase to move to

    public void setSprite(string name) // Sets the sprite of the card to the resource file described by name
    {
        // This is the default implementation
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + name);
    }

    /// <summary> Restore player menu. Disable current card's active status. </summary>
    protected void RestorePM()
    {
        if (hm.GetCardInCharge() != null)
        {
            hm.SetCardInCharge(null);
            if (!pm.enabled)
                pm.enabled = true;
        }
    }

    public void OnMouseOver()
    {
        // If your mouse hovers over the GameObject with the script attached, output this message
    //    Debug.Log("Mouse is over GameObject.");
        // TODO: Need to set card display area to this current card once we figure out how display works
    }

    protected void OnMouseDown()
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
            firstAction();
        }
    }

    public void Update()
    {
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
