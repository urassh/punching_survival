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
		// 自分に操作権限がなければ、ここで処理を中断する
		// これにより、他人のキャラクターから弾が発射されるのを防ぐ
		if (!HasInputAuthority)
		{
			Debug.Log("No Input Authority");
			return;
		}

		// 必要な参照がすべて設定されていることを確認
		Debug.Log("Runner: " + (Runner != null));
		Debug.Log("BulletPrefab: " + (BulletPrefab != null));
		Debug.Log("FirePoint: " + (FirePoint != null));
		if (Runner != null && BulletPrefab != null && FirePoint != null)
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
