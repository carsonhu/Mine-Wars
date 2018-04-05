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

    [HideInInspector]
    public Phase currentPhase = Phase.Pawns;

    [HideInInspector]
    public List<GameObject> misledPawns = new List<GameObject>(); //for use with mislead
    public void AddMisledPawn(GameObject pawn) { misledPawns.Add(pawn); }

    GameObject p1Menu;
    GameObject p2Menu;

    DeckManager deck;

    //these variables are used for preparation stage
    int piecesLeft;
    int bombsLeft;
    int nBombsLeft;

    /// <summary>Decrement number of pawns. When it reaches 0, move on to bomb phase. </summary>
    public void decrementPieces()
    {
        piecesLeft--;
        if (piecesLeft == 0)
            SetPhase(Phase.Bombs);
    }

    /// <summary> Decrement number of bombs. When it reaches 0, move on to neutral bombs phase.</summary>
    public void decrementBombs()
    {
        bombsLeft--;
        if (bombsLeft == 0)
            SetPhase(Phase.NeutralBombs);
    }

    public void decrementNeutralBombs()
    {
        nBombsLeft--; //TODO/NOTE: THIS IS CURRENTLY BUGGED B/C OF DEBOUNCING: CLICKING WILL USUALLY LEAD TO 2 BOMBS BEING PLACED HERE. PROBABLY NOT A PROBLEM ONCE WE INCLUDE TURN-SEPARATING WINDOWS
        if (nBombsLeft == 0)
        {
            SetPhase(Phase.Game);
            Debug.Log("Game started");
        }
    }

    /// <summary> Sets current game phase. </summary>
    /// <param name="phase">Game phase to set to</param>
    void SetPhase(Phase phase)
    {
        if(currentPhase == Phase.NeutralBombs && phase == Phase.Game) //upon game start, each player draws 3 cards.
        {
            HandManager hm1 = p1Menu.transform.GetChild(0).GetComponent<HandManager>();
            HandManager hm2 = p2Menu.transform.GetChild(0).GetComponent<HandManager>();
            for(int i =0; i < 3; i++)
            {
                deck.DrawCard(hm1);
                deck.DrawCard(hm2);
            }
        }
        currentPhase = phase;
    }
    
    /// <summary>Get current game phase. </summary>
    /// <returns>Current Game Phase</returns>
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

        deck = GameObject.FindGameObjectWithTag("Deck").GetComponent<DeckManager>();

        InitGame();
    }

    /// <summary> Set up grid and start game </summary>
    void InitGame()
    {
        gridScript.SetupScene();
        StartGame();
    }

    /// <summary>Initializes # of pieces, # of bombs, # of neutral bombs</summary>
    void StartGame()
    {
        piecesLeft = 10; //players will place all their dudes
        bombsLeft = 2;
        nBombsLeft = 2;
    }

    /// <summary>
    /// Returns the Player Menu of the Current Player
    /// </summary>
    public GameObject getPlayerMenu()
    {
        if (playersTurn == Team.Red)
        {
            return p1Menu;
        }
        else
            return p2Menu;
    }

    /// <summary>
    /// Toggles player's turn
    /// </summary>
    public void TogglePlayerTurn()
    {
        playersTurn = (playersTurn == Team.Red) ? Team.Blue : Team.Red;
        int redAP = (playersTurn == Team.Red) ? 3 : 0;
        int blueAP = (playersTurn == Team.Blue) ? 3 : 0;

        HandManager hm1 = p1Menu.transform.GetChild(0).GetComponent<HandManager>();
        HandManager hm2 = p2Menu.transform.GetChild(0).GetComponent<HandManager>();

        p1Menu.GetComponent<PlayerMenu>().SetAP(redAP); //setting action points
        p2Menu.GetComponent<PlayerMenu>().SetAP(blueAP);

        bool redActive = (playersTurn == Team.Red) ? true : false; //make hand inactive upon turn change
        hm1.ToggleHand(redActive);
        hm2.ToggleHand(!redActive);

        if (currentPhase == Phase.Game) //drawing card on turn start
        {
            if (playersTurn == Team.Red)
                deck.DrawCard(hm1);
            else
                deck.DrawCard(hm2);
        }
        
        foreach(GameObject pawn in misledPawns) //handle pawns whose side has changed due to the [MISLEAD] card
        {
            if(pawn != null)
            {
                Team pawnSide = gridScript.GetPawnSide(pawn);
                Team pawnSideNew = (pawnSide == Team.Red) ? Team.Blue : Team.Red;
                gridScript.AlterPawn(pawn, pawnSideNew);
            }
        }
        misledPawns.Clear();

        //Victory check (rocks)
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

        //Victory check (pawns)
		if(currentPhase == Phase.Game && gridScript.redPawns.Count == 0)
        {
            Debug.Log("Blue wins!");
        }else if(currentPhase == Phase.Game && gridScript.bluePawns.Count == 0)
        {
            Debug.Log("Red wins!");
        }
	}
}
