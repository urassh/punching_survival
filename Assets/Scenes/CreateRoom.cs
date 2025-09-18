using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class CreateRoom : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    public static bool isHost = false;

    private async void Awake()
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        if (runner == null || !runner.IsRunning)
        {
            isHost = true;
            //prefab(ゲームオブジェクトの設計書)を使ってゲームオブジェクトをコピーする
            runner = Instantiate(networkRunnerPrefab);
            // ゲーム終了まで保持する
            DontDestroyOnLoad(runner.gameObject);
            var roomNumberText = FindObjectOfType<RoomNumberText>();
            if (roomNumberText != null)
            {
                roomNumberText.SetRoomNumber();
            }
            //部屋を作る
            var result = await runner.StartGame(new StartGameArgs
            {
                SessionName = RoomNumberText.roomNumStr,
                GameMode = GameMode.Shared
            });
            Debug.Log(result);
            Debug.Log($"生成したルームの名前:{runner.SessionInfo.Name}");
        }
        Debug.Log("ルームは生成されています。");
    }
}
