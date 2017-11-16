using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Highlights
{
    Plain, Rock, None
}

public class Tile : MonoBehaviour {

   // public string type; //rock or not rock
    public bool hasBomb = false; //is there a bomb
    public Team affiliation; //Neutral, Blue, Red

    public void HighlightTile(Highlights highlight)
    {
        if (highlight == Highlights.Plain)
            GetComponent<SpriteRenderer>().color = Color.green;
        else if (highlight == Highlights.Rock)
            GetComponent<SpriteRenderer>().color = Color.red;
        else
            GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void SetTile(string type)
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
        }else
        {
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + "plain");
            tag = "plain";
            affiliation = Team.Neutral;
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>().ActivateBomb(transform.position);
            //blow 'em up
        }

    }

    public void RemoveBomb()
    {
        hasBomb = false;
    }

    public void AddBomb()
    {
        hasBomb = true;
    }
}
