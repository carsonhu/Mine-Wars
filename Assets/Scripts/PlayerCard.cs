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
}
