using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    private int rows = 6;
    private int columns = 9;

    public Dictionary<Vector3, GameObject> tilePositions = new Dictionary<Vector3, GameObject>();
    public List<GameObject> redPawns = new List<GameObject>();
    public List<GameObject> bluePawns = new List<GameObject>();
  //  private List<GameObject> tiles = new List<GameObject>();
    public Sprite[] sprites; //list of sprites. Sprites[0] is plains, sprites[1] is rocks
    public Transform boardHolder;

    private List<Vector3> gridPositions = new List<Vector3>(); //we use this to get a list of empty grid positions
    List<Vector3> illuminatedPositions = new List<Vector3>();
    public int GetColumns() { return columns; }
    public int GetRows() { return rows; }

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

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

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
    
    private void _ActivateBombHelper(Vector3 pos)
    {
        if (tilePositions.ContainsKey(pos))
        {
            Collider2D coll = Physics2D.OverlapCircle(pos, 0.1f, LayerMask.GetMask("UnitsLayer"));
            if (coll)
            {
                RemovePawn(coll.gameObject);
            }
            tilePositions[pos].GetComponent<Tile>().SetTile("plain");
      //      tilePositions[pos].GetComponent<Tile>().RemoveBomb();
        }
    }

    public void ActivateBomb(Vector3 pos) //set tile, and tiles adjacent to it, to plain.
    {
        if (tilePositions[pos].GetComponent<Tile>().hasBomb) {
            tilePositions[pos].GetComponent<Tile>().RemoveBomb();
            _ActivateBombHelper(pos + Vector3.right);
            _ActivateBombHelper(pos + Vector3.down);
            _ActivateBombHelper(pos + Vector3.left);
            _ActivateBombHelper(pos + Vector3.up);
      }
        //then proceed to check adjacent tiles
    }

    public void AddBomb(GameObject floorHitter) //it now has a bomb!
    {
        floorHitter.GetComponent<Tile>().hasBomb = true;
    }

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

    public void RemovePawn(GameObject obj)
    {
        bluePawns.Remove(obj);
        redPawns.Remove(obj);
        Destroy(obj);
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

    public bool IsIlluminated(Vector3 pos) { return illuminatedPositions.Contains(pos); }

    public void DellumPositions()
    {
        foreach(Vector3 pos in illuminatedPositions)
        {
            tilePositions[pos].GetComponent<Tile>().HighlightTile(Highlight.None);
        }
        illuminatedPositions.Clear();
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
			return TileType.BlueRock;
		} else if (tilePositions [pos].tag == "redPawn") {  // TODO: this may be wrong.
			return TileType.RedPawn;
		} else if (tilePositions [pos].tag == "bluePawn") {
			return TileType.BluePawn;
		} else if (tilePositions [pos].tag == "rock") {
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
