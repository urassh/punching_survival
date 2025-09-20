using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyNetwork : INetworkRunnerCallbacks
{
    private List<SessionInfo> cachedSessionList = new();
    private readonly NetworkRunner runner;
    public Action<NetworkRunner> OnConnectedCallback { get; set; }
    private TaskCompletionSource<bool> sessionListUpdatedTask;

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
        Debug.Log("ルーム参加を開始します...");
        
        if (runner == null) return;

        await runner.JoinSessionLobby(SessionLobby.Shared);

        // セッションリストの更新を待つ
        if (sessionListUpdatedTask == null || sessionListUpdatedTask.Task.IsCompleted)
        {
            sessionListUpdatedTask = new TaskCompletionSource<bool>();
            Debug.Log("セッションリストの更新を待機中...");
            await sessionListUpdatedTask.Task;
        }

        Debug.Log("IsExistRoom: " + IsExistRoom(roomNumber));
        
        if (!IsExistRoom(roomNumber)) 
        {
            Debug.LogError($"ルーム '{roomNumber}' が見つかりません");
            return;
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
        foreach (SessionInfo session in cachedSessionList)
        {
            if (session.Name == roomNumber)
                return true;
            Debug.Log($"セッション名: {session.Name}");
        }
        Debug.Log($"cachedSessionList.count: {cachedSessionList.Count}");

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
        if (runner.IsSharedModeMasterClient)
            OnConnectedCallback?.Invoke(runner);
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        cachedSessionList = sessionList;
        Debug.Log($"セッションリストが更新されました。現在のルーム数: {cachedSessionList.Count}");
        
        // セッションリストの更新完了を通知
        sessionListUpdatedTask?.SetResult(true);
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
            OnConnectedCallback?.Invoke(runner);
    }

    public void OnSceneLoadStart(NetworkRunner runner) {}
}
