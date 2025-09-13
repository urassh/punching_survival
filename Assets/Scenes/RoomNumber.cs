using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomNumberText : MonoBehaviour
{
    public static int roomNum;
    public string roomNumStr;
    private Text roomNumText;
    void Start()
    {
        CreateRoomNum();
        roomNumStr = roomNum.ToString();
        DisplayRoomNum();
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
