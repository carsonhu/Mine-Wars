﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Highlight
{
    Plain, Rock, None
}

public class Tile : MonoBehaviour {

   // public string type; //rock or not rock
    public bool hasBomb = false; //is there a bomb
    public Team affiliation; //Neutral, Blue, Red

    /// <summary>Highlight tile: Plain = green, Rock = red, other = white</summary>
    public void HighlightTile(Highlight highlight)
    {
        if (highlight == Highlight.Plain)
            GetComponent<SpriteRenderer>().color = Color.green;
        else if (highlight == Highlight.Rock)
            GetComponent<SpriteRenderer>().color = Color.red;
        else
            GetComponent<SpriteRenderer>().color = Color.white;
    }

    /// <summary>Set tile to type specified by string. If tile has bomb, bomb will explode</summary>
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

    /// <summary>
    /// Copies the specified tile
    /// </summary>
    /// <param name="tile"></param>
    public void CopyTile(GameObject tile)
    {
        
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
