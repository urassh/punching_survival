using UnityEngine;
using Fusion;

public class Lobby : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    public RoomNumberText roomNumberText;
    public GameObject connectedUI;
    public GameObject connectingUI;
    public NetworkObject playerPrefab;
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
        Debug.Log("OnConnected");

        if (connectedUI != null && connectingUI != null)
        {
            connectedUI.SetActive(true);
            connectingUI.SetActive(false);
            roomNumberText.SetRoomNumber(roomNumber);
        }
    }
}
