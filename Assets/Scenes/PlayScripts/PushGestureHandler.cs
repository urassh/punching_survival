using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushGestureHandler : MonoBehaviour
{
    public float PushThreshold = 1.5f;
    public float CooldownTime = 1.0f;
    private float _cooldownTimer;

    void Update()
    {
        // クールダウンタイマーを減らす
        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;
        }

        // クールダウン中でなければ、ジェスチャーを検知する
        if (_cooldownTimer <= 0)
        {
            // Z軸（前後方向）の加速度を取得
            float forwardAcceleration = Input.acceleration.z;

            // 加速度がしきい値を超えたかチェック
            if (forwardAcceleration > PushThreshold)
            {
                // --- ジェスチャーを検知！ ---
                OnPushForwardDetected();

                // クールダウンタイマーをリセット
                _cooldownTimer = CooldownTime;
            }
        }
    }

    /// <summary>
    /// 前に押し出すジェスチャーが検知された時に呼ばれるメソッド
    /// </summary>
    private void OnPushForwardDetected()
    {
        Debug.Log("「前に押し出す」ジェスチャーを検知しました！");
        // PlayerMovement.Local を使って、自分のプレイヤーインスタンスを探す
        // （PlayerCharacter.Localでも同じ考え方です）
        if (PlayerMovement.Local != null)
        {
            // 自分のプレイヤーにアタッチされているBulletSpawnerコンポーネントを取得
            BulletSpawner spawner = PlayerMovement.Local.GetComponent<BulletSpawner>();
            if (spawner != null)
            {
                // そのFire()メソッドを呼び出す
                spawner.Fire();
            }
        }
        else
        {
            Debug.LogError("自分のプレイヤーが見つかりません！ PlayerMovement.Localが設定されていません。");
        }
    }
}
