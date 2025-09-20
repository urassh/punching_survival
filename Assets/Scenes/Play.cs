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
		// 各クライアントが自分のプレイヤーのみをスポーンする
		PlayerRef localPlayer = runner.LocalPlayer;
		if (localPlayer != null)
		{
			// プレイヤーIDに基づいてスポーンポイントを決定
			int spawnIndex = localPlayer.PlayerId % spawnPoints.Length;
			Vector3 spawnPosition = spawnPoints[spawnIndex].position;
			// 0,0,0の方向を向くように設定
			Quaternion spawnRotation = Quaternion.LookRotation(Vector3.forward);
			runner.Spawn(playerPrefab, spawnPosition, spawnRotation, localPlayer);
		}
	}
}
