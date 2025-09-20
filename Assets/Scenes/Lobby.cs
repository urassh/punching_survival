using UnityEngine;
using Fusion;

public class Lobby : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    public RoomNumberText roomNumberText;
    public GameObject connectedUI;
    public GameObject connectingUI;
    public NetworkObject playerInfoPrefab;
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
            lobbyNetwork.OnConnectedCallback += OnConnected;
            lobbyNetwork.OnPlayerLeaveCallback += RemovePlayerFromLobby;
            roomNumber = Random.Range(1000, 10000);

            Debug.Log($"ルーム番号 {roomNumber} でルーム作成を開始します");
            lobbyNetwork.CreateRoom(roomNumber.ToString());
        }
        else
        {
            Debug.Log("Room Name: " + runner.SessionInfo.Name);
            roomNumber = int.Parse(runner.SessionInfo.Name);
            lobbyNetwork = new(runner);
            lobbyNetwork.OnConnectedCallback += OnConnected;
            lobbyNetwork.OnPlayerLeaveCallback += RemovePlayerFromLobby;
            Debug.Log($"ルーム番号 {roomNumber} のルームに参加します");
        }
    }

    public void OnInputStart()
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        
        Scene.Play.LoadScene(runner);
    }

    public async void OnInputCancel()
    {
        await lobbyNetwork.LeaveRoomAsync();

        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        if (runner != null)
            Destroy(runner.gameObject);

        Scene.Start.LoadScene();
    }

    public void OnConnected(NetworkRunner runner)
    {
        if (connectedUI != null && connectingUI != null)
        {
            connectedUI.SetActive(true);
            connectingUI.SetActive(false);
            roomNumberText.SetRoomNumber(roomNumber);
        }

        // 最初のクライアント（ホスト）がPlayerInfoオブジェクトを作成
        CreatePlayerInfoIfNeeded(runner);
    }

    private void CreatePlayerInfoIfNeeded(NetworkRunner runner)
    {
        // 既にPlayerInfoが存在するかチェック
        PlayerInfo existingPlayerInfo = FindObjectOfType<PlayerInfo>();
        if (existingPlayerInfo != null)
        {
            Debug.Log("PlayerInfoは既に存在します");
            // PlayerInfoのSpawned()メソッドで自動的にプレイヤー情報が追加されるため、
            // ここでRPC_AddPlayerを呼ぶ必要はありません
            return;
        }

        // ホスト（SharedModeMasterClient）のみがPlayerInfoオブジェクトを作成
        if (runner.IsSharedModeMasterClient && playerInfoPrefab != null)
        {
            Debug.Log("ホストがPlayerInfoオブジェクトを作成します");
            runner.Spawn(playerInfoPrefab, Vector3.zero, Quaternion.identity);
        }
    }

    /// <summary>
    /// PlayerInfoからそのクライアントのプレイヤーを削除する
    /// </summary>
    private void RemovePlayerFromLobby()
    {
        Debug.Log("プレイヤーをロビーから削除中...");
        
        PlayerInfo playerInfo = FindObjectOfType<PlayerInfo>();
        if (playerInfo != null)
        {
            string playerId = PlayerPrefs.GetString("playerId", "");
            if (!string.IsNullOrEmpty(playerId))
            {
                Debug.Log($"プレイヤーID {playerId} を削除します");
                playerInfo.RPC_RemovePlayer(playerId);
            }
            else
            {
                Debug.LogWarning("PlayerIdが見つかりません");
            }
        }
        else
        {
            Debug.LogWarning("PlayerInfoオブジェクトが見つかりません");
        }
    }
}
