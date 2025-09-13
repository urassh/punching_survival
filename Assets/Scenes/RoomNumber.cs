using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomNumber : MonoBehaviour
{
    public static int roomNum;
    public string roomNumStr;
    private Text roomNumText;
    void Start()
    {
        CreateRoomNum();
        IntToString();
        DisplayRoomNum();
    }

    private void CreateRoomNum()
    {
        roomNum = Random.Range(1111, 10000);
        Debug.Log("Random : " + roomNum);
    }

    private void IntToString()
    {
        roomNumStr = roomNum.ToString();
        Debug.Log("roomNumStr= " + roomNumStr);
    }

    private void DisplayRoomNum()
    {
        this.roomNumText = this.GetComponent<Text>();
        this.roomNumText.text = roomNumStr;
    }
}
