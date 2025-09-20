using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class LobbyJoin : MonoBehaviour
{
    public InputField roomNumberInput;
    public NetworkRunner runnerPrefab;
    public NetworkObject masterClientPrefab;
    public NetworkObject clientPrefab;
    private LobbyNetwork lobbyNetwork;

    void Start()
    {
        // runnerを生成する
        NetworkRunner runner = Instantiate(runnerPrefab);
        DontDestroyOnLoad(runner.gameObject);
        lobbyNetwork = new(runner);
        runner.AddCallbacks(lobbyNetwork);
    }

    public async void OnInputJoin()
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();

        if (roomNumberInput == null) return;
        string roomNumber = roomNumberInput.text;
        Debug.Log($"入力された文字列：{roomNumber}");

        await lobbyNetwork.JoinRoom(roomNumber);
        Scene.Lobby.LoadScene(runner);
    }

    public async void OnInputCancel()
    {
        await lobbyNetwork.LeaveRoomAsync();

        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        if (runner != null)
            Destroy(runner.gameObject);

        Scene.Start.LoadScene();
    }
} 
