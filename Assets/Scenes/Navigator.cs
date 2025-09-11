using UnityEngine;

public class Navigator : MonoBehaviour
{
    public void NavigateToStart()
    {
        LoadScene("Start");
    }

    public void NavigateToLobby()
    {
        LoadScene("Lobby");
    }

    public void NavigateToLobbyJoin()
    {
        LoadScene("LobbyJoin");
    }

    public void NavigateToPlay()
    {
        LoadScene("Play");
    }

    public void NavigateToResult()
    {
        LoadScene("Result");
    }

    private void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
