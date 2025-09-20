using UnityEngine;
using Fusion;

public class Play : MonoBehaviour
{
    public NetworkObject playerPrefab; // プレイヤープレハブの参照
    private PlayNetwork playNetwork;

	private void Awake()
	{
		// NetworkRunnerを探す
		NetworkRunner runner = FindObjectOfType<NetworkRunner>();

		// runnerがnullでないかチェック
		if (runner != null)
		{
			playNetwork = new PlayNetwork(runner);
			playNetwork.OnLoadedSceneCallback += OnLoadedPlayScene;
		}
		else
		{
			Debug.LogError("NetworkRunner not found in the scene. Please make sure it's present.");
		}	
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

    // PlaySceneがロードされたときに呼ばれるコールバック
    private void OnLoadedPlayScene(NetworkRunner runner)
    {
        runner.Spawn(playerPrefab);        
    }
}
