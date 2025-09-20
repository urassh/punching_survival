using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DisplayResult : MonoBehaviour
{
    public GameObject score_object = null; // Textオブジェクト
    // 初期化
    IEnumerator Start () {
        while (Ranking.Instance == null)
        {
            yield return null;
        }
        Text score_text = score_object.GetComponent<Text>();

        if (Ranking.Instance != null)
        {
            var rankingList = Ranking.Instance.GetRankingList();
            var sortedRankingList = rankingList.OrderBy(p => p.Rank).ToList();
            string resultText = "";

            foreach (var player in sortedRankingList)
            {
                resultText += $"{player.Rank}位: {player.PlayerName}\n";
            }

            score_text.text = resultText;
            Debug.Log("ランキング表示:\n" + resultText);
        }
        else
        {
            score_text.text = "ランキングデータがありません。";
            Debug.LogWarning("Ranking.Instance が null です。");
        }
    }

    // 更新
    void Update()
    {
    }

}