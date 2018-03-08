using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class NETTile : NetworkBehaviour {

    public bool hasBomb = false; //is there a bomb
    public Team affiliation; //Neutral, Blue, Red

    /// <summary>Highlight tile: Plain = green, Rock = red, other = white</summary>
    [ClientRpc]
    public void RpcHighlightTile(Highlight highlight)
    {
        if (highlight == Highlight.Plain)
            GetComponent<SpriteRenderer>().color = Color.green;
        else if (highlight == Highlight.Rock)
            GetComponent<SpriteRenderer>().color = Color.red;
        else
            GetComponent<SpriteRenderer>().color = Color.white;
    }

    /// <summary>Set tile to type specified by string. If tile has bomb, bomb will explode</summary>
    [ClientRpc]
    public void RpcSetTile(string type)
    {
        if (!hasBomb)
        {
            tag = type;
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + type);
            if (type == "redRock")
                affiliation = Team.Red;
            else if (type == "blueRock")
                affiliation = Team.Blue;
            else
                affiliation = Team.Neutral;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + "plain");
            tag = "plain";
            affiliation = Team.Neutral;
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>().ActivateBomb(transform.position);
            //blow 'em up
        }

    }

}
