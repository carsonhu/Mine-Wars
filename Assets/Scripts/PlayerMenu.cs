﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenu : MonoBehaviour {

    public Team playerSide; //1 is red, 2 is blue
    GameManager gameManager;
    GridManager gridManager;
    private bool movementMode = false;
    private GameObject activeObject;
    public int actionPoints;
    bool turnPanelOpen = false;
    GameObject turnPanel;
    //we'll have a variable referencing playerCards


	// Use this for initialization
	void Start () {
        actionPoints = 0;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        gridManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
	}
	
    /// <summary>
    /// Initialize a pawn. Give them a side too.
    /// </summary>
    /// <param name="side"></param>
    public void InitPlayer(Team side)
    {
        playerSide = side;
        Debug.Log("Player side:" + playerSide);
    }

    /// <summary>
    /// Set action points.
    /// </summary>
    /// <param name="a"></param>
    public void SetAP(int a)
    {
        actionPoints = a;
    }

    /// <summary>
    /// Destory a rock.
    /// </summary>
    /// <param name="obj">the rock</param>
    public void DestroyObject(GameObject activeO, GameObject obj)
    {
        obj.GetComponent<Tile>().SetTile("plain");
        if(activeO != null)
            activeO.GetComponent<Pawn>().MoveTo(obj.transform.position);
        actionPoints -= 2;
    }

    /// <summary>
    /// Capture a rock.
    /// </summary>
    /// <param name="obj">the rock</param>
    public void CaptureObject(GameObject obj)
    {
        if (playerSide == Team.Red)
            obj.GetComponent<Tile>().SetTile("redRock");
        else
            obj.GetComponent<Tile>().SetTile("blueRock");
      //  activeObject.GetComponent<Pawn>().MoveTo(obj.transform.position);
        actionPoints -= 3;
    }

    void CursorPosition()
    {

    }

    /// <summary>
    /// OnClick method to be used for placing pawns, bombs, and neutral bombs
    /// </summary>
    void ClickSelect()
    {
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
     //   RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f); //hit: checks everything (perhaps should just make it check players)
        RaycastHit2D playerHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("UnitsLayer"));
        RaycastHit2D floorHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("Default"));
        if (gameManager.getPhase() == Phase.Pawns) //if you're placing a pawn
        {
            
            //Check the position to see if it's free (grid-wise and not-grid-wise).
            //If so, add a pawn on there!
            if (floorHitter && !playerHitter) //we hit a floor
            {
                if(floorHitter.collider.tag == "plain")
                {
                    gridManager.AddPawn(playerSide,floorHitter.transform.position);
                    gameManager.decrementPieces();
                    gameManager.TogglePlayerTurn();
                }
            }
        }else if(gameManager.getPhase() == Phase.Bombs) //now we place bombs under rocks
        {
            if (floorHitter)
            {
                if ((playerSide == Team.Red && floorHitter.collider.tag == "redRock") || (playerSide == Team.Blue && floorHitter.collider.tag == "blueRock"))
                {
                    gridManager.AddBomb(floorHitter.collider.gameObject);
                    gameManager.decrementBombs();
                    gameManager.TogglePlayerTurn();
                    Debug.Log("Bomb placed");
                }
            }
        }else if(gameManager.getPhase() == Phase.NeutralBombs)
        {
            if (floorHitter)
            {
                if(floorHitter.collider.tag == "rock")
                {
                    gridManager.AddBomb(floorHitter.collider.gameObject);
                    gameManager.decrementNeutralBombs();
                    gameManager.TogglePlayerTurn();
                    Debug.Log("Neutral Bomb placed");
                }
            }
        }else if(gameManager.getPhase() == Phase.Game)
        {

        }
    }

    /// <summary>
    /// Illuminates a position: green if plain, red if rock.
    /// </summary>
    /// <param name="pos"></param>
    void IllumPosition(Vector3 pos)
    {
    //    Debug.Log("Position: " + pos);
        Collider2D coll = Physics2D.OverlapCircle(pos, 0.1f, LayerMask.GetMask("UnitsLayer"));
		TileType tile_type = gridManager.GetTileType (pos);
		if (tile_type == TileType.NullTile || coll) return;
		if (tile_type == TileType.Empty) {
			gridManager.IllumPosition(pos, new Color(0, 1, 0, 1));
        }
		else if ((playerSide == Team.Blue && tile_type == TileType.RedRock) || 
				 (playerSide == Team.Red && tile_type == TileType.BlueRock) || 
				 (tile_type == TileType.NeutralRock)) {
			gridManager.IllumPosition(pos, new Color(1, 0, 0, 1));
        } else {
    		// Debug.Log("Tag: " + gridManager.tilePositions[pos].tag);
        }
    }

    /// <summary>
    /// Illuminates positions around a pawn
    /// </summary>
    /// <param name="pos"></param>
    void IllumPositions(Vector3 pos)
    {
        IllumPosition(pos + Vector3.right);
        IllumPosition(pos + Vector3.left);
        IllumPosition(pos + Vector3.up);
        IllumPosition(pos + Vector3.down);
    }

    /// <summary>
    /// Activate Movement Mode for an object: Now clicks check to see if object can move somewhere.
    /// </summary>
    /// <param name="obj"></param>
    void ActivateMovementMode(GameObject obj)
    {
        Debug.Log("activating movement mode");
        IllumPositions(obj.transform.position);
        activeObject = obj;
        movementMode = true;
    }

    /// <summary>
    /// Set position's color back to normal
    /// </summary>
    /// <param name="pos"></param>
    void DellumPosition(Vector3 pos)
    {
		if (gridManager.GetTileType(pos) == TileType.NullTile)
            return;
		gridManager.IllumPosition(pos, new Color(1,1,1,1));
    }

    void DeactivateMovementMode()
    {
        if (activeObject != null)
        {
            Debug.Log("Deactivating Movement mode");
            DellumPosition(activeObject.transform.position + Vector3.up);
            DellumPosition(activeObject.transform.position + Vector3.right);
            DellumPosition(activeObject.transform.position + Vector3.down);
            DellumPosition(activeObject.transform.position + Vector3.left);
        }
        activeObject = null;
        movementMode = false;
    }

    void OpenPanel(GameObject activeObj, GameObject obj)
    {
        turnPanelOpen = true;
        GameObject panel = Resources.Load("TurnMenu") as GameObject;
        turnPanel = Instantiate(panel);
        turnPanel.GetComponent<TurnEndScript>().SetPlayerMenu(this, activeObj, obj);
        turnPanel.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
        turnPanel.transform.localPosition = obj.transform.position;
    }
    
    public void ClosePanel()
    {
        Destroy(turnPanel);
        turnPanelOpen = false;
        DeactivateMovementMode();
    }


    void GameSelect() //longer and for stuff past the preparation stage
    {
        Debug.Log("yes");
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D playerHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("UnitsLayer"));
        RaycastHit2D floorHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("Default"));

        if (!movementMode) //if we haven't selected a unit
        {
            string spriteName = (playerSide == Team.Red) ? "redPawn" : "bluePawn";

            if (playerHitter && playerHitter.transform.gameObject.GetComponent<SpriteRenderer>().sprite.name == spriteName)
            {
                ActivateMovementMode(playerHitter.transform.gameObject);
            }
        }
        else
        {
            //if we select a dude:
            if (floorHitter)
            {
                if (floorHitter.transform.gameObject.GetComponent<SpriteRenderer>().color == Color.red) //it's a rock!
                {
                    if (actionPoints >= 2)
                        OpenPanel(activeObject, floorHitter.transform.gameObject);
         //           else
          //              DeactivateMovementMode();
                }
                else if (floorHitter.transform.gameObject.GetComponent<SpriteRenderer>().color == Color.green)
                {
                    if(actionPoints >= 1)
                    {
                        activeObject.GetComponent<Pawn>().MoveTo(floorHitter.transform.position);
                        actionPoints -= 1;
                        Debug.Log("moved to plain");
                    }
     //               DeactivateMovementMode();
                }
                else
                {
                    Debug.Log("Not red or green");
                }
                DeactivateMovementMode();
                //getPositions(position of playerHitter)
            }
        }
        //first, we want to have behavior for: capturing a rock, destroying a rock, and moving dudes
    }


    void CheckCursor()
    {

    }

    // Update is called once per frame
    void Update () {
     //   Debug.Log(playerSide);  
        if(gameManager.playersTurn == playerSide)
        {
            CursorPosition();
            //if playerCards isn't active: if it's active, we let playerCards deal with the clickstuff since there's some unique cases
            if (Input.GetMouseButtonDown(0))
            {
                if (gameManager.getPhase() == Phase.Game)
                    GameSelect();
                else
                    ClickSelect();
            }
        }

    }
}