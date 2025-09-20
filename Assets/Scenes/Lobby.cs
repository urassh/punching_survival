using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using System.Threading.Tasks;

public class Lobby : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    public RoomNumberText roomNumberText;
    private LobbyNetwork lobbyNetwork;

    private void Awake()
    {
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();

        if (runner == null || !runner.IsRunning)
        {
            //prefab(ゲームオブジェクトの設計書)を使ってゲームオブジェクトをコピーする
            runner = Instantiate(networkRunnerPrefab);
            lobbyNetwork = new(runner);
            int roomNumber = CreateRoomNum();

            // ゲーム終了まで保持する
            DontDestroyOnLoad(runner.gameObject);
            if (roomNumberText != null)
                roomNumberText.SetRoomNumber(roomNumber);
            lobbyNetwork.CreateRoom(roomNumber.ToString());
        }
        Debug.Log("ルームは生成されています。");
    }

    public void OnInputStart()
    {
        
    }

    public void OnInputCancel()
    {
        lobbyNetwork.LeaveRoom();

        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        if (runner != null)
            Destroy(runner.gameObject);
        
        Scene.Start.LoadScene();
    }

    private int CreateRoomNum()
    {
        return Random.Range(1000, 10000);
    }
}
