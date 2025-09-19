using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Bullet : NetworkBehaviour
{
	public float BulletSpeed = 30f;
	public float LifeTime = 5f;
	public float KnocbackForce = 10f;

	private float _lifeTimer;
	private Rigidbody _rigidbody;


	public override void Spawned()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
		_rigidbody.velocity = transform.forward * BulletSpeed;
		_lifeTimer = LifeTime;
		_rigidbody.useGravity = false;
	}

	// Update is called once per frame
	public override void FixedUpdateNetwork()
	{
		_lifeTimer -= Runner.DeltaTime;
		if (_lifeTimer <= 0)
		{
			Runner.Despawn(Object);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		var targetRigidbody = collision.gameObject.GetComponent<Rigidbody>();
		if (targetRigidbody != null && targetRigidbody != _rigidbody)
		{
			Vector3 dir = (collision.transform.position - transform.position).normalized;
			targetRigidbody.AddForce(dir * KnocbackForce, ForceMode.Impulse);
		}
		Runner.Despawn(Object);
	}
}
