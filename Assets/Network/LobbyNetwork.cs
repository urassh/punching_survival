using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class LobbyNetwork : INetworkRunnerCallbacks
{
    private List<SessionInfo> cachedSessionList = new();
    private NetworkRunner runner;

    public LobbyNetwork(NetworkRunner runner)
    {
        this.runner = runner;
        runner.AddCallbacks(this);
        
        // セッションロビーへの接続を試行
        try
        {
            runner.JoinSessionLobby(SessionLobby.Shared);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"セッションロビーへの接続に失敗しました: {e.Message}");
        }
    }

    public async void CreateRoom(string roomNumber)
    {
        if (runner == null || !runner.IsRunning) return;

        var result = await runner.StartGame(new StartGameArgs
        {
            SessionName = roomNumber,
            GameMode = GameMode.Host,
            Scene = Scene.Lobby.GetSceneRef(),
            SceneManager = runner.gameObject.GetComponent<NetworkSceneManagerDefault>()
        });

        Debug.Log(result);
        Debug.Log($"生成したルームの名前:{runner.SessionInfo.Name}");
    }

    public async void JoinRoom(string roomNumber)
    {
        if (runner == null || !runner.IsRunning || !IsExistRoom(roomNumber)) return;

        var result = await runner.StartGame(new StartGameArgs
        {
            SessionName = roomNumber,
            GameMode = GameMode.Client,
            Scene = Scene.Lobby.GetSceneRef(),
            SceneManager = runner.gameObject.GetComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
            Debug.Log($"ルームに参加しました");
        else
            Debug.Log($"ルームに参加できませんでした");
        Debug.Log(result);
        Debug.Log($"参加したルームの名前:{runner.SessionInfo.Name}");
    }

    private bool IsExistRoom(string roomNumber)
    {
        if (runner == null || !runner.IsRunning) return false;

        foreach (SessionInfo session in cachedSessionList)
            if (session.Name == roomNumber)
                return true;
        return false;
    }

    public void LeaveRoom()
    {
        Debug.Log("ルームを退出しました。");
        if (runner != null && runner.IsRunning)
        {
            runner.Shutdown();
        }
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {}

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {}

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) 
    {
        Debug.Log($"NetworkRunner がシャットダウンしました: {shutdownReason}");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) 
    {
        Debug.LogError($"サーバーから切断されました: {reason}");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) 
    {
        Debug.LogError($"接続に失敗しました: {reason}");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) {}

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {}

    public void OnInput(NetworkRunner runner, NetworkInput input) {}
    
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}

    public void OnConnectedToServer(NetworkRunner runner) {}

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        cachedSessionList = sessionList;
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}

    public void OnSceneLoadDone(NetworkRunner runner) {}
    

    public void OnSceneLoadStart(NetworkRunner runner) {}
}
