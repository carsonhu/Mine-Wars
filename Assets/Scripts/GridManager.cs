using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
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

    /// <summary>Initializes GridPositions, which is used for making the board </summary>
    void InitializeList()
    {
        gridPositions.Clear();
        for(int i = 0; i < columns; i++)
        {
            for(int j = 0;  j < rows; j++)
            {
                gridPositions.Add(new Vector3(i, j, 0f));
            }
        }
    }

    /// <summary>Initializes Tile Positions, a map from coordinate to tile located at that coord. </summary>
    void InitTilePositions()
    {
        foreach(Transform child in boardHolder)
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
    void LayoutObjectAtRandom(string tile, int count)
    {
        for(int i = 0; i < count; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject theTile = Resources.Load("Tile") as GameObject;
            GameObject instantiated = Instantiate(theTile, randomPosition, Quaternion.identity) as GameObject;
            instantiated.transform.position = randomPosition;
            instantiated.GetComponent<Tile>().SetTile(tile);
      //      tilePositions.Add(theTile.transform.position, instantiated);
            instantiated.transform.SetParent(boardHolder);
        }
    }
    
    /// <summary> Check if a particular type of object is adjacent to the specified position.</summary>
    /// <param name="tag">TileType of object</param>
    /// <param name="pos">specified position</param>
    /// <returns></returns>
    public bool IsAdjacentToObject(TileType tag, Vector3 pos)
    {
        if (GetTileType(pos + Vector3.right) == tag || GetTileType(pos + Vector3.left) == tag || GetTileType(pos + Vector3.down) == tag || GetTileType(pos + Vector3.up) == tag)
            return true;
        else
            return false;
    }

    /// <summary> Helper function for exploding bombs. Helps explode a space.</summary>
    /// <param name="pos"></param>
    private void _ActivateBombHelper(Vector3 pos)
    {
        if (tilePositions.ContainsKey(pos))
        {
            Collider2D coll = Physics2D.OverlapCircle(pos, 0.1f, LayerMask.GetMask("UnitsLayer"));
            if (coll)
            {
                RemovePawn(coll.gameObject);
            }
            tilePositions[pos].GetComponent<Tile>().SetTile("plain"); //SetTile(plain) will trigger a bomb if the tile has one.
        }
    }

    /// <summary>Trigger a bomb by setting a tile, and tiles adjacent to it, to plains.</summary>
    /// <param name="pos">Position of tile to bomb</param>
    public void ActivateBomb(Vector3 pos)
    {
        if (tilePositions[pos].GetComponent<Tile>().hasBomb) {
            tilePositions[pos].GetComponent<Tile>().RemoveBomb();
            _ActivateBombHelper(pos + Vector3.right);
            _ActivateBombHelper(pos + Vector3.down);
            _ActivateBombHelper(pos + Vector3.left);
            _ActivateBombHelper(pos + Vector3.up);
      }
    }

    /// <summary>Returns count of red rocks, blue rocks, and neutral rocks. </summary>
    /// <returns>int array: [#red rocks, #blue rocks, #neutral rocks]</returns>
    public int[] CountRocks() //Not optimal but theres like 50 tiles so it's probably fine
    {
        int redSum = 0;
        int blueSum = 0;
        int neutralSum = 0;
        foreach(var tilePos in tilePositions.Keys)
        {
            if(GetTileType(tilePos) == TileType.RedRock)
                redSum++;
            if (GetTileType(tilePos) == TileType.BlueRock)
                blueSum++;
            if (GetTileType(tilePos) == TileType.NeutralRock)
                neutralSum++;
        }
        return new int[] { redSum, blueSum, neutralSum };
    }

    /// <summary>Add a bomb to a tile.</summary>
    /// <param name="floorHitter">Tile to add bomb to</param>
    public void AddBomb(GameObject floorHitter) //it now has a bomb!
    {
        floorHitter.GetComponent<Tile>().hasBomb = true;
    }

    /// <summary>Add pawn to a position.</summary>
    /// <param name="affiliation">Team pawn belongs to</param>
    /// <param name="position">Position to add pawn on</param>
    public void AddPawn(Team affiliation, Vector3 position)
    {
        GameObject thePawn = Resources.Load("Pawn") as GameObject;
        if (affiliation == Team.Blue)
            thePawn.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/bluePawn");
        else if(affiliation == Team.Red)
            thePawn.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/redPawn");
        else
            Debug.Log("Something went wrong!");
        thePawn.transform.position = position;
        GameObject realPawn = Instantiate(thePawn) as GameObject;
        if (affiliation == Team.Blue)
            bluePawns.Add(realPawn);
        else
            redPawns.Add(realPawn);
    }

    /// <summary>Gets the number of pawns for specified team.</summary>
    /// <param name="affiliation">Team pawn belongs to</param>
    public int GetPawnCount(Team affiliation)
    {
        GameObject thePawn = Resources.Load("Pawn") as GameObject;
        if (affiliation == Team.Blue)
            return bluePawns.Count;
        else if (affiliation == Team.Red)
            return redPawns.Count;
        else
        {
            Debug.Log("You have entered an invalid Team Affiliation!");
            return -1; // Need to change this part later.
        }
    }

    /// <summary>Destroy a pawn.</summary>
    /// <param name="obj">Pawn to destroy</param>
    public void RemovePawn(GameObject obj)
    {
        bluePawns.Remove(obj);
        redPawns.Remove(obj);
        GameObject explosion = Instantiate(Resources.Load("ToonExplosion/Prefabs/Explosion", typeof(GameObject)), obj.transform.position, Quaternion.identity) as GameObject;
        Destroy(explosion, .5f);
        Destroy(obj);
    }

    /// <summary>
    /// Primarily for use with Card 6: mislead. Changes a pawn's team.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="team"></param>
    public void AlterPawn(GameObject obj, Team team)
    {
        bluePawns.Remove(obj);
        redPawns.Remove(obj);
        if (team == Team.Red)
        {
            redPawns.Add(obj);
            obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/redPawn");
        }
        else
        {
            bluePawns.Add(obj);
            obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/bluePawn");
        }
    }

    /// <summary>
    /// Get the side the pawn is on
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public Team GetPawnSide(GameObject obj)
    {
        if (redPawns.Contains(obj))
            return Team.Red;
        if (bluePawns.Contains(obj))
            return Team.Blue;
        else
            return Team.Neutral;
    }



    public void SetupScene()
    {
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom("plain", rows*columns - 25);
        LayoutObjectAtRandom("redRock", 5);
        LayoutObjectAtRandom("blueRock", 5);
        LayoutObjectAtRandom("rock", 15);
        InitTilePositions();
    }


	// The following are PURE UTILITY FUNCTIONS and should NOT contain any logic pertaining to types of objects
	// and different colors. That sort of logic should go in HIGHER level functions that call
	// the following functions, passing in the desired color and position.
	// Highlights a single position with the given color.
	public void IllumPosition(Vector3 pos, Highlight color) {
		tilePositions[pos].GetComponent<Tile>().HighlightTile(color);
        illuminatedPositions.Add(pos);
	}

	// Highlights tiles adjacent to pos with the given color.
	public void IllumAdjacent(Vector3 pos, Highlight color) {
		IllumPosition(pos + Vector3.right, color);
		IllumPosition(pos + Vector3.left, color);
		IllumPosition(pos + Vector3.up, color);
		IllumPosition(pos + Vector3.down, color);
	}

    //For use of card 4: Highlights in target direction until hitting a rock or enemy player
    void IllumDirection(Vector3 pos, Vector3 direction, int length)
    {
        while (length > 0) //if tile is empty, higlight. if it's a rock, stop.
        {
            TileType tile = GetTileType(pos);
            if (tile != TileType.Empty) //if it's a rock or null we're done
                length = 0;
            else //if it's an empty tile
            {
                GameObject pawn = GetPawnAtPos(pos); //get the pawn at that position
                Highlight toHiglight = Highlight.Plain;
                if (pawn != null)  //we highlight this position, then we're done
                {
                    length = 1;
                    toHiglight = Highlight.Rock;
                }
                IllumPosition(pos, toHiglight);
                pos = pos + direction;
                length--;
            }
        }
    }

    public void IllumDirections(Vector3 pos, int length)
    {
        IllumDirection(pos + Vector3.up, Vector3.up, length);
        IllumDirection(pos + Vector3.left, Vector3.left, length);
        IllumDirection(pos + Vector3.right, Vector3.right, length);
        IllumDirection(pos + Vector3.down, Vector3.down, length);
    }



    // Highlights the given team with the given color.
    public void IllumTeam(Team t, Highlight color) {
		List<GameObject> team_list = (t == Team.Red) ? redPawns : bluePawns;
		foreach (GameObject pawn in team_list) {
			IllumPosition (pawn.transform.position, color);
		}
	}
    //Highlights list of positions as certain color
    public void IllumPositions(List<Vector3> poses, Highlight color)
    {
        foreach(Vector3 pos in poses)
        {
            IllumPosition(pos, color);
        }
    }

    public int IlluminatedPositionsCount()
    {
        return illuminatedPositions.Count;
    }

    public bool IsIlluminated(Vector3 pos) { return illuminatedPositions.Contains(pos); }

    public void DellumPositions()
    {
        foreach(Vector3 pos in illuminatedPositions)
        {
            tilePositions[pos].GetComponent<Tile>().HighlightTile(Highlight.None);
        }
        illuminatedPositions.Clear();
    }

    public GameObject GetPawnAtPos(Vector3 pos) //returns a pawn at a particular location, if there is one
    {
        RaycastHit2D playerHitter = Physics2D.Raycast(pos, Vector2.zero, 0f, LayerMask.GetMask("UnitsLayer"));
        if (playerHitter)
            return playerHitter.transform.gameObject;
        else
            return null;
    }

	public TileType GetTileType(Vector3 pos) {
		if (!tilePositions.ContainsKey (pos)) {
			return TileType.NullTile;
		}
		if (tilePositions [pos].tag == "plain") {  // TODO: is there a better way to do this...?
			return TileType.Empty;
		} else if (tilePositions [pos].tag == "redRock") {
			return TileType.RedRock;
		} else if (tilePositions [pos].tag == "blueRock") {
			return TileType.BlueRock; //pawns can't be under the tiletype
        }  else if (tilePositions [pos].tag == "rock") {
			return TileType.NeutralRock;	
		} else {
			return TileType.NullTile;
		}
	}

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
