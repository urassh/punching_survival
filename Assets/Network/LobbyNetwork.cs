using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class LobbyNetwork : INetworkRunnerCallbacks
{
    private List<SessionInfo> cachedSessionList = new();
    private readonly NetworkRunner runner;
    public Action<NetworkRunner> OnConnectedCallback { get; set; }

    public LobbyNetwork(NetworkRunner runner)
    {
        this.runner = runner;
        runner.AddCallbacks(this);
    }

    public async void CreateRoom(string roomNumber)
    {
        if (runner == null) return;

        // 既に接続されている場合は一度シャットダウン
        if (runner.IsRunning)
        {
            Debug.Log("既存の接続をシャットダウンしています...");
            await runner.Shutdown();
        }

        var result = await runner.StartGame(new StartGameArgs
        {
            SessionName = roomNumber,
            GameMode = GameMode.Shared,
            Scene = Scene.Lobby.GetSceneRef(),
            SceneManager = runner.gameObject.GetComponent<NetworkSceneManagerDefault>()
        });

        Debug.Log(result);
        if (result.Ok)
        {
            Debug.Log($"生成したルームの名前:{runner.SessionInfo.Name}");
            Debug.Log($"IsSharedModeMasterClient: {runner.IsSharedModeMasterClient}");
        }
        else
        {
            Debug.LogError($"ルーム作成に失敗しました: {result.ErrorMessage}");
            throw new Exception($"ルーム作成に失敗しました: {result.ErrorMessage}");
        }
    }

    public async Task JoinRoom(string roomNumber)
    {
        await runner.JoinSessionLobby(SessionLobby.Shared);

        if (runner == null || !IsExistRoom(roomNumber)) return;

        // 既に接続されている場合は一度シャットダウン
        if (runner.IsRunning)
        {
            Debug.Log("既存の接続をシャットダウンしています...");
            await runner.Shutdown();
        }

        var result = await runner.StartGame(new StartGameArgs
        {
            SessionName = roomNumber,
            GameMode = GameMode.Shared,
            Scene = Scene.Lobby.GetSceneRef(),
            SceneManager = runner.gameObject.GetComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
        {
            Debug.Log($"ルームに参加しました");
            Debug.Log($"IsSharedModeMasterClient: {runner.IsSharedModeMasterClient}");
        }
        else
        {
            Debug.Log($"ルームに参加できませんでした: {result.ErrorMessage}");
        }
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

    public async Task LeaveRoomAsync()
    {
        Debug.Log("ルームを退出しました。");
        if (runner != null)
        {
            await runner.Shutdown(shutdownReason: ShutdownReason.Ok);
        }
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
    {
        Debug.Log($"プレイヤーが参加しました: {player}");
        Debug.Log($"IsSharedModeMasterClient: {runner.IsSharedModeMasterClient}");
        Debug.Log($"SessionInfo IsValid: {runner.SessionInfo.IsValid}");
        Debug.Log($"Runner State: {runner.State}");
    }

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

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("サーバーに接続されました");
        Debug.Log($"IsSharedModeMasterClient: {runner.IsSharedModeMasterClient}");

        // コールバック関数が設定されている場合は実行
        OnConnectedCallback?.Invoke(runner);
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        cachedSessionList = sessionList;
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}

    public void OnSceneLoadDone(NetworkRunner runner) {}

    public void OnSceneLoadStart(NetworkRunner runner) {}
}
