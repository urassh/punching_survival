using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BulletSpawner : NetworkBehaviour
{
	public NetworkObject BulletPrefab;
	public Transform FirePoint;

	// Animatorコンポーネントへの参照
	private Animator _animator;

	// 射撃アニメーションのトリガー名（Animatorで設定した名前に合わせる）
	[SerializeField] private string shootTriggerName = "Fire";

	private void Awake()
	{
		// Animatorコンポーネントを取得
		_animator = GetComponent<Animator>();

		if (_animator == null)
		{
			Debug.LogError("BulletSpawner: Animatorコンポーネントが見つかりません！", this);
		}
		else
		{
			Debug.Log($"BulletSpawner: Animator取得成功。Controller: {_animator.runtimeAnimatorController?.name}", this);
		}
	}

	// ボタンが押された時などにUIから呼び出す
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
			// 射撃アニメーションをトリガー
			if (_animator != null)
			{
				Debug.Log($"BulletSpawner: アニメーショントリガー '{shootTriggerName}' を実行", this);
				_animator.SetTrigger(shootTriggerName);
			}
			else
			{
				Debug.LogWarning("BulletSpawner: Animatorが見つからないため、アニメーションは再生されません", this);
			}

			// 弾をネットワーク越しに直接生成する
			// 第4引数で、この弾の所有者（操作権限者）を自分に設定する
			Runner.Spawn(BulletPrefab,
						 FirePoint.position,
						 FirePoint.rotation * Quaternion.Euler(90, 0, 0),
						 Object.InputAuthority); // 撃ったプレイヤーが所有者となる
		}
	}
}
