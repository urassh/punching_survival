using UnityEngine;
using System;

public class PlayerId : MonoBehaviour
{
    //PlayerPrefに保存するUUIDのキー名
    public static string playerIdKey = "playerId";
    // UUIDの文字列(クラス外からは読み取り専用)
    public static string playerUuid { get; private set; }
    private void Start()
    {
        if (PlayerPrefs.HasKey(playerIdKey) == false)
        {
            CreateNewPlayerId();
        }
        else
        {
            playerUuid = PlayerPrefs.GetString(playerIdKey);
        }
        Debug.Log("PlayerId: " + PlayerPrefs.GetString(playerIdKey));
    }

    public void CreateNewPlayerId()
    {
        playerUuid = Guid.NewGuid().ToString();
        //playerPrefに保存する
        PlayerPrefs.SetString(playerIdKey, playerUuid);
        PlayerPrefs.Save();
    }

    //ResultSceneのボタン遷移機能に追加してPlayerIDを削除する関数
    public void CleanPlayerId()
    {
        playerUuid = null;
        //PlayerPlefから引数に対応するものを削除する
        PlayerPrefs.DeleteKey(playerIdKey);
        PlayerPrefs.Save();
    }
}
