using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System;

[Serializable]
public class PlayerData
{
    public string playerId;
    public string playerName;
    
    public PlayerData(string id, string name)
    {
        playerId = id;
        playerName = name;
    }
}

public class PlayerInfo : NetworkBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds0_5 = new(0.5f);

    [Networked, Capacity(4)] 
    public NetworkArray<NetworkString<_32>> PlayerIds { get; }
    
    [Networked, Capacity(4)] 
    public NetworkArray<NetworkString<_32>> PlayerNames { get; }
    
    [Networked] public int PlayerCount { get; set; }
    
    private const int MAX_PLAYERS = 4;

    public override void Spawned()
    {
        base.Spawned();
        
        // シーン間で保持
        DontDestroyOnLoad(gameObject);
        
        // シーン読み込み完了後に呼び出されるコールバックを登録
        StartCoroutine(WaitForSceneLoadAndRPC_AddPlayer());
    }

    private System.Collections.IEnumerator WaitForSceneLoadAndRPC_AddPlayer()
    {
        // 少し待ってからプレイヤー情報を追加（シーン読み込み完了を確実にする）
        yield return _waitForSeconds0_5;
        
        // 各クライアントが自分の情報をPlayerPrefsから読み取って追加
        if (Object.HasInputAuthority)
        {
            string playerId = PlayerPrefs.GetString("playerId", "");
            string playerName = PlayerPrefs.GetString("PlayerName", "Player");
            
            // PlayerIdが空の場合は新しく生成
            if (string.IsNullOrEmpty(playerId))
            {
                playerId = Guid.NewGuid().ToString();
                PlayerPrefs.SetString("playerId", playerId);
                PlayerPrefs.Save();
            }
            
            Debug.Log($"プレイヤー情報読み込み: ID={playerId}, Name={playerName}");
            RPC_AddPlayer(playerId, playerName);
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        // プレイヤーが切断した時に自分の情報を削除
        if (Object.HasInputAuthority)
        {
            string playerId = PlayerPrefs.GetString("playerId", "");
            if (!string.IsNullOrEmpty(playerId))
            {
                RPC_RemovePlayer(playerId);
            }
        }
        
        base.Despawned(runner, hasState);
    }

    /// <summary>
    /// プレイヤーをリストに追加
    /// </summary>
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_AddPlayer(string playerId, string playerName)
    {
        if (PlayerCount >= MAX_PLAYERS)
        {
            Debug.LogWarning("ルームが満員です");
            return;
        }

        // 既に存在するかチェック
        for (int i = 0; i < PlayerCount; i++)
        {
            if (PlayerIds[i].ToString() == playerId)
            {
                Debug.LogWarning($"プレイヤーID {playerId} は既に存在します");
                return;
            }
        }

        // プレイヤーを追加
        PlayerIds.Set(PlayerCount, playerId);
        PlayerNames.Set(PlayerCount, playerName);
        PlayerCount++;
        
        Debug.Log($"プレイヤー追加: {playerName} (ID: {playerId}) - 現在のプレイヤー数: {PlayerCount}");
    }

    /// <summary>
    /// プレイヤーをリストから削除
    /// </summary>
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RemovePlayer(string playerId)
    {
        for (int i = 0; i < PlayerCount; i++)
        {
            if (PlayerIds[i].ToString() == playerId)
            {
                // 後続の要素を前に詰める
                for (int j = i; j < PlayerCount - 1; j++)
                {
                    PlayerIds.Set(j, PlayerIds[j + 1]);
                    PlayerNames.Set(j, PlayerNames[j + 1]);
                }
                
                PlayerCount--;
                Debug.Log($"プレイヤー削除: ID {playerId} - 現在のプレイヤー数: {PlayerCount}");
                return;
            }
        }
    }

    /// <summary>
    /// 全プレイヤーのデータを取得
    /// </summary>
    public List<PlayerData> GetAllPlayers()
    {
        List<PlayerData> players = new();
        for (int i = 0; i < PlayerCount; i++)
        {
            players.Add(new PlayerData(PlayerIds[i].ToString(), PlayerNames[i].ToString()));
        }
        return players;
    }
}
