using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Card8_GuidingRope : PlayerCard {

    //move allied pawn 6 places or enemy 2
    //phase 1: pick a pawn
    //phase 2: pick a spot to move to
    //then restore control to player menu

    private CardPhase currentPhase;
    GameObject pawn1;
    private static int INFTY = 9000;
    int moves = 6;
    Dictionary<Vector3, int> theDists = new Dictionary<Vector3, int>();
    /// <summary> an enum for the card phases </summary>
    private enum CardPhase
    {
        phase1, phase2, action
    }

    //We also initialize the theDists vector to store distances from target pawn
    public override void firstAction()
    {
        theDists = new Dictionary<Vector3, int>(); //reinitialize
        TogglePhase(CardPhase.phase1);
    }

    public override bool applyEffect() //click behavior
    {
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D playerHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("UnitsLayer"));
        RaycastHit2D floorHitter = Physics2D.Raycast(rayPos, Vector2.zero, 0f, LayerMask.GetMask("Default"));
        if (currentPhase == CardPhase.phase1 && gm.IsIlluminated(floorHitter.transform.position) && playerHitter) //click a pawn. if it's your side, move = 6. else, move = 3.
        {
            pawn1 = playerHitter.transform.gameObject;
            if (gm.GetPawnSide(pawn1) == pm.GetPlayerSide())
                moves = 6;
            else if (gm.GetPawnSide(pawn1) == pm.GetNotPlayerSide())
                moves = 3;
            TogglePhase(CardPhase.phase2);
        }
        else if (currentPhase == CardPhase.phase2 && gm.IsIlluminated(floorHitter.transform.position) && !playerHitter) //select a space to move to.
        {
            LinkedList<Vector3> path = new LinkedList<Vector3>();
            GetPath(floorHitter.transform.position, path, theDists);
            path.AddLast(floorHitter.transform.position); //the path doesn't add the last pos so... here :/
            StartCoroutine(MoveToPos(path));
        }
        return true;
    }

    /// <summary>
    /// Used to move along a path.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    IEnumerator MoveToPos(LinkedList<Vector3> path)
    {
        while (path.Count > 0)
        {
            Vector3 end = path.First.Value;
            path.RemoveFirst();
            yield return StartCoroutine(pawn1.GetComponent<Pawn>().SmoothMovement(end));
        }
        TogglePhase(CardPhase.action);
    }

    void TogglePhase(CardPhase phase)
    {
        currentPhase = phase;
        if (phase == CardPhase.phase1) //Illuminate red and blue pawns
        {
            gm.IllumTeam(pm.GetPlayerSide(), Highlight.Plain);
            gm.IllumTeam(pm.GetNotPlayerSide(), Highlight.Plain);
        }
        else if(phase == CardPhase.phase2)
        {
            gm.DellumPositions();
            //illuminate the rest
            theDists = IllumValidPositions();
            if (gm.IlluminatedPositionsCount() == 0)
                CancelCard();
        }
        else if(phase == CardPhase.action)
        {
            gm.DellumPositions();
            RestorePM();
            hm.DiscardCard(this.gameObject);
        }

    }

    /// <summary>
    /// Retrieves the path to move along.
    /// </summary>
    /// <param name="pos">end position</param>
    /// <param name="path">referenced path list</param>
    /// <param name="dists">dictionary of dists from pawn</param>
    public void GetPath(Vector3 pos, LinkedList<Vector3> path, Dictionary<Vector3, int> dists)
    {
        if (dists[pos] <= 1)
        {
            return;
        }

        //To Find neighbors:
        Vector3 upDir = new Vector3(pos.x, pos.y + 1, 0);
        Vector3 downDir = new Vector3(pos.x, pos.y - 1, 0);
        Vector3 leftDir = new Vector3(pos.x - 1, pos.y, 0);
        Vector3 rightDir = new Vector3(pos.x + 1, pos.y, 0);
        Vector3 chosenDir = _minVec(new Vector3[] { upDir, downDir, leftDir, rightDir }, dists);

        // Debug.Log(dists[pos]);
        path.AddFirst(chosenDir);
        GetPath(chosenDir, path,dists);
    }

    /// <summary>
    /// Helper function for GetPath. Chooses the vector with minimum dists value given an array.
    /// </summary>
    /// <param name="vecs">array of vectors</param>
    /// <returns>the minimal vector</returns>
    private Vector3 _minVec(Vector3[] vecs, Dictionary<Vector3, int> dists)
    {
        Miscellaneous.Shuffle<Vector3>(vecs); //We use shuffle so that you're given a different path every time.

        int min = INFTY;
        int minIndex = -1;
        for (int i = 0; i < vecs.Length; i++)
        {
            if (dists.ContainsKey(vecs[i]) && dists[vecs[i]] < min)
            {
                minIndex = i;
                min = dists[vecs[i]];
            }
        }
        return vecs[minIndex];
    }

    Dictionary<Vector3, int> IllumValidPositions() //illuminates valid positions as green
    {
        Dictionary<Vector3, int> dists = FindValidPositions(moves);
        List<Vector3> tilesToIlluminate = new List<Vector3>();
        foreach(var item in dists)
        {
            if(item.Value <= moves)
            {
                if (item.Key != pawn1.transform.position)
                {
                    tilesToIlluminate.Add(item.Key);
                    Debug.Log(item.Key + "," + item.Value);
                }
            }
        }
        gm.IllumPositions(tilesToIlluminate,Highlight.Plain);
        return dists;
    }

    List<Vector3> validPositions(Vector3 pos)
    {
        List<Vector3> validPos = new List<Vector3>();
        Vector3 upDir = new Vector3(pos.x, pos.y + 1, pos.z);
        Vector3 downDir = new Vector3(pos.x, pos.y - 1, pos.z);
        Vector3 leftDir = new Vector3(pos.x - 1, pos.y, pos.z);
        Vector3 rightDir = new Vector3(pos.x + 1, pos.y, pos.z);

        if (_validPosition(upDir)) validPos.Add(upDir);
        if (_validPosition(downDir)) validPos.Add(downDir);
        if (_validPosition(leftDir)) validPos.Add(leftDir);
        if (_validPosition(rightDir)) validPos.Add(rightDir);
        return validPos;
    }

    bool _validPosition(Vector3 pos) //if it's valid, that means there's no pawns or tiles on the same tag
    {
        if(gm.GetTileType(pos)==TileType.Empty && !gm.GetPawnAtPos(pos))
            return true;
        return false;
    }


    Dictionary<Vector3, int> FindValidPositions(int moves) //gets an array of valid positions. probably their positions.
    {
        List<Vector3> gridPositions = gm.tilePositions.Keys.ToList<Vector3>();
        Dictionary<Vector3, int> dists = new Dictionary<Vector3, int>();
        foreach(Vector3 i in gridPositions)
            dists[i] = INFTY;

        PriorityQueue<int, Vector3> Q = new PriorityQueue<int, Vector3>();
        dists[pawn1.transform.position] = 0;
        foreach (Vector3 v in gridPositions)
            Q.Enqueue(v, dists[v]);

        while (!Q.IsEmpty)
        {
            Vector3 u = Q.Dequeue();
            if (dists[u] >= moves)
                continue;

            List<Vector3> neighbors = validPositions(u);

            foreach(Vector3 neigh in neighbors)
            {
                int alt = dists[u] + 1;

                if (alt < dists[neigh])
                {
                    Q.Replace(neigh, dists[neigh], alt);
                    dists[neigh] = alt;
                }
            }

        }
        return dists;
    }

}
