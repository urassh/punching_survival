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

        if (runner == null)
        {
            // prefab(ゲームオブジェクトの設計書)を使ってゲームオブジェクトをコピーする
            runner = Instantiate(networkRunnerPrefab);
            // ゲーム終了まで保持する(キャンセル時は削除される)
            DontDestroyOnLoad(runner.gameObject);
        }

        lobbyNetwork = new(runner);
        lobbyNetwork.OnConnectedCallback += OnConnected;
        roomNumber = CreateRoomNum();

        Debug.Log($"ルーム番号 {roomNumber} でルーム作成を開始します");
        lobbyNetwork.CreateRoom(roomNumber.ToString());
    }

    public void OnInputStart()
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();

        Debug.Log("LOAD START");
        Debug.Log("runner.IsSharedModeMasterClient: " + runner.IsSharedModeMasterClient);
        
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
    }

    private int CreateRoomNum()
    {
        return Random.Range(1000, 10000);
    }
}
