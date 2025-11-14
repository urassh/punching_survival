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
    [Networked, Capacity(4)] 
    public NetworkArray<NetworkString<_32>> PlayerIds { get; }
    
    [Networked, Capacity(4)] 
    public NetworkArray<NetworkString<_32>> PlayerNames { get; }
    
    [Networked] public int PlayerCount { get; set; }
    
    private const int MAX_PLAYERS = 4;
    
    // プレイヤー削除完了のコールバック
    public Action OnPlayerRemoved;

    public override void Spawned()
    {   
        // シーン間で保持
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("PlayerInfoがSpawnされました");
        
        // ここでは各クライアントの情報追加は行わない
        // 別途AddCurrentPlayerメソッドを呼び出してもらう
    }
    
    /// <summary>
    /// 現在のクライアントの情報をPlayerInfoに追加する
    /// 各クライアントが個別に呼び出す必要がある
    /// </summary>
    public void AddCurrentPlayer()
    {
        string playerId = PlayerPrefs.GetString("playerId", "");
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");

        Debug.Log($"AddCurrentPlayer called for ID={playerId}, Name={playerName}");
        
        // PlayerIdまたはPlayerNameが空の場合は処理を中断
        if (string.IsNullOrEmpty(playerId) || string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("PlayerIdまたはPlayerNameが空のため、プレイヤー追加をスキップします");
            return;
        }
        
        Debug.Log($"プレイヤー情報読み込み: ID={playerId}, Name={playerName}");
        RPC_AddPlayer(playerId, playerName);
    }

    public void RemoveCurrentPlayer()
    {
        string playerId = PlayerPrefs.GetString("playerId", "");
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");

        Debug.Log($"RemoveCurrentPlayer called for ID={playerId}, Name={playerName}");

        // PlayerIdまたはPlayerNameが空の場合は処理を中断
        if (string.IsNullOrEmpty(playerId) || string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("PlayerIdまたはPlayerNameが空のため、プレイヤー追加をスキップします");
            return;
        }

        Debug.Log($"プレイヤー情報読み込み: ID={playerId}, Name={playerName}");
        RPC_RemovePlayer(playerId, playerName);
    }

    /// <summary>
    /// プレイヤーをリストに追加
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AddPlayer(string playerId, string playerName)
    {
        // PlayerIdまたはPlayerNameが空の場合は追加しない
        if (string.IsNullOrEmpty(playerId) || string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("PlayerIdまたはPlayerNameが空のため、プレイヤー追加をスキップします");
            return;
        }
        
        if (PlayerCount >= MAX_PLAYERS)
        {
            Debug.LogWarning("ルームが満員です");
            return;
        }

        // 既に存在するかチェック（PlayerIdの重複チェック）
        for (int i = 0; i < PlayerCount; i++)
        {
            if (PlayerIds[i].ToString() == playerId)
            {
                Debug.LogWarning($"プレイヤーID {playerId} は既に存在します。追加をスキップします");
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
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RemovePlayer(string playerId, string playerName)
    {
        // PlayerIdまたはPlayerNameが空の場合は削除しない
        if (string.IsNullOrEmpty(playerId) || string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("PlayerIdまたはPlayerNameが空のため、プレイヤー削除をスキップします");
            return;
        }

        // 削除対象のプレイヤーを検索
        int removeIndex = -1;
        for (int i = 0; i < PlayerCount; i++)
        {
            if (PlayerIds[i].ToString() == playerId)
            {
                removeIndex = i;
                break;
            }
        }

        // プレイヤーが見つからない場合
        if (removeIndex == -1)
        {
            Debug.LogWarning($"プレイヤーID {playerId} が見つかりません。削除をスキップします");
            return;
        }

        // 配列を詰める（削除したインデックス以降を前に移動）
        for (int i = removeIndex; i < PlayerCount - 1; i++)
        {
            PlayerIds.Set(i, PlayerIds[i + 1]);
            PlayerNames.Set(i, PlayerNames[i + 1]);
        }

        // 最後の要素をクリア
        PlayerIds.Set(PlayerCount - 1, "");
        PlayerNames.Set(PlayerCount - 1, "");
        
        // プレイヤー数を減らす
        PlayerCount--;
        
        Debug.Log($"プレイヤー削除: {playerName} (ID: {playerId}) - 現在のプレイヤー数: {PlayerCount}");
        
        // 削除完了をコールバックで通知
        OnPlayerRemoved?.Invoke();
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
