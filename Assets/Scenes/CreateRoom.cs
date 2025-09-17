using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class CreateRoom : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    private async void Start()
    {
        //prefab(ゲームオブジェクトの設計書)を使ってゲームオブジェクトをコピーする
        var networkRunner = Instantiate(networkRunnerPrefab);
        //部屋を作る
        var result = await networkRunner.StartGame(new StartGameArgs
        {
            SessionName = RoomNumberText.roomNumStr,
            GameMode = GameMode.Shared
        });
        Debug.Log(result);
        Debug.Log($"SessionName:{networkRunner.SessionInfo.Name}");
    }

}
