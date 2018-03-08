using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class NETLoader : NetworkBehaviour {

    // Use this for initialization
    GameObject gameManager;
    void Awake()
    {
        if (GameManager.instance == null)
            gameManager = Resources.Load("Networking/NETGameManager") as GameObject;
        GameObject gm = Instantiate(gameManager);
        if(NetworkServer.active)
            NetworkServer.Spawn(gm);
        Destroy(this);
    }
}
