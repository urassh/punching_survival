using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class RoomNumberText : MonoBehaviour
{
    public static int roomNum;
    private Text roomNumText;

    void Awake()
    {
        roomNumText = GetComponent<Text>();
    }
    public void SetRoomNumber(int roomNumber)
    {
        roomNumText.text = roomNumber.ToString();
    }
}
