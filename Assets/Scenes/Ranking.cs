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
        [Networked] public NetworkString<_32> PlayerName { get; set; }
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
/// DontDestroyOnLoadによりシーン間で共有される
/// </summary>
public class Ranking : NetworkBehaviour
{
    private Dictionary<string, PlayerRankingData> playerData = new Dictionary<string, PlayerRankingData>();
    private List<PlayerRankingData> rankingList = new List<PlayerRankingData>();
    private int currentRank = 0;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }


    /// <summary>
    /// プレイヤーをランキングに登録（RPC版）
    /// </summary>
    /// <param name="playerId">プレイヤーID</param>
    /// <param name="playerName">プレイヤー名</param>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RegisterPlayer(string playerId, string playerName)
    {
        if (!playerData.ContainsKey(playerId))
        {
            playerData[playerId] = new PlayerRankingData(playerId, playerName);
        }
    }

    /// <summary>
    /// プレイヤーを脱落ランクに設定（RPC版）
    /// </summary>
    /// <param name="playerId">脱落したプレイヤーのID</param>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetDropPlayerRank(string playerId)
    {
        if (playerData.ContainsKey(playerId))
        {
            currentRank++;
            var player = playerData[playerId];
            player.Rank = currentRank;
            playerData[playerId] = player;
        }
    }

    /// <summary>
    /// プレイヤーを生存者ランク（1位）に設定（RPC版）
    /// </summary>
    /// <param name="playerId">生存したプレイヤーのID</param>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetSurvivedPlayerRank(string playerId)
    {
        for (int i = 0; i < rankingList.Count; i++)
        {
            if (rankingList[i].PlayerId.ToString() == playerId)
            {
                var player = rankingList[i];
                player.Rank = 1; // 1位
                rankingList[i] = player;
                break;
            }
        }
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

    /// <summary>
    /// ランキングをリセット
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ResetRanking()
    {
        rankingList.Clear();
    }
}
