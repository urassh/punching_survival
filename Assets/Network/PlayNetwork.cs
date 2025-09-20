using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class PlayNetwork : INetworkRunnerCallbacks
{
    public Action<NetworkRunner> OnLoadedSceneCallback { get; set; }

    private readonly NetworkRunner runner;

    public PlayNetwork(NetworkRunner runner)
    {
        this.runner = runner;
        runner.AddCallbacks(this);
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
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {}
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("シーンロード2が完了しました");
        OnLoadedSceneCallback?.Invoke(runner);
    }

    public void OnSceneLoadStart(NetworkRunner runner) {}
}
