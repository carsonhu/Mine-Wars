using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NETNetworkManager : NetworkManager {

    const int maxPlayers = 2;
    NETPlayerMenu[] players = new NETPlayerMenu[2];

   // public override void


    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        for(int slot=0; slot < maxPlayers; slot++)
        {
            if(players[slot] == null)
            {
                var playerObj = (GameObject)GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                var playerMenu = playerObj.GetComponent<NETPlayerMenu>();

                playerMenu.playerSide = (slot == 0) ? Team.Red : Team.Blue;
                players[slot] = playerMenu;

                Debug.Log("Adding player in slot " + slot);
                NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);
                return;
            }
        }

        conn.Disconnect();

        /*
        base.OnServerAddPlayer(conn, playerControllerId);
        GameObject[] thePlayers = GameObject.FindGameObjectsWithTag("PlayerMenu");
        foreach(GameObject player in thePlayers)
        {
            if(player.GetComponent<NetworkIdentity>().connectionToClient.connectionId == conn.connectionId)
            {
                if (thePlayers.Length <= 1)
                    player.GetComponent<NETPlayerMenu>().ServerInitPlayer(Team.Red);
                else
                    player.GetComponent<NETPlayerMenu>().ServerInitPlayer(Team.Blue);
            }
        }*/
    }

}
