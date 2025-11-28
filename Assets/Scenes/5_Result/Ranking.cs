using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

/// <summary>
/// プレイヤーのランキング情報を管理するデータ構造
/// Rank = 0: 生存中, Rank > 0: 脱落済み（順位）
/// </summary>
[System.Serializable]
public struct PlayerRankingData : INetworkStruct
{
        [Networked] public NetworkString<_16> PlayerId { get; set; }
        [Networked] public NetworkString<_64> PlayerName { get; set; }
        [Networked] public int Rank { get; set; }

        public PlayerRankingData(string id, string name)
        {
            PlayerId = id;
            PlayerName = name;
            Rank = 0;
        }
}

/// <summary>
/// 2~4人プレイゲームのランキング機能を管理するクラス
/// NetworkObjectとして動的にスポーンされる
/// </summary>
public class Ranking : NetworkBehaviour
{
    public readonly Dictionary<string, PlayerRankingData> playerData = new();
    public static Ranking Instance { get; private set; }
    private Action onDroppedPlayer;

    public override void Spawned()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    /// <summary>
    /// プレイヤーをランキングに登録（RPC版）
    /// </summary>
    /// <param name="playerId">プレイヤーID</param>
    /// <param name="playerName">プレイヤー名</param>
    public void RegisterPlayer(string playerId, string playerName)
    {
        if (playerData.ContainsKey(playerId))
            return ;
        playerData[playerId] = new PlayerRankingData(playerId, playerName);
    }

    /// <summary>
    /// 生存中（Rank = 0）のプレイヤー数を取得
    /// </summary>
    public int GetSurvivingPlayersCount()
    {
        return playerData.Values.Count(p => p.Rank == 0);
    }

    public void SetOnDroppedPlayerCallback(Action callback)
    {
        onDroppedPlayer = callback;
    }

    /// <summary>
    /// プレイヤーを脱落ランクに設定（RPC版）
    /// </summary>
    /// <param name="playerId">脱落したプレイヤーのID</param>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetDropPlayerRank(string playerId)
    {
        Debug.Log("RPC_SetDropPlayerRank called for playerId: " + playerId);
        Debug.Log($"playerData contains keys: {string.Join(", ", playerData.Keys)}");
        if (playerData.ContainsKey(playerId))
        {
            var player = playerData[playerId];
            Debug.Log($"Setting rank for player {player.PlayerName} (ID: {playerId})");
            player.Rank = GetSurvivingPlayersCount(); // 脱落ランクを設定
            playerData[playerId] = player;
            onDroppedPlayer?.Invoke();
        }
    }

    /// <summary>
    /// 残ったプレイヤー数が1人の場合、そのプレイヤーを1位に設定（RPC版）
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SetLastPlayerAsFirstRank()
    {
        var survivingPlayers = playerData.Values.Where(p => p.Rank == 0).ToList();
        if (survivingPlayers.Count == 1)
        {
            var lastPlayer = survivingPlayers[0];
            lastPlayer.Rank = 1;
            playerData[lastPlayer.PlayerId.ToString()] = lastPlayer;
        }
    }

    /// <summary>
    /// ランキングの集計が完了したかどうかを確認
    /// </summary>
    /// <returns>集計完了ならtrue、未完了ならfalse</returns>
    public bool IsRankingComplete()
    {
        if (playerData.Count == 0) return false;

        if (GetSurvivingPlayersCount() == 1)
            RPC_SetLastPlayerAsFirstRank();

        return playerData.Values.All(p => p.Rank > 0);
    }

    /// <summary>
    /// 現在のランキングリストを取得
    /// </summary>
    /// <returns>ランキングリスト</returns>
    public List<PlayerRankingData> GetRankingList()
    {
        /// ダミーのランキングリスト
        /// uuid(String), name, rank
        List<PlayerRankingData> dummyList = new List<PlayerRankingData>
        {
            new(System.Guid.NewGuid().ToString(), "うらっしゅ") { Rank = 2 },
            new(System.Guid.NewGuid().ToString(), "じく") { Rank = 1 },
            new(System.Guid.NewGuid().ToString(), "かわみー") { Rank = 3 },
            new(System.Guid.NewGuid().ToString(), "はる") { Rank = 4 }
        };
        return dummyList;
    }

    // /// <summary>
    // /// ランキングを順位順で取得（クライアント側用）
    // /// </summary>
    // public List<PlayerRankingData> GetSortedRankingList()
    // {
    //     return rankingList.OrderBy(p => p.Rank).ToList();
    // }

}
