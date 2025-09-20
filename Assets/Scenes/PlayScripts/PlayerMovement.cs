using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.TextCore.Text;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
	// 自分自身（åローカルプレイヤー）のインスタンスを保持するstatic変数
    public static PlayerMovement Local { get; private set; }
    private Rigidbody _rb;

    [SerializeField] private float PlayerSpeed = 50f;
    [SerializeField] private Camera Camera;
	private Animator anim;
	[Networked] private TickTimer KnockbackTimer { get; set; }
	[SerializeField] private float freezeTime = 2f; 

    private void OnCollisionEnter(Collision collision)
	{
		Debug.Log("Player OnCollisionEnter");
		// "Bullet" タグを持つオブジェクトに衝突した場合
		if (collision.gameObject.CompareTag("Bullet"))
		{
			Debug.Log("Player is hit by a Bullet");
			Debug.Log(Runner);
			KnockbackTimer = TickTimer.CreateFromSeconds(Runner, freezeTime);
			Debug.Log($"KnockbackTimer:{KnockbackTimer}");
		}
	}

    public override void Spawned()
	{
		if (HasInputAuthority)
		{
			Local = this; // static変数に自分を登録
		}
		if (HasStateAuthority)
		{
			Camera = Camera.main;
			Camera.GetComponent<ThirdPersonCamera>().Target = transform;
		}
	}

	private void Awake()
	{
		_rb = GetComponent<Rigidbody>();
		_rb.constraints = RigidbodyConstraints.FreezeRotation;
		_rb.useGravity = true;
		anim = GetComponent<Animator>();
    }

	public override void FixedUpdateNetwork()
	{
		if (transform.position.y < -10)
		{
			Debug.Log("Died");
			//Objectを消す
			Runner.Despawn(Object);
			//Playerカメラを特定の位置に切り替える
			Camera.transform.position = new Vector3(0, 20, 0);
			Camera.transform.LookAt(new Vector3(0, 0, 0));
			GameObject.Find("UICanvas").SetActive(false);
			string playerId = PlayerPrefs.GetString(PlayerId.playerIdKey);
			Ranking ranking = FindObjectOfType<Ranking>();
			ranking.RPC_SetDropPlayerRank(playerId);
		}
		// ノックバックタイマーが作動中なら、移動処理をすべてスキップ
		if (KnockbackTimer.ExpiredOrNotRunning(Runner) == false)
		{
			return;
		}

		Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
		Vector3 camForward = cameraRotationY * Vector3.forward;
		Vector3 camRight = cameraRotationY * Vector3.right;

		Vector3 moveDir = (camForward * JoyStickMovement.JoyStickPositionY +
						   camRight * JoyStickMovement.JoyStickPositionX).normalized;
		Vector3 targetVelocity = moveDir * PlayerSpeed;

		anim.SetFloat("Speed", targetVelocity.magnitude, 0.1f, Runner.DeltaTime);
		Vector3 velocityChange = targetVelocity - new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
		_rb.AddForce(new Vector3(velocityChange.x, 0, velocityChange.z), ForceMode.VelocityChange);

		if (moveDir != Vector3.zero)
			transform.forward = moveDir;
    }
}
