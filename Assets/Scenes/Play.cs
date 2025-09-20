using UnityEngine;
using Fusion;

public class Play : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
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
		SpawnAllPlayers(runner);
		// スタークライアントだけが、ランキング登録処理を実行する
		// runner.IsSharedModeMasterClient は、マスタークライアントでのみtrueを返す
		if (runner.IsSharedModeMasterClient)
		{
			//PlayerInfoをFind
			PlayerInfo playerInfo = FindObjectOfType<PlayerInfo>();
			//全プレイヤー情報をRankingに登録
			Ranking ranking = FindObjectOfType<Ranking>();
			for (int i = 0; i < playerInfo.PlayerCount; i++)
			{
				ranking.RPC_RegisterPlayer(playerInfo.PlayerIds[i].ToString(), playerInfo.PlayerNames[i].ToString());
			}
		}
	}
	
    private void SpawnAllPlayers(NetworkRunner runner)
	{
		int spawnIndex = 0;
		foreach (PlayerRef player in runner.ActivePlayers)
		{
			// 自分自身のキャラクターだけを生成する
			if (player == runner.LocalPlayer)
			{
				// スポーンポイントのインデックスを計算（最大4つまで対応）
				Vector3 spawnPosition = spawnIndex < spawnPoints.Length ? spawnPoints[spawnIndex].position : Vector3.zero;
				runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
			}
			spawnIndex++;
		}
	}
}
