using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.TextCore.Text;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
	    // 自分自身（ローカルプレイヤー）のインスタンスを保持するstatic変数
    public static PlayerMovement Local { get; private set; }
    private Rigidbody _rb;

    [SerializeField] private float PlayerSpeed = 50f;
    [SerializeField] private Camera Camera;

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
    }

    public override void FixedUpdateNetwork()
    {
        Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
        Vector3 camForward = cameraRotationY * Vector3.forward;
        Vector3 camRight   = cameraRotationY * Vector3.right;

        Vector3 moveDir = (camForward * JoyStickMovement.JoyStickPositionY +
                           camRight   * JoyStickMovement.JoyStickPositionX).normalized;
        Vector3 targetVelocity = moveDir * PlayerSpeed;
        Vector3 velocityChange = targetVelocity - new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
		Debug.Log(moveDir);
		_rb.AddForce(new Vector3(velocityChange.x, 0, velocityChange.z), ForceMode.VelocityChange);

        if (moveDir != Vector3.zero)
            transform.forward = moveDir;
    }
}
