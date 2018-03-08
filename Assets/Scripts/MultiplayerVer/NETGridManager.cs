using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;
public class NETGridManager : NetworkBehaviour {
    private int rows = 6;
    private int columns = 9;

    public Dictionary<Vector3, GameObject> tilePositions = new Dictionary<Vector3, GameObject>(); //map: pos -> tile
    public List<GameObject> redPawns = new List<GameObject>();
    public List<GameObject> bluePawns = new List<GameObject>();
    public Transform boardHolder; //A gameobject to hold all the tiles.

    List<Vector3> gridPositions = new List<Vector3>(); //we use this to get a list of empty grid positions
    List<Vector3> illuminatedPositions = new List<Vector3>(); //A list of all currently illuminated positions
    public int GetColumns() { return columns; }
    public int GetRows() { return rows; }


    void Start () {
		
	}

    void InitializeList()
    {
        gridPositions.Clear();
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                gridPositions.Add(new Vector3(i, j, 0f));
            }
        }
    }

    /// <summary>Initializes Tile Positions, a map from coordinate to tile located at that coord. </summary>
    void InitTilePositions()
    {
        foreach (Transform child in boardHolder)
        {
            tilePositions.Add(child.position, child.gameObject);
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
    }

    /// <summary>get a random pos. used for making the board.</summary>
    /// <returns>a random pos</returns>
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    /// <summary>Lay out X objects at random. used for making the board.</summary>
    /// <param name="tile">Tile to lay out.</param>
    /// <param name="count">Number of tiles to lay out.</param>
    [Command]
    void CmdLayoutObjectAtRandom(string tile, int count)
    {
        for (int i = 0; i < count; i++)
        {
   //         Debug.Log("laying out object");
            Vector3 randomPosition = RandomPosition();
            GameObject theTile = Resources.Load("Networking/NETTile") as GameObject;
            GameObject instantiated = Instantiate(theTile, randomPosition, Quaternion.identity) as GameObject;
            instantiated.transform.position = randomPosition;
            //      tilePositions.Add(theTile.transform.position, instantiated);
            instantiated.transform.SetParent(boardHolder);
            NetworkServer.Spawn(instantiated);
            instantiated.GetComponent<NETTile>().RpcSetTile(tile);
        }
    }

    public void SetupScene()
    {
        BoardSetup();
        InitializeList();
        CmdLayoutObjectAtRandom("plain", rows * columns - 25);
        CmdLayoutObjectAtRandom("redRock", 5);
        CmdLayoutObjectAtRandom("blueRock", 5);
        CmdLayoutObjectAtRandom("rock", 15);
        InitTilePositions();
    }

    // The following are PURE UTILITY FUNCTIONS and should NOT contain any logic pertaining to types of objects
    // and different colors. That sort of logic should go in HIGHER level functions that call
    // the following functions, passing in the desired color and position.
    // Highlights a single position with the given color.
    public void CmdIllumPosition(Vector3 pos, Highlight color)
    {
        tilePositions[pos].GetComponent<NETTile>().RpcHighlightTile(color);
        illuminatedPositions.Add(pos);
    }

    // Highlights tiles adjacent to pos with the given color.
    public void IllumAdjacent(Vector3 pos, Highlight color)
    {
        CmdIllumPosition(pos + Vector3.right, color);
        CmdIllumPosition(pos + Vector3.left, color);
        CmdIllumPosition(pos + Vector3.up, color);
        CmdIllumPosition(pos + Vector3.down, color);
    }

    /// <summary>Add pawn to a position.</summary>
    /// <param name="affiliation">Team pawn belongs to</param>
    /// <param name="position">Position to add pawn on</param>
    [Command]
    public void CmdAddPawn(Team affiliation, Vector3 position)
    {
        GameObject thePawn = Resources.Load("Networking/NETPawn") as GameObject;
        thePawn.transform.position = position;
        GameObject realPawn = Instantiate(thePawn) as GameObject;
        if (affiliation == Team.Blue)
            bluePawns.Add(realPawn);
        else
            redPawns.Add(realPawn);
        NetworkServer.Spawn(realPawn);
        realPawn.GetComponent<NETPawn>().RpcSwitchSprite(affiliation);
    }

    /// <summary>Add a bomb to a tile.</summary>
    /// <param name="floorHitter">Tile to add bomb to</param>
    public void CmdAddBomb(GameObject tile) //it now has a bomb!
    {
        tile.GetComponent<NETTile>().hasBomb = true;
    }

    public TileType GetTileType(Vector3 pos)
    {
        if (!tilePositions.ContainsKey(pos))
        {
            return TileType.NullTile;
        }
        if (tilePositions[pos].tag == "plain")
        {  // TODO: is there a better way to do this...?
            return TileType.Empty;
        }
        else if (tilePositions[pos].tag == "redRock")
        {
            return TileType.RedRock;
        }
        else if (tilePositions[pos].tag == "blueRock")
        {
            return TileType.BlueRock; //pawns can't be under the tiletype
        }
        else if (tilePositions[pos].tag == "rock")
        {
            return TileType.NeutralRock;
        }
        else
        {
            return TileType.NullTile;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
