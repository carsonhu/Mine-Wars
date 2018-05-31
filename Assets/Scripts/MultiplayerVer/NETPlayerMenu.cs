using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NETPlayerMenu : NetworkBehaviour {

    //[SyncVar(hook = "OnPlayerSide")]

    [SyncVar]
    public Team playerSide;
    NETGameManager gameManager;
    NETGridManager gridManager;

    [SyncVar]
    public bool movementMode = false;
    private GameObject activeObject;

    [SyncVar]
    public int actionPoints;
    bool turnPanelOpen = false;
    GameObject turnPanel;
    Vector3 OUTOFBOUNDS = new Vector3(9000, 9000, 0);

    // Use this for initialization
    void Start () {

    }

    public override void OnStartClient()
    {
        actionPoints = 0;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<NETGameManager>();
        gridManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<NETGridManager>();
     //   gameManager.gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
    }

    /// <summary>
    /// Initialize a pawn. Give them a side too.
    /// </summary>
    /// <param name="side"></param>
    [Server]
    public void ServerInitPlayer(Team side)
    {
        playerSide = side;
        Debug.Log("Player side:" + playerSide);

    }

    void OnPlayerSide(Team side)
    {
        playerSide = side;
    }

    public Team GetPlayerSide() { return playerSide; }
    public Team GetNotPlayerSide() { return (playerSide == Team.Red) ? Team.Blue : Team.Red; }

    /// <summary>
    /// Set action points.
    /// </summary>
    /// <param name="a"></param>
    public void SetAP(int a)
    {
        actionPoints = a;
    }

    /* bool canPlacePawn(Vector3 pos)
     {
         return (gridManager.GetTileType(pos) == TileType.Empty) && ((GetPlayerSide() == Team.Red && !gridManager.IsAdjacentToObject(TileType.BlueRock, pos)) || (GetPlayerSide() == Team.Blue && !gridManager.IsAdjacentToObject(TileType.RedRock, pos)));
     }*/

    /// <summary>
    /// Illuminates a position: green if plain, red if rock.
    /// </summary>
    /// <param name="pos"></param>
    void IllumPosition(Vector3 pos)
    {
        //    Debug.Log("Position: " + pos);
        Collider2D coll = Physics2D.OverlapCircle(pos, 0.1f, LayerMask.GetMask("UnitsLayer"));
        TileType tile_type = gridManager.GetTileType(pos);
        if (tile_type == TileType.NullTile || coll) return;
        if (tile_type == TileType.Empty)
        {
            gridManager.CmdIllumPosition(pos, Highlight.Plain);
        }
        else if ((playerSide == Team.Blue && tile_type == TileType.RedRock) ||
                 (playerSide == Team.Red && tile_type == TileType.BlueRock) ||
                 (tile_type == TileType.NeutralRock))
        {
            gridManager.CmdIllumPosition(pos, Highlight.Rock);
        }
        else
        {
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

    void ClickSelect(Vector2 rayPos)
    {
        RaycastHit2D playerHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("UnitsLayer"));
        RaycastHit2D floorHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("Default"));
        
        if (gameManager.getPhase() == Phase.Pawns) //if you're placing a pawn.
        {
            //Check the position to see if it's free (grid-wise and not-grid-wise).
            //If so, add a pawn on there!
            if (floorHitter && !playerHitter) //we hit a floor
            {
          //      if (canPlacePawn(floorHitter.transform.position))
          //      {
                    gridManager.CmdAddPawn(playerSide, floorHitter.transform.position);
                    gameManager.decrementPieces();
                    gameManager.RpcTogglePlayerTurn();
          //      }
            }
        }
        else if (gameManager.getPhase() == Phase.Bombs) //now we place bombs under rocks
        {
            if (floorHitter)
            {
                if ((playerSide == Team.Red && floorHitter.collider.tag == "redRock") || (playerSide == Team.Blue && floorHitter.collider.tag == "blueRock"))
                {
                    gridManager.CmdAddBomb(floorHitter.collider.gameObject);
                    gameManager.decrementBombs();
                    gameManager.RpcTogglePlayerTurn();
                    Debug.Log("Bomb placed");
                }
            }
        }
        else if (gameManager.getPhase() == Phase.NeutralBombs)
        {
            if (floorHitter)
            {
                if (floorHitter.collider.tag == "rock")
                {
                    gridManager.CmdAddBomb(floorHitter.collider.gameObject);
                    gameManager.decrementNeutralBombs();
                    gameManager.RpcTogglePlayerTurn();
                    Debug.Log("Neutral Bomb placed");
                }
            }
        }
    }

    void GameSelect(Vector2 rayPos)
    {
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
            if (floorHitter)
            {

            }
        }



     }

    // Update is called once per frame
    void Update()
    {

        if (!isLocalPlayer)
        {
            return;
        }

        if(GetPlayerSide() == Team.Blue)
        {
            if(gridManager == null)
            {
                gridManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<NETGridManager>();
            }
            if(gameManager == null)
                gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<NETGameManager>();
            gridManager.gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
            gridManager.CmdAddPawn(Team.Blue, new Vector3(0, 0, 0));
        }

 //       if (gameManager.playersTurn == playerSide)
   //     {
            if (Input.GetMouseButtonDown(0))
            {
               Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                if (gameManager.getPhase() == Phase.Game)
                    GameSelect(rayPos);
                else
                    ClickSelect(rayPos);
            }
     //   }
    }
}
