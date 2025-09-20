using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class LobbyJoin : MonoBehaviour
{
    public InputField roomNumberInput;
    public Button joinButton;
    public NetworkRunner runnerPrefab;
    private LobbyNetwork lobbyNetwork;

    void Start()
    {
        // runnerを生成する
        NetworkRunner runner = Instantiate(runnerPrefab);
        DontDestroyOnLoad(runner.gameObject);
        lobbyNetwork = new(runner);
        runner.AddCallbacks(lobbyNetwork);
        joinButton.onClick.AddListener(OnJoinButtonClicked);
    }

    private void OnJoinButtonClicked()
    {
        if (roomNumberInput == null) return;
        string roomNumber = roomNumberInput.text;
        Debug.Log($"入力された文字列：{roomNumber}");

        lobbyNetwork.JoinRoom(roomNumber);
    }
} 
