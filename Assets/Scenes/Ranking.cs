using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranking : MonoBehaviour
{
    Dictionary<int, string> playerNames = new Dictionary<int, string>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void RPC_RegisterPlayer(int playerId, string playerName)
    {
        playerNames[playerId] = playerName;
    }

    void RPC_SetDropPlayerRank(string playerId)
    {

    }
    void RPC_SetSurvivedPlayerRank(string playerId)
    {

    } 

    void RPC_ResetRanking()
    {

    }
}
