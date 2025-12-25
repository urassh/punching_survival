using Fusion;
using UnityEngine;

public class GameTimer : NetworkBehaviour
{
    [SerializeField] private int limitSeconds = 180;

    [Networked] private int StartTick { get; set; }

    public bool IsInitialized => StartTick > 0;

    public override void Spawned()
    {
        // Shared Mode：StateAuthority を持つクライアントが初期化
        if (Object.HasStateAuthority)
            StartTick = Runner.Tick;
        
    }

    public float RemainingTime
    {
        get
        {
            if (!IsInitialized)
                return limitSeconds; // 未同期中は初期表示だけ

            float elapsed = (Runner.Tick - StartTick) * Runner.DeltaTime;

            return Mathf.Max(0f, limitSeconds - elapsed);
        }
    }
}




