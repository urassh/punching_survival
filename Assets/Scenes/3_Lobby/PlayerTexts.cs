using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerTexts : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds1_0 = new WaitForSeconds(1.0f);
    [Header("プレイヤーテキストUI")]
    public Text player1Text;
    public Text player2Text;
    public Text player3Text;
    public Text player4Text;
    public GameObject player1Model;
    public GameObject player2Model;
    public GameObject player3Model;
    public GameObject player4Model;
    
    private Text[] playerTexts;
    private GameObject[] playerModels;
    private PlayerInfo playerInfo;
    private Coroutine updateCoroutine;
    
    public void ActivatePlayerTexts()
    {
        // プレイヤーテキストの配列を初期化
        playerTexts = new Text[] { player1Text, player2Text, player3Text, player4Text };
        playerModels = new GameObject[] { player1Model, player2Model, player3Model, player4Model };
        // 全てのテキストを初期状態では非アクティブにする
        foreach (var text in playerTexts)
        {
            if (text != null)
            {
                text.gameObject.SetActive(false);
            }
        }
        
        // 1秒おきにプレイヤー情報を更新するコルーチンを開始
        updateCoroutine = StartCoroutine(UpdatePlayerTextsCoroutine());
    }
    
    private void OnDestroy()
    {
        // コルーチンを停止
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
        }
    }
    
    /// <summary>
    /// 1秒おきにプレイヤー情報を更新するコルーチン
    /// </summary>
    private IEnumerator UpdatePlayerTextsCoroutine()
    {
        while (true)
        {
            UpdatePlayerTexts();
            yield return _waitForSeconds1_0;
        }
    }
    
    /// <summary>
    /// PlayerInfoからプレイヤー情報を取得してテキストを更新
    /// </summary>
    private void UpdatePlayerTexts()
    {
        // PlayerInfoオブジェクトを検索（毎回検索することで、後から生成されるケースにも対応）
        if (playerInfo == null)
        {
            playerInfo = FindObjectOfType<PlayerInfo>();
        }
        
        if (playerInfo == null)
        {
            // PlayerInfoが見つからない場合は全てのテキストを非アクティブに
            SetAllTextsInactive();
            return;
        }
        
        // プレイヤーリストを取得
        List<PlayerData> players = playerInfo.GetAllPlayers();
        
        // 各プレイヤーテキストを更新
        for (int i = 0; i < playerTexts.Length; i++)
        {
            if (playerTexts[i] == null) continue;
            
            if (i < players.Count)
            {
                // プレイヤーが存在する場合：テキストをアクティブにして名前を表示
                playerTexts[i].gameObject.SetActive(true);
                playerModels[i].SetActive(true);
                playerTexts[i].text = $"{i + 1}. {players[i].playerName}";
                Debug.Log($"Player{i + 1}Text updated: {players[i].playerName}");
            }
            else
            {
                // プレイヤーが存在しない場合：テキストを非アクティブに
                playerTexts[i].gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// 全てのプレイヤーテキストを非アクティブにする
    /// </summary>
    private void SetAllTextsInactive()
    {
        foreach (var text in playerTexts)
        {
            if (text != null)
            {
                text.gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// 手動でプレイヤーテキストを更新する（デバッグ用）
    /// </summary>
    public void ForceUpdatePlayerTexts()
    {
        UpdatePlayerTexts();
    }
}
