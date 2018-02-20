using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour {

    //for Pawn movement, we currently just test out having only 1 move available. Perhaps the super move would be enabled with the use of that particular card

    public float moveTime = 0.1f;
    private float inverseMoveTime; //just 1 / moveTime

    private BoxCollider2D boxCollider; //boxCollider of the object
    private Rigidbody2D rb2D; //rigidbody of the object

    // Use this for initialization
    void Awake () {
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

    // Update is called once per frame
    void Update () {
		
	}
}
