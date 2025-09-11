using UnityEngine;
using System;

public class Uuid : MonoBehaviour
{
    public static Uuid Instance;

    // 現在のUUID
    public string currentUuid { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            //自分自身を登録する
            Instance = this;
            // UUIDのゲームオブジェクトをゲーム終了まで保持する
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        CreateNewPlayerId();
    }

    public void CreateNewPlayerId()
    {
        currentUuid = Guid.NewGuid().ToString();
        //playerPrefに保存する
        PlayerPrefs.SetString("PlayerId", currentUuid);
        PlayerPrefs.Save();
        Debug.Log("UUID: " + PlayerPrefs.GetString("PlayerId"));
    }

    //ResultSceneのボタン遷移機能に追加してPlayerIDを削除する関数
    public void CleanPlayerId()
    {
        currentUuid = null;
        //PlayerPlefから引数に対応するものを削除する
        PlayerPrefs.DeleteKey("PlayerId");
        PlayerPrefs.Save();
    }
}
