using UnityEngine;
using Fusion;

public class Lobby : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    public RoomNumberText roomNumberText;
    public NetworkObject masterClientPrefab;
    public NetworkObject clientPrefab;
    private LobbyNetwork lobbyNetwork;

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
        int roomNumber = CreateRoomNum();

        if (roomNumberText != null)
            roomNumberText.SetRoomNumber(roomNumber);
        
        Debug.Log($"ルーム番号 {roomNumber} でルーム作成を開始します");
        lobbyNetwork.CreateRoom(roomNumber.ToString());
    }

    public void OnInputStart()
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();

        Debug.Log("LOAD START");
        Debug.Log("runner.IsSharedModeMasterClient: " + runner.IsSharedModeMasterClient);
        
        // SharedModeのマスタークライアントのみmasterClientPrefabを生成
        if (runner.IsSharedModeMasterClient && masterClientPrefab != null)
        {
            runner.Spawn(masterClientPrefab);
        }
        
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


    private int CreateRoomNum()
    {
        return Random.Range(1000, 10000);
    }
}
