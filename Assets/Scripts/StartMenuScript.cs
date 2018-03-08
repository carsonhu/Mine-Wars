using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuScript : MonoBehaviour {

    public void OnStartLocalMultiplayerBtn()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public void OnStartOnlineMultiplayerBtn()
    {
        SceneManager.LoadScene("Multiplayer Lobby", LoadSceneMode.Single);
    }

}
