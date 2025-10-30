using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 加速度センサーで前に押し出すジェスチャーを検知して銃を撃つ
/// オプション：無効にしてUIボタンのみでも使用可能
/// </summary>
public class PushGestureHandler : MonoBehaviour
{
    [Header("ジェスチャー設定")]
    [SerializeField] private bool enableGesture = true; // ジェスチャー検知を有効にするか
    public float PushThreshold = 1.5f;
    public float CooldownTime = 1.0f;
    private float _cooldownTimer;
    
    [Header("オーディオ設定")]
    // 銃声のオブジェクトをとる
    [SerializeField] public AudioSource gunShotSoundSource;
    [SerializeField] public AudioClip gunShotSoundClip;
    private Animator anim;

    void Start()
    {
        _cooldownTimer = 0;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // ジェスチャー検知が無効なら何もしない
        if (!enableGesture)
        {
            return;
        }

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

            // NetworkedBulletSpawnerがあればそちらを優先
            NetworkedBulletSpawner networkedSpawner = PlayerMovement.Local.GetComponent<NetworkedBulletSpawner>();

            if (networkedSpawner != null)
            {
                //銃弾の音を鳴らす
                // NetworkedBulletSpawnerのFire()メソッドを呼び出す
                networkedSpawner.Fire();
                gunShotSoundSource.PlayOneShot(gunShotSoundClip);
            }
            else if (spawner != null)
            {
                //銃弾の音を鳴らす
                // そのFire()メソッドを呼び出す
                spawner.Fire();
                anim.SetTrigger("Fire");
                gunShotSoundSource.PlayOneShot(gunShotSoundClip);
            }
        }
        else
        {
            Debug.LogError("自分のプレイヤーが見つかりません！ PlayerMovement.Localが設定されていません。");
        }
    }
}
