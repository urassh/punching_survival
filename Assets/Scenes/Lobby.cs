using UnityEngine;
using Fusion;

public class Lobby : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    public RoomNumberText roomNumberText;
    private LobbyNetwork lobbyNetwork;

    private void Awake()
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();

        if (runner == null || !runner.IsRunning)
        {
            //prefab(ゲームオブジェクトの設計書)を使ってゲームオブジェクトをコピーする
            runner = Instantiate(networkRunnerPrefab);
            lobbyNetwork = new(runner);
            int roomNumber = CreateRoomNum();

            // ゲーム終了まで保持する(キャンセル時は削除される)
            DontDestroyOnLoad(runner.gameObject);
            if (roomNumberText != null)
                roomNumberText.SetRoomNumber(roomNumber);
            lobbyNetwork.CreateRoom(roomNumber.ToString());
        }
        Debug.Log("ルームは生成されています。");
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


    private int CreateRoomNum()
    {
        return Random.Range(1000, 10000);
    }
}
