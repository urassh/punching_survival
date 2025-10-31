using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : NetworkBehaviour
{
    [SerializeField] float BulletSpeed = 10f;
    [SerializeField] float LifeTime = 5f;

    private float _lifeTimer;
    private Rigidbody _rigidbody;

    /// <summary>
    /// オブジェクトがネットワーク上に生成された時に一度だけ呼ばれる
    /// </summary>
    public override void Spawned()
    {
        // 必要なコンポーネントを取得
        _rigidbody = GetComponent<Rigidbody>();

        // 初期設定
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _lifeTimer = LifeTime;
        _rigidbody.useGravity = false;

        // ▼▼▼ ここで初速を与える ▼▼▼
        // この弾は、自身の前方に、設定された速度で飛んでいく
        // x軸90度回転を考慮して、本来の前方向（transform.up）を使用
        _rigidbody.AddForce(transform.up * BulletSpeed, ForceMode.VelocityChange);
    }

    /// <summary>
    /// 物理演算のタイミングで定期的に呼ばれる
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        // 時間経過で消滅する処理
        _lifeTimer -= Runner.DeltaTime;
        if (_lifeTimer <= 0)
        {
            Debug.Log("Bullet will Despawn");
            // オブジェクトが有効な場合のみDespawnを呼ぶ
            if (Object != null && Object.IsValid)
            {
                Runner.Despawn(Object);
            }
        }
    }

    /// <summary>
    /// 何かに衝突した時に呼ばれる
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        // プレイヤーに衝突した場合はノックバック処理を実行
        PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
        if (player != null)
        {
            // 弾の移動方向を取得（ノックバック方向として使用）
            Vector3 knockbackDirection = transform.up.normalized;
            
            // プレイヤーにノックバックを適用
            player.ApplyKnockback(knockbackDirection);
        }

        // 衝突したら自身は消滅する
        if (Object != null && Object.IsValid)
        {
            Runner.Despawn(Object);
        }
    }
}
