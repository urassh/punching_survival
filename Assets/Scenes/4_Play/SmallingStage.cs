using Fusion;
using UnityEngine;

public class SmallingStage : NetworkBehaviour
{
    [Header("Smalling Settings")]
    [SerializeField] private float smallingStartDelay = 5f;
    [SerializeField] private float smallingDuration = 180f;
    [SerializeField] private float finalScaleRatio = 0.25f;

    private Vector3 initialScale;
    private Vector3 finalScale;

    [Networked] private TickTimer smallingTimer { get; set; }

    public override void Spawned()
    {
        initialScale = transform.localScale;
        finalScale = initialScale * finalScaleRatio;

        if (Object.HasStateAuthority)
        {
            // 「smallingStartDelay秒後から、smallingDuration秒間有効なタイマー」
            smallingTimer = TickTimer.CreateFromSeconds(
                Runner,
                smallingStartDelay + smallingDuration
            );
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        if (!smallingTimer.IsRunning)
            return;

        // 縮小開始前
        float remaining = smallingTimer.RemainingTime(Runner).Value;

        if (remaining > smallingDuration)
            return;

        // 経過時間 = 全体 - 残り
        float elapsed = smallingDuration - remaining;

        float t = Mathf.Clamp01(elapsed / smallingDuration);

        transform.localScale = Vector3.Lerp(
            initialScale,
            finalScale,
            t
        );
    }
}


