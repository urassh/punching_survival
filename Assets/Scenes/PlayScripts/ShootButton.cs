using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 射撃ボタンのUI制御
/// UIボタンをタップすると銃を撃つ
/// </summary>
public class ShootButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("射撃設定")]
    [SerializeField] private float cooldownTime = 0.5f; // 連射制限時間
    private float _cooldownTimer = 0f;

    [Header("エフェクト設定")]
    [SerializeField] private Image buttonImage; // ボタンの画像コンポーネント
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color cooldownColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    [Header("オーディオ")]
    [SerializeField] private AudioSource gunShotSoundSource;
    [SerializeField] private AudioClip gunShotSoundClip;

    private bool _isPressed = false;

    private void Start()
    {
        // ボタン画像が未設定の場合、自動取得
        if (buttonImage == null)
        {
            buttonImage = GetComponent<Image>();
        }

        // 初期色を設定
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
    }

    private void Update()
    {
        // クールダウンタイマーを減らす
        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;

            // クールダウン中は色を変える
            if (buttonImage != null)
            {
                buttonImage.color = cooldownColor;
            }
        }
        else if (!_isPressed && buttonImage != null)
        {
            // クールダウン終了で通常色に戻す
            buttonImage.color = normalColor;
        }
    }

    /// <summary>
    /// ボタンが押された時の処理
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;

        // ボタンの色を変更
        if (buttonImage != null)
        {
            buttonImage.color = pressedColor;
        }

        // クールダウン中でなければ射撃
        if (_cooldownTimer <= 0)
        {
            FireBullet();
            _cooldownTimer = cooldownTime;
        }
    }

    /// <summary>
    /// ボタンが離された時の処理
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;

        // ボタンの色を戻す
        if (buttonImage != null && _cooldownTimer <= 0)
        {
            buttonImage.color = normalColor;
        }
    }

    /// <summary>
    /// 射撃処理を実行
    /// </summary>
    private void FireBullet()
    {
        Debug.Log("射撃ボタンが押されました！");

        // PlayerMovement.Localを使って自分のプレイヤーを取得
        if (PlayerMovement.Local != null)
        {
            // NetworkedBulletSpawnerがあればそちらを優先
            NetworkedBulletSpawner networkedSpawner = PlayerMovement.Local.GetComponent<NetworkedBulletSpawner>();

            if (networkedSpawner != null)
            {
                // ネットワーク同期版の射撃
                networkedSpawner.Fire();
                PlayGunSound();
            }
            else
            {
                // 通常版のBulletSpawnerを探す
                BulletSpawner spawner = PlayerMovement.Local.GetComponent<BulletSpawner>();
                if (spawner != null)
                {
                    spawner.Fire();
                    PlayGunSound();
                }
                else
                {
                    Debug.LogWarning("BulletSpawnerコンポーネントが見つかりません");
                }
            }
        }
        else
        {
            Debug.LogError("PlayerMovement.Localが設定されていません");
        }
    }

    /// <summary>
    /// 銃声を再生
    /// </summary>
    private void PlayGunSound()
    {
        if (gunShotSoundSource != null && gunShotSoundClip != null)
        {
            gunShotSoundSource.PlayOneShot(gunShotSoundClip);
        }
    }

    /// <summary>
    /// 外部から射撃を呼び出す（オプション）
    /// </summary>
    public void OnShootButtonClick()
    {
        if (_cooldownTimer <= 0)
        {
            FireBullet();
            _cooldownTimer = cooldownTime;
        }
    }
}
