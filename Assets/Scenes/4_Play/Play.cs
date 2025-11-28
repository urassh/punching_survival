using UnityEngine;
using Fusion;

public class Play : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
	[SerializeField] private NetworkObject[] playerPrefabs;
	public NetworkObject rankingPrefab; // Rankingプレハブの参照
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
		// マスタークライアントの場合のみRankingオブジェクトをスポーン
		if (runner.IsSharedModeMasterClient && rankingPrefab != null)
		{
			runner.Spawn(rankingPrefab, Vector3.zero, Quaternion.identity);
		}

		SpawnAllPlayers(runner);

        // 3秒間の猶予を持たせてからランキング登録処理を実行
        Invoke(nameof(RegisterAllPlayersToRanking), 5.0f);
	}

    // 全プレイヤーをランキングに登録するメソッド
    private void RegisterAllPlayersToRanking()
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        if (runner == null || !runner.IsSharedModeMasterClient) return;

        PlayerInfo playerInfo = FindObjectOfType<PlayerInfo>();
        Ranking ranking = FindObjectOfType<Ranking>();

        // オブジェクトの存在確認とNetworkBehaviourの初期化確認
        if (playerInfo == null || ranking == null)
        {
            Debug.LogWarning("PlayerInfo or Ranking not found, retrying in 2 seconds...");
            Invoke(nameof(RegisterAllPlayersToRanking), 2.0f);
            return;
        }

        // NetworkBehaviourが適切に初期化されているかチェック
        if (!ranking.Object.IsValid)
        {
            Debug.LogWarning("Ranking NetworkBehaviour not properly initialized, retrying in 2 seconds...");
            Invoke(nameof(RegisterAllPlayersToRanking), 2.0f);
            return;
        }

        for (int i = 0; i < playerInfo.PlayerCount; i++)
        {
            ranking.RPC_RegisterPlayer(playerInfo.PlayerIds[i].ToString(), playerInfo.PlayerNames[i].ToString());
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
			NetworkObject playerPrefab = playerPrefabs[spawnIndex];
            // 0,0,0の方向を向くように設定
			Quaternion spawnRotation = Quaternion.LookRotation(Vector3.forward);
            runner.Spawn(playerPrefab, spawnPosition, spawnRotation, localPlayer);
        }
    }

    public void OnDropPlayer(string playerId)
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        if (runner == null)
            return ;
        Ranking ranking = FindObjectOfType<Ranking>();
        if (ranking == null)
            return ;
        ranking.RPC_SetDropPlayerRank(playerId);
        Debug.Log("Player Dropped: " + playerId);
        RPC_OnEndGame(ranking);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_OnEndGame(Ranking ranking)
    {
        Debug.Log("Checking if game has ended...");
        if (!ranking.IsRankingComplete())
            return ;
        Debug.Log("Game Ended. Loading Result Scene...");
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        Scene.Result.LoadScene(runner);
    }
}
