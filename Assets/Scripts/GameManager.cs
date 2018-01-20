using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum for phases of the game.
/// </summary>
public enum Phase { Pawns, Bombs, NeutralBombs, Game }

/// <summary>
/// Enum for affiliations (red, blue, or neither)
/// </summary>
public enum Team { Neutral, Red, Blue }

public enum TileType { Empty, RedRock, BlueRock, RedPawn, BluePawn, NeutralRock, NullTile }  // add to this if necessary

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;
    public GridManager gridScript;

    [HideInInspector]
    public Team playersTurn = Team.Red; //1 or 0


    public Phase currentPhase = Phase.Pawns; //

    GameObject p1Menu;
    GameObject p2Menu;

    //these variables are used for preparation stage
    int piecesLeft;
    int bombsLeft;
    int nBombsLeft;

    /// <summary>
    /// Decrement number of pawns. When it reaches 0, move on to bomb phase.
    /// </summary>
    public void decrementPieces()
    {
        piecesLeft--;
        if (piecesLeft == 0)
            currentPhase = Phase.Bombs;
    }

    /// <summary>
    /// Decrement number of bombs. When it reaches 0, move on to neutral bombs phase.
    /// </summary>
    public void decrementBombs()
    {
        bombsLeft--;
        if (bombsLeft == 0)
            currentPhase = Phase.NeutralBombs;
    }

    public void decrementNeutralBombs()
    {
        nBombsLeft--;
        if (nBombsLeft == 0)
        {
            currentPhase = Phase.Game;
            Debug.Log("Game started");
        }
    }

    public Phase getPhase()
    {
        return currentPhase;
    }

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        gridScript = GetComponent<GridManager>();

        GameObject playerMenu1 = Resources.Load("playerMenu") as GameObject;
        p1Menu = Instantiate(playerMenu1) as GameObject;
        p1Menu.GetComponent<PlayerMenu>().InitPlayer(Team.Red);

        GameObject playerMenu2 = Resources.Load("playerMenu") as GameObject;
        p2Menu = Instantiate(playerMenu2) as GameObject;
        p2Menu.GetComponent<PlayerMenu>().InitPlayer(Team.Blue);

        InitGame();
    }

    /// <summary>
    /// Set up grid and start game
    /// </summary>
    void InitGame()
    {
        gridScript.SetupScene();
        StartGame();
    }

/// <summary>
/// Initializes # of pieces, # of bombs, # of neutral bombs
/// </summary>
    void StartGame()
    {
        piecesLeft = 10; //players will place all their dudes
        bombsLeft = 2;
        nBombsLeft = 2;
    }

    /// <summary>
    /// Toggles player's turn
    /// </summary>
    public void TogglePlayerTurn()
    {
        playersTurn = (playersTurn == Team.Red) ? Team.Blue : Team.Red;
        int redAP = (playersTurn == Team.Red) ? 3 : 0;
        int blueAP = (playersTurn == Team.Blue) ? 3 : 0;

        p1Menu.GetComponent<PlayerMenu>().SetAP(redAP);
        p2Menu.GetComponent<PlayerMenu>().SetAP(blueAP);

        int[] rocks = gridScript.CountRocks(); //rocks[0] is red, rocks[1] is blue
        if (rocks[0] == 0 && rocks[1] != 0)
            Debug.Log("Blue wins!");
        else if (rocks[0] != 0 && rocks[1] == 0)
            Debug.Log("Red wins!");
        else if (rocks[0] == 0 && rocks[1] == 0)
            Debug.Log("Nobody wins!");
    }


    // Update is called once per frame
    void Update () {
		if(currentPhase == Phase.Game && gridScript.redPawns.Count == 0)
        {
            Debug.Log("Blue wins!");
        }else if(currentPhase == Phase.Game && gridScript.bluePawns.Count == 0)
        {
            Debug.Log("Red wins!");
        }
	}
}
