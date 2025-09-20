using UnityEngine;
using Fusion;

public class Play : MonoBehaviour
{
    public NetworkObject playerPrefab; // プレイヤープレハブの参照
    private PlayNetwork playNetwork;

    private void Awake()
    {
        // NetworkRunnerを探してコールバックを登録
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        playNetwork = new PlayNetwork(runner);
        playNetwork.OnLoadedSceneCallback += OnLoadedPlayScene;
    }

    private void OnDestroy()
    {
        // コールバックを削除
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        if (runner != null)
        {
            runner.RemoveCallbacks(playNetwork);
        }
    }

    private void OnLoadedPlayScene(NetworkRunner runner)
    {
        runner.Spawn(playerPrefab);
    }
}
