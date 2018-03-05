using UnityEngine;
using Prototype.NetworkLobby;
using System.Collections;
using UnityEngine.Networking;

public class NetworkLobbyHook2 : LobbyHook
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        /*NetworkSpaceship spaceship = gamePlayer.GetComponent<NetworkSpaceship>();
        spaceship.name = lobby.name;
        spaceship.color = lobby.playerColor;
        spaceship.score = 0;
        spaceship.lifeCount = 3; */

        PlayerController IGNPlayer = gamePlayer.GetComponent<PlayerController>();
        IGNPlayer.color = lobby.playerColor;
        IGNPlayer.name = lobby.name;
        
        
    }
}
