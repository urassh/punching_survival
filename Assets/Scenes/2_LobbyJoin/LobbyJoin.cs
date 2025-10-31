using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class LobbyJoin : MonoBehaviour
{
    [SerializeField] private InputField roomNumberInput;
    [SerializeField] private Button joinButton;
    [SerializeField] private Text loadingText;
    [SerializeField] private Text errorText;
    [SerializeField] private GameObject inputUI;
    public NetworkRunner runnerPrefab;
    private LobbyNetwork lobbyNetwork;
    
    [SerializeField] private Color enabledColor = Color.white;
    [SerializeField] private Color disabledColor = Color.gray;

    void Start()
    {
        // runnerを生成する
        NetworkRunner runner = Instantiate(runnerPrefab);
        DontDestroyOnLoad(runner.gameObject);
        lobbyNetwork = new(runner);
        
        // 接続失敗時のコールバックを設定
        lobbyNetwork.OnJoinFailedCallback = OnJoinFailed;
        
        // 初期状態ではボタンを無効化
        SetButtonState(joinButton, false);
        
        // 初期状態ではエラーテキストを非表示
        errorText.gameObject.SetActive(false);
    }

    public void OnInputRoomNumber()
    {
        bool isValidRoomNumber = IsValidRoomNumber(roomNumberInput.text);
        SetButtonState(joinButton, isValidRoomNumber);
    }
    
    private bool IsValidRoomNumber(string roomNumber)
    {
        return roomNumber.Length == 4 && int.TryParse(roomNumber, out _);
    }

    private void OnJoinFailed(string errorMessage)
    {
        Debug.LogError($"ルーム参加に失敗しました: {errorMessage}");
        loadingText.gameObject.SetActive(false);
        inputUI.SetActive(true);
        errorText.text = errorMessage;
        errorText.gameObject.SetActive(true);
    }

    public async void OnInputJoin()
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();

        if (roomNumberInput == null) return;
        string roomNumber = roomNumberInput.text;
        Debug.Log($"入力された文字列：{roomNumber}");
        
        // エラーテキストを非表示にして新しい試行を開始
        errorText.gameObject.SetActive(false);
        loadingText.gameObject.SetActive(true);
        inputUI.SetActive(false);

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

    private void SetButtonState(Button button, bool isEnabled)
    {
        button.enabled = isEnabled;

        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = isEnabled ? enabledColor : disabledColor;
        colorBlock.highlightedColor = isEnabled ? enabledColor : disabledColor;
        colorBlock.pressedColor = isEnabled ? enabledColor : disabledColor;
        colorBlock.selectedColor = isEnabled ? enabledColor : disabledColor;
        colorBlock.disabledColor = disabledColor;
        button.colors = colorBlock;
    }
} 
