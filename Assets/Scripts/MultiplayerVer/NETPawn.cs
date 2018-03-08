using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NETPawn : NetworkBehaviour {

    public float moveTime = 0.1f;
    private float inverseMoveTime; //just 1 / moveTime

    private BoxCollider2D boxCollider; //boxCollider of the object
    private Rigidbody2D rb2D; //rigidbody of the object

    // Use this for initialization
    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
    }

    public IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition); //move the rigidbody over there
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }


    public void MoveTo(Vector3 pos)
    {
        StartCoroutine(SmoothMovement(pos));
    }

    [ClientRpc]
    public void RpcSwitchSprite(Team affiliation)
    {
        if(affiliation == Team.Blue)
        {
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/bluePawn");
        }else if(affiliation == Team.Red)
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/redPawn");
        else
            Debug.Log("Something went wrong!");
    }

}
