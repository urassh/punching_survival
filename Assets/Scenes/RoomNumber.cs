using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomNumberText : MonoBehaviour
{
    public static int roomNum;
    public static string roomNumStr;
    private Text roomNumText;

    void Awake()
    {
        roomNumText = GetComponent<Text>();
    }
    public void SetRoomNumber()
    {
        if (CreateRoom.isHost && roomNumText != null)
        {
            CreateRoomNum();
            roomNumStr = roomNum.ToString();
            DisplayRoomNum();
        }
    }

    private void CreateRoomNum()
    {
        roomNum = Random.Range(1000, 10000);
        Debug.Log("Random : " + roomNum);
    }


    private void DisplayRoomNum()
    {
        this.roomNumText = this.GetComponent<Text>();
        this.roomNumText.text = roomNumStr;
    }
}
