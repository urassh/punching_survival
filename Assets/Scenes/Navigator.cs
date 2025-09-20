using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SceneInfo
{
    public string sceneName;
    public string scenePath;

    public SceneInfo(string name, string path)
    {
        sceneName = name;
        scenePath = path;
    }

    public readonly void LoadScene(NetworkRunner runner)
    {
        // if (runner.IsSharedModeMasterClient)
        runner.LoadScene(GetSceneRef(), LoadSceneMode.Additive);
    }

    public readonly void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    public readonly SceneRef GetSceneRef()
    {
        return SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath(scenePath));
    }
}

public static class Scene
{
    public static readonly SceneInfo Start = new("Start", "Assets/Scenes/Start.unity");
    public static readonly SceneInfo Lobby = new("Lobby", "Assets/Scenes/Lobby.unity");
    public static readonly SceneInfo LobbyJoin = new("LobbyJoin", "Assets/Scenes/LobbyJoin.unity");
    public static readonly SceneInfo Play = new("Play", "Assets/Scenes/Play.unity");
    public static readonly SceneInfo Result = new("Result", "Assets/Scenes/Result.unity");
}


public class Navigator : MonoBehaviour
{
    public void NavigateToStart()
    {
        Scene.Start.LoadScene();
    }

    public void NavigateToLobby()
    {
        Scene.Lobby.LoadScene();
    }

    public void NavigateToLobby(NetworkRunner runner)
    {
        Scene.Lobby.LoadScene(runner);
    }

    public void NavigateToLobbyJoin()
    {
        Scene.LobbyJoin.LoadScene();
    }

    public void NavigateToPlay(NetworkRunner runner)
    {
        Scene.Play.LoadScene(runner);
    }

    public void NavigateToResult()
    {
        Scene.Result.LoadScene();
    }
}
