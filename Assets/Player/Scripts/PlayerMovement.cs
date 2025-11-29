using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.TextCore.Text;
using Unity.VisualScripting;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : NetworkBehaviour
{
	// 自分自身（ローカルプレイヤー）のインスタンスを保持するstatic変数
    public static PlayerMovement Local { get; private set; }
    private Rigidbody _rb;

    [SerializeField] private float PlayerSpeed = 50f;
    [SerializeField] private Camera Camera;
	private Animator anim;
	[SerializeField] private float freezeTime = 2f;
	
	// ノックバック関連の変数
	[SerializeField] private float knockbackForce = 15f;
	[SerializeField] private float knockbackDuration = 0.5f;
	private bool isKnockedBack = false; 
	private bool isDead = false;

    private void OnCollisionEnter(Collision collision)
	{
		Debug.Log("Player OnCollisionEnter");
		// "Bullet" タグを持つオブジェクトに衝突した場合
		if (collision.gameObject.CompareTag("Bullet"))
		{
			Debug.Log("Player is hit by a Bullet");
			Debug.Log(Runner);
		}
	}

    public override void Spawned()
	{
		Debug.Log(
			$"[{name}] Spawned | " +
			$"StateAuthority={Object.StateAuthority}, " +
			$"HasStateAuthority={HasStateAuthority}, " +
			$"InputAuthority={Object.InputAuthority}, " +
			$"HasInputAuthority={HasInputAuthority}, " +
			$"IsMasterClient={Runner.IsSharedModeMasterClient}"
		);
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
		if (isDead)
			return ;

		if (transform.position.y < -10)
		{
			Debug.Log("Died");
			//Playerカメラを特定の位置に切り替える
			Camera.transform.position = new Vector3(0, 20, 0);
			Camera.transform.LookAt(new Vector3(0, 0, 0));
			GameObject.Find("UICanvas").SetActive(false);
			string playerId = PlayerPrefs.GetString(PlayerId.playerIdKey);
			Play play = FindObjectOfType<Play>();
			play.OnDropMe(playerId);
			isDead = true;
			return ;
		}

		// ノックバック中は通常の移動処理をスキップ
		if (isKnockedBack)
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

	/// <summary>
	/// ノックバック処理（弾丸から呼び出される）
	/// </summary>
	public void ApplyKnockback(Vector3 knockbackDirection)
	{
		Debug.Log(
			$"[{name}] ApplyKnockback called | " +
			$"StateAuthority={Object.StateAuthority}, " +
			$"HasStateAuthority={HasStateAuthority}, " +
			$"InputAuthority={Object.InputAuthority}, " +
			$"HasInputAuthority={HasInputAuthority}, " +
			$"IsMasterClient={Runner.IsSharedModeMasterClient}"
		);
		if (Object != null && Object.HasStateAuthority)
		{
			RPC_ApplyKnockback(knockbackDirection);
		}
	}

	/// <summary>
	/// ネットワーク同期されたノックバック処理
	/// </summary>
	[Rpc(RpcSources.All, RpcTargets.All)]
	public void RPC_ApplyKnockback(Vector3 knockbackDirection)
	{
		Debug.Log("RPC_ApplyKnockback");
		// 既にノックバック中の場合は処理しない
		if (isKnockedBack) return;

		isKnockedBack = true;
		// ノックバック方向を水平面に制限（Y軸は0にする）
		Vector3 horizontalKnockback = new Vector3(knockbackDirection.x, 0, knockbackDirection.z).normalized;

		// 現在位置から目標位置を計算
		Vector3 startPosition = transform.position;
		Vector3 targetPosition = startPosition + horizontalKnockback * knockbackForce;

		// DoTweenを使用してスムーズなノックバック移動
		transform.DOMove(targetPosition, knockbackDuration)
			.SetEase(Ease.OutCubic)
			.SetUpdate(UpdateType.Fixed)
			.OnComplete(() => {
				Debug.Log("knockback completed!");
				isKnockedBack = false;
			});

		// ノックバック中のアニメーション
		anim.SetFloat("Speed", 0f);
	}
}
