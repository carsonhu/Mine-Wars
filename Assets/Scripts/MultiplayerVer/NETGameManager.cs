using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NETGameManager : NetworkBehaviour {
    public static NETGameManager instance = null;
    public NETGridManager gridManager;

    [HideInInspector]
    public Team playersTurn = Team.Red; //1 or 0

    [HideInInspector]
    public Phase currentPhase = Phase.Pawns;

    [SyncVar]
    GameObject p1Menu;
    [SyncVar]
    GameObject p2Menu;

    //these variables are used for preparation stage
    int piecesLeft;
    int bombsLeft;
    int nBombsLeft;

    [SyncVar(hook = "RpcAwakeManager")]
    bool hasStarted = false;
    NetworkManager networkManager;

    /// <summary>Get current game phase. </summary>
    /// <returns>Current Game Phase</returns>
    public Phase getPhase()
    {
        return currentPhase;
    }

    void SetPhase(Phase phase)
    {
        /*
        if (currentPhase == Phase.NeutralBombs && phase == Phase.Game) //upon game start, each player draws 3 cards.
        {
            HandManager hm1 = p1Menu.transform.GetChild(0).GetComponent<HandManager>();
            HandManager hm2 = p2Menu.transform.GetChild(0).GetComponent<HandManager>();
            for (int i = 0; i < 3; i++)
            {
                deck.DrawCard(hm1);
                deck.DrawCard(hm2);
            }
        }*/
        currentPhase = phase;
    }

    /// <summary>Decrement number of pawns. When it reaches 0, move on to bomb phase. </summary>
    public void decrementPieces()
    {
        piecesLeft--;
        Debug.Log(piecesLeft);
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

    // Use this for initialization
    void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
    }

    [ClientRpc]
    void RpcAwakeManager(bool hasStarted)
    {
        //    GameObject playerMenu1 = Resources.Load("Networking/NETPlayerMenu") as GameObject;
        //    p1Menu = Instantiate(playerMenu1) as GameObject;
        //    GameObject playerMenu2 = Resources.Load("Networking/NETPlayerMenu") as GameObject;
        //    p2Menu = Instantiate(playerMenu1) as GameObject;
        if (hasStarted)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerMenu");
            Debug.Log("Length: " + players.Length);


            p1Menu = players[0];
        //    p1Menu.GetComponent<NETPlayerMenu>().ServerInitPlayer(Team.Red);
            p2Menu = players[1];
        //    p2Menu.GetComponent<NETPlayerMenu>().ServerInitPlayer(Team.Blue);
            gridManager = GetComponent<NETGridManager>();

            InitGame();
        }
    }



    void InitGame()
    {
        gridManager.SetupScene();
        StartGame();
        //
    }

    void StartGame()
    {
        piecesLeft = 10; //players will place all their dudes
        bombsLeft = 2;
        nBombsLeft = 2;
    }

    /// <summary>
    /// Toggles player's turn
    /// </summary>
    public void RpcTogglePlayerTurn()
    {
        playersTurn = (playersTurn == Team.Red) ? Team.Blue : Team.Red;
        int redAP = (playersTurn == Team.Red) ? 3 : 0;
        int blueAP = (playersTurn == Team.Blue) ? 3 : 0;
    }
    /*
    [ClientRpc]
    public void RpcTogglePlayerTurn()
    {

    }*/


        // Update is called once per frame
        void Update () {
        var numConnected = networkManager.numPlayers;
  //      Debug.Log(numConnected);
        if (numConnected == 2 && !hasStarted)
        {
            hasStarted = true;
            Debug.Log("Enough players!");
        }else if(numConnected != 2 && !hasStarted)
        {
            Debug.Log("Not enough players");
        }
	}
}
