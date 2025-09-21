using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBgm : MonoBehaviour
{
    private static bool isLoad = false;

    void Awake()
    {
        if (GameBgm.isLoad)
        {
            Destroy(this.gameObject);
            return;
        }
        GameBgm.isLoad = true;
        DontDestroyOnLoad(this.gameObject);
    }
}
