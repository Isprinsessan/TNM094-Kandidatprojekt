using UnityEngine;
using Prototype.NetworkLobby;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class NetworkLobbyHook2 : LobbyHook
{
    public HashSet<Color> colorListLobby = new HashSet<Color>();

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
      

        PlayerController IGNPlayer = gamePlayer.GetComponent<PlayerController>();
        IGNPlayer.color = lobby.playerColor;
        IGNPlayer.name = lobby.name;
        IGNPlayer.charindex = lobby.charindex;

        //might need to do this in ClientRpc to sync colorList properly. . .
        if (!colorListLobby.Contains(lobby.playerColor))
            colorListLobby.Add(lobby.playerColor);

        IGNPlayer.colorList = colorListLobby;



    /* 
     * make the list here with the colors 
     * */

    }
}
