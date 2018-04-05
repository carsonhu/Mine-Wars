using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenu : MonoBehaviour {

    public Team playerSide; //1 is red, 2 is blue
    GameManager gameManager;
    GameObject cursorObject; //this is the semi-transparent dude
    GridManager gridManager;
    DeckManager deck;
    private bool movementMode = false;
    private GameObject activeObject;
    public int actionPoints;
    bool turnPanelOpen = false;
    GameObject turnPanel;
    Vector3 OUTOFBOUNDS = new Vector3(9000, 9000, 0);
    //we'll have a variable referencing playerCards


    // Use this for initialization
    void Start() {
        actionPoints = 0;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        gridManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GridManager>();
        deck = GameObject.FindGameObjectWithTag("Deck").GetComponent<DeckManager>();
        Debug.Log("My team is: " + playerSide);
        cursorObject = Instantiate(Resources.Load<GameObject>("EmptySprite")) as GameObject;
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

    public Team GetPlayerSide() { return playerSide; }
    public Team GetNotPlayerSide() { return (playerSide == Team.Red) ? Team.Blue : Team.Red;}


    /// <summary>
    /// Set action points.
    /// </summary>
    /// <param name="a"></param>
    public void SetAP(int a)
    {
        actionPoints = a;
    }

    /// <summary>
    /// Destroy a rock.
    /// </summary>
    /// <param name="obj">the rock</param>
    public void DestroyObject(GameObject activeO, GameObject obj)
    {
        obj.GetComponent<Tile>().SetTile("plain");
        if(activeO != null)
            activeO.GetComponent<Pawn>().MoveTo(obj.transform.position);
        deck.DrawCard(GetComponentInChildren<HandManager>());
        actionPoints -= 2;
    }

    /// <summary>
    /// Revive Pawn
    /// </summary>
    public void RevivePawn()
    {
        if (actionPoints == 3)
        {
            if (GetPlayerSide() == Team.Red && gridManager.GetPawnCount(Team.Red) < 5 || GetPlayerSide() == Team.Blue && gridManager.GetPawnCount(Team.Blue) < 5)
            {
                TileType enemyRock;
                if (GetPlayerSide() == Team.Red)
                    enemyRock = TileType.BlueRock;
                else
                    enemyRock = TileType.RedRock;

                Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                RaycastHit2D playerHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("UnitsLayer"));
                RaycastHit2D floorHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("Default"));
                //Check the position to see if it's free (grid-wise and not-grid-wise).
                //If so, add a pawn on there!
                if (floorHitter && !playerHitter) //we hit a floor
                {
                    if (floorHitter.collider.tag == "plain")
                    {
                        if (!gridManager.IsAdjacentToObject(enemyRock, floorHitter.transform.position))
                        {
                            // Might probably consider highlighting the available positions in the future
                            gridManager.AddPawn(playerSide, floorHitter.transform.position);
                            Debug.Log("HEROES NEVER DIE!!!!!");
                        }
                    }
                }
            }
            else
            {
                Debug.Log("You have no one to rez yo! Stop trolling.");
            }
            actionPoints -= 3;
        }
        else
        {
            Debug.Log("You don't have enough action points! :(");
        }
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
        deck.DrawCard(GetComponentInChildren<HandManager>());
        actionPoints -= 3;
    }

    /// <summary>
    /// OnMouseOver method to be used when placing pawns, bombs, neutral bombs
    /// </summary>
    void CursorPosition()
    {
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D playerHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("UnitsLayer"));
        RaycastHit2D floorHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("Default"));
        if (gameManager.getPhase() == Phase.Pawns)
        {
            if(floorHitter && !playerHitter && canPlacePawn(floorHitter.transform.position)) //show a transparent dude there
            {
             //   Debug.Log(floorHitter.collider.tag);
                cursorObject.transform.position = floorHitter.transform.position;
                string pawnType = (playerSide == Team.Red) ? "redPawn" : "bluePawn";
                cursorObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/" + pawnType);
                cursorObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            }
            else
            {
                cursorObject.transform.position = OUTOFBOUNDS;
            }
        }
    }

    bool canPlacePawn(Vector3 pos)
    {
        return (gridManager.GetTileType(pos) == TileType.Empty) && ((GetPlayerSide() == Team.Red && !gridManager.IsAdjacentToObject(TileType.BlueRock, pos)) || (GetPlayerSide() == Team.Blue && !gridManager.IsAdjacentToObject(TileType.RedRock, pos)));
    }

    /// <summary>
    /// OnClick method to be used for placing pawns, bombs, and neutral bombs
    /// </summary>
    void ClickSelect()
    {
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D playerHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("UnitsLayer"));
        RaycastHit2D floorHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("Default"));
        if (gameManager.getPhase() == Phase.Pawns) //if you're placing a pawn.
        {
            
            //Check the position to see if it's free (grid-wise and not-grid-wise).
            //If so, add a pawn on there!
            if (floorHitter && !playerHitter) //we hit a floor
            {
                if ( canPlacePawn(floorHitter.transform.position)){
                    gridManager.AddPawn(playerSide, floorHitter.transform.position);
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
			gridManager.IllumPosition(pos, Highlight.Plain);
        }
		else if ((playerSide == Team.Blue && tile_type == TileType.RedRock) || 
				 (playerSide == Team.Red && tile_type == TileType.BlueRock) || 
				 (tile_type == TileType.NeutralRock)) {
			gridManager.IllumPosition(pos, Highlight.Rock);
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


    void DeactivateMovementMode()
    {
        gridManager.DellumPositions();
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
                if (gameManager.getPhase() == Phase.Game && !turnPanelOpen)
                     GameSelect();
                else
                    ClickSelect();
            }
        }

    }
}
