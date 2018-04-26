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

	public void OnStartOnlineInstructionsBtn()
	{
		SceneManager.LoadScene("Instructions Page", LoadSceneMode.Single);

	}
	public void InstructionsBack()
	{
		SceneManager.LoadScene("Title Scene", LoadSceneMode.Single);

	}

}
