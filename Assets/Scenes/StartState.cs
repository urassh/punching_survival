using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartState : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button createButton;
    
    [SerializeField] private Color enabledColor = Color.white;
    [SerializeField] private Color disabledColor = Color.gray;

    void Start()
    {
        SetButtonState(joinButton, false);
        SetButtonState(createButton, false);
    }

    public void OnInputPlayerName()
    {
        bool isValidName = !string.IsNullOrWhiteSpace(playerNameInputField.text);
        SetButtonState(joinButton, isValidName);
        SetButtonState(createButton, isValidName);
    }
    
    public void OnClickedJoin()
    {
        PlayerPrefs.SetString("PlayerName", playerNameInputField.text);
        Scene.LobbyJoin.LoadScene();
    }

    public void OnClickedCreate()
    {
        PlayerPrefs.SetString("PlayerName", playerNameInputField.text);
        Scene.Lobby.LoadScene();
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
