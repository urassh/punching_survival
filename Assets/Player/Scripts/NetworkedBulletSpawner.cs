using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// ネットワーク同期された弾丸発射とアニメーション制御
/// NetworkMecanimAnimatorと連携して使用
/// </summary>
public class NetworkedBulletSpawner : NetworkBehaviour
{
	public NetworkObject BulletPrefab;
	public Transform FirePoint;

	// NetworkMecanimAnimatorコンポーネントへの参照
	private NetworkMecanimAnimator _networkAnimator;

	// 射撃アニメーションのトリガー名（Animatorで設定した名前）
	[SerializeField] private string shootTriggerName = "Fire";

	// 射撃アニメーションのトリガーハッシュ（パフォーマンス向上のため）
	private int _shootTriggerHash;

	private void Awake()
	{
		// NetworkMecanimAnimatorコンポーネントを取得
		_networkAnimator = GetComponent<NetworkMecanimAnimator>();

		// トリガー名をハッシュに変換（パフォーマンス向上）
		_shootTriggerHash = Animator.StringToHash(shootTriggerName);

		if (_networkAnimator == null)
		{
			Debug.LogError("NetworkedBulletSpawner: NetworkMecanimAnimatorコンポーネントが見つかりません！", this);
		}
		else if (_networkAnimator.Animator == null)
		{
			Debug.LogError("NetworkedBulletSpawner: NetworkMecanimAnimatorにAnimatorが設定されていません！", this);
		}
		else
		{
			Debug.Log($"NetworkedBulletSpawner: NetworkAnimator取得成功。Controller: {_networkAnimator.Animator.runtimeAnimatorController?.name}", this);
		}
	}

	/// <summary>
	/// 銃を発射する（UIやジェスチャーから呼び出される）
	/// </summary>
	public void Fire()
	{
		if (!HasInputAuthority)
		{
			Debug.Log("No Input Authority");
			return;
		}

		Debug.Log("BulletPrefab: " + (BulletPrefab != null));
		Debug.Log("FirePoint: " + (FirePoint != null));

		if (BulletPrefab != null && FirePoint != null)
		{
			// RPC経由でネットワーク同期された発射処理を呼び出す
			RPC_FireBullet(FirePoint.position, FirePoint.rotation);
		}
	}

	/// <summary>
	/// ネットワーク同期された発射処理
	/// StateAuthorityで実行し、全クライアントに同期
	/// </summary>
	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	private void RPC_FireBullet(Vector3 position, Quaternion rotation)
	{
		// 射撃アニメーションをトリガー（ネットワーク同期）
		if (_networkAnimator != null && _networkAnimator.Animator != null)
		{
			Debug.Log($"NetworkedBulletSpawner: アニメーショントリガー '{shootTriggerName}' (Hash: {_shootTriggerHash}) を実行", this);
			// NetworkMecanimAnimatorを使用してトリガーを設定
			_networkAnimator.Animator.SetTrigger(_shootTriggerHash);
		}
		else
		{
			Debug.LogWarning("NetworkedBulletSpawner: NetworkAnimatorまたはAnimatorが見つからないため、アニメーションは再生されません", this);
		}

		// 弾をネットワーク越しに生成
		Runner.Spawn(
			BulletPrefab,
			position,
			rotation * Quaternion.Euler(90, 0, 0),
			Object.InputAuthority // 撃ったプレイヤーが所有者
		);

		Debug.Log($"Bullet spawned at {position}");
	}
}
