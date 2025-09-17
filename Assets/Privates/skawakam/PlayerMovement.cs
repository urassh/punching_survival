using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.TextCore.Text;

public class PlayerMovement : NetworkBehaviour
{
	private bool _jumpPressed;
	private Vector3 _velocity;
	private CharacterController _controller;
	public float PlayerSpeed = 2f;
	public float JumpForce = 5f;
	public float GravityValue = -9.81f;
	public Camera Camera;

	public override void Spawned()
	{
		if (HasStateAuthority)
		{
			Camera = Camera.main;
			Camera.GetComponent<FirstPersonCamera>().Target = transform;
		}
	}

	private void Awake()
	{
		_controller = GetComponent<CharacterController>();
	}

	void Update()
	{
		if (Input.GetButtonDown("Jump"))
		{
			_jumpPressed = true;
		}
	}

	public override void FixedUpdateNetwork()
	{
		if (_controller.isGrounded)
		{
			_velocity = new Vector3(0, -1, 0);
		}

		Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);

		// カメラ基準の前・右を作る
		Vector3 camForward = cameraRotationY * Vector3.forward;
		Vector3 camRight   = cameraRotationY * Vector3.right;

		// 入力方向をカメラ基準で計算
		Vector3 moveDir = (camForward * JoyStickMovement.JoyStickPositionY +
						camRight * JoyStickMovement.JoyStickPositionX).normalized;

		// 移動処理
		Vector3 move = moveDir * Runner.DeltaTime * PlayerSpeed;

		_velocity.y += GravityValue * Runner.DeltaTime;
		if (_jumpPressed && _controller.isGrounded)
		{
			_velocity.y += JumpForce;
		}

		_controller.Move(move + _velocity * Runner.DeltaTime);

		// 入力があるときだけ向きを変える
		if (moveDir != Vector3.zero)
		{
			transform.forward = moveDir;
		}

		_jumpPressed = false;
	}
}
