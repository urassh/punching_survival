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
/// NetworkObjectとして動的にスポーンされる
/// </summary>
public class Ranking : NetworkBehaviour
{
    private Dictionary<string, PlayerRankingData> playerData = new Dictionary<string, PlayerRankingData>();
    public static Ranking Instance { get; private set; }

    public override void Spawned()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // 既にインスタンスが存在する場合は削除
            Runner.Despawn(Object);
        }
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
    /// 生存中（Rank = 0）のプレイヤー数を取得
    /// </summary>
    public int GetSurvivingPlayersCount()
    {
        return playerData.Values.Count(p => p.Rank == 0);
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
			Debug.Log(playerId);
            var player = playerData[playerId];
            player.Rank = GetSurvivingPlayersCount(); // 脱落ランクを設定
            playerData[playerId] = player;
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

}
