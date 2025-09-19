using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BulletSpawner : NetworkBehaviour
{
	public NetworkObject BulletPrefab;
	public Transform FirePoint;

	public void OnFireButtonPressed()
	{
		if (!HasInputAuthority) return;
		if (Runner != null && BulletPrefab != null && FirePoint != null)
		{
			Runner.Spawn(BulletPrefab, FirePoint.position, FirePoint.rotation, null, null);
		}
	}
}
