using UnityEngine;
using Fusion;
using System.Collections;

public class Lobby : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds0_1 = new WaitForSeconds(0.1f);
    public NetworkRunner networkRunnerPrefab;
    public RoomNumberText roomNumberText;
    public GameObject connectedUI;
    public GameObject connectingUI;
    public NetworkObject playerInfoPrefab;
    public PlayerTexts playerTexts;
    private LobbyNetwork lobbyNetwork;
    private int roomNumber;

    private void Awake()
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();

        // LobbyCreate -> Lobby
        if (runner == null)
        {
            // prefab(ゲームオブジェクトの設計書)を使ってゲームオブジェクトをコピーする
            runner = Instantiate(networkRunnerPrefab);
            // ゲーム終了まで保持する(キャンセル時は削除される)
            DontDestroyOnLoad(runner.gameObject);
            lobbyNetwork = new(runner);
            lobbyNetwork.OnConnectedCallback += OnConnectedHost;
            roomNumber = Random.Range(1000, 10000);

            Debug.Log($"ルーム番号 {roomNumber} でルーム作成を開始します");
            lobbyNetwork.CreateRoom(roomNumber.ToString());
        }
        else
        {
            Debug.Log("Room Name: " + runner.SessionInfo.Name);
            roomNumber = int.Parse(runner.SessionInfo.Name);
            lobbyNetwork = new(runner);
            lobbyNetwork.OnLoadedSceneCallback += OnConnectedClient;
            Debug.Log($"ルーム番号 {roomNumber} のルームに参加します");
        }
    }

    public void OnInputStart()
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();

        Scene.Play.LoadScene(runner);
    }

    public void OnInputCancel()
    {
        StartCoroutine(CancelAndLeaveRoom());
    }

    private IEnumerator CancelAndLeaveRoom()
    {
        // プレイヤー削除処理を同期的に実行
        yield return StartCoroutine(WaitForPlayerInfoAndRemovePlayer());
        
        // 少し待機してRPC処理が完了するのを確保
        yield return new WaitForSeconds(0.5f);
        
        // ルーム退出処理
        var leaveTask = lobbyNetwork.LeaveRoomAsync();
        yield return new WaitUntil(() => leaveTask.IsCompleted);

        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        if (runner != null)
            Destroy(runner.gameObject);

        Scene.Start.LoadScene();
    }

    /// ホストが接続されたときに呼ばれるコールバック
    private void OnConnectedHost(NetworkRunner runner)
    {
        if (connectedUI != null && connectingUI != null)
        {
            connectedUI.SetActive(true);
            connectingUI.SetActive(false);
            roomNumberText.SetRoomNumber(roomNumber);
            playerTexts.ActivatePlayerTexts();
        }

        runner.Spawn(playerInfoPrefab, Vector3.zero, Quaternion.identity);
        PlayerInfo playerInfo = FindObjectOfType<PlayerInfo>();
        playerInfo.AddCurrentPlayer();
    }

    /// ホスト以外のクライアントが接続されたときにPlayerInfoオブジェクトを作成
    private void OnConnectedClient(NetworkRunner runner)
    {
        if (connectedUI != null && connectingUI != null)
        {
            connectedUI.SetActive(true);
            connectingUI.SetActive(false);
            roomNumberText.SetRoomNumber(roomNumber);
            playerTexts.ActivatePlayerTexts();
        }

        // PlayerInfoオブジェクトが存在するまで待機
        StartCoroutine(WaitForPlayerInfoAndAddPlayer());
    }

    /// <summary>
    /// PlayerInfoオブジェクトが利用可能になるまで待機してからプレイヤーを追加
    /// </summary>
    private IEnumerator WaitForPlayerInfoAndAddPlayer()
    {
        PlayerInfo playerInfo = null;
        int maxAttempts = 50; // 最大5秒待機（0.1秒間隔で50回）
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            playerInfo = FindObjectOfType<PlayerInfo>();
            if (playerInfo != null)
            {
                // PlayerInfoが見つかったらすぐにループを終了
                Debug.Log("PlayerInfoオブジェクトが見つかりました。プレイヤーを追加します。");
                playerInfo.AddCurrentPlayer();
                yield break; // コルーチンを終了
            }

            yield return _waitForSeconds0_1;
            attempts++;
        }

        // ここに到達するのはタイムアウトした場合のみ
        Debug.LogError("PlayerInfoオブジェクトが見つかりませんでした。タイムアウトしました。");
    }
    
    /// <summary>
    /// PlayerInfoオブジェクトが利用可能になるまで待機してからプレイヤーを削除
    /// </summary>
    private IEnumerator WaitForPlayerInfoAndRemovePlayer()
    {
        PlayerInfo playerInfo = null;
        int maxAttempts = 50; // 最大5秒待機（0.1秒間隔で50回）
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            playerInfo = FindObjectOfType<PlayerInfo>();
            if (playerInfo != null)
            {
                // PlayerInfoが見つかったらすぐにループを終了
                Debug.Log("PlayerInfoオブジェクトが見つかりました。プレイヤーを削除します。");
                
                // 削除完了フラグ
                bool removalCompleted = false;
                
                // 削除完了コールバックを設定
                playerInfo.OnPlayerRemoved += () => removalCompleted = true;
                
                // プレイヤー削除を実行
                playerInfo.RemoveCurrentPlayer();
                
                // 削除完了まで待機（最大2秒）
                float waitTime = 0f;
                while (!removalCompleted && waitTime < 2f)
                {
                    yield return _waitForSeconds0_1;
                    waitTime += 0.1f;
                }
                
                // コールバックをクリーンアップ
                playerInfo.OnPlayerRemoved -= () => removalCompleted = true;
                
                if (removalCompleted)
                {
                    Debug.Log("プレイヤー削除が完了しました。");
                }
                else
                {
                    Debug.LogWarning("プレイヤー削除の完了確認がタイムアウトしました。");
                }
                
                yield break; // コルーチンを終了
            }

            yield return _waitForSeconds0_1;
            attempts++;
        }

        // ここに到達するのはタイムアウトした場合のみ
        Debug.LogError("PlayerInfoオブジェクトが見つかりませんでした。タイムアウトしました。");
    }
}
