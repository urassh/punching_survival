using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BulletSpawner : NetworkBehaviour
{
	public NetworkObject BulletPrefab;
	public Transform FirePoint;

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
			// 弾をネットワーク越しに直接生成する
			// 第4引数で、この弾の所有者（操作権限者）を自分に設定する
			Runner.Spawn(BulletPrefab,
						 FirePoint.position,
						 FirePoint.rotation,
						 Object.InputAuthority); // 撃ったプレイヤーが所有者となる
		}
	}
}
