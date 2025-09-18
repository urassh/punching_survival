using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using System;


public class ConnectRoom : MonoBehaviour, INetworkRunnerCallbacks
{
    public InputField roomNumberInput;
    public string roomNumber;
    public Button joinButton;
    public NetworkRunner runnerPrefab;
    public NetworkRunner runner;

    // ロビーにあるセッション名が入ってるリスト
    private List<SessionInfo> cachedSessionList = new List<SessionInfo>();

    async Task Start()
    {
        // runnerを生成する
        runner = Instantiate(runnerPrefab);
        DontDestroyOnLoad(runner.gameObject);
        // コールバック関数を作る（ロビー接続等が成功したときに行う関数）
        runner.AddCallbacks(this);
        // ロビーに接続する
        await runner.JoinSessionLobby(SessionLobby.Shared);
        // ボタンが押されたときに実行する関数
        joinButton.onClick.AddListener(OnJoinButtonClicked);
    }

    public void OnSessionListUpdated(NetworkRunner runner, System.Collections.Generic.List<SessionInfo> sessionList)
    {
        // ルームのリストを取得する
        cachedSessionList = sessionList;
        Debug.Log($"ロビーに接続しました。");
    }

    private void OnJoinButtonClicked()
    {
        if (roomNumberInput != null)
        {
            roomNumber = roomNumberInput.text;
            Debug.Log($"入力された文字列：{roomNumber}");
        }
        else
        {
            Debug.LogError("4桁の番号を入力してください。");
        }

        SessionInfo sessionToJoin = null;
        int i = 0;
        while (i < cachedSessionList.Count)
        {
            SessionInfo currentSession = cachedSessionList[i];
            if (currentSession.Name == roomNumber)
            {
                sessionToJoin = currentSession;
                break;
            }
            i++;
        }
        if (sessionToJoin == null)
        {
            Debug.Log($"ルームが見つかりませんでした。");
        }
        else
        {
            Debug.Log($"ルームが見つかりました！");
            JoinSession(roomNumber);
        }
    }


    private async void JoinSession(string roomNumber)
    {
        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = roomNumber,
        });
        if (result.Ok)
        {
            Debug.Log($"ルームに参加しました");
        }
        else
        {
            Debug.Log($"ルームに参加できませんでした");
        }
    }

    // なぜか入れないとコンパイルエラーになる、、？
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken token) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}
