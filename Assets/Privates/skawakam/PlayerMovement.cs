using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
    private Rigidbody _rb;

    public float PlayerSpeed = 50f;
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

        _rb.AddForce(new Vector3(velocityChange.x, 0, velocityChange.z), ForceMode.VelocityChange);

        if (moveDir != Vector3.zero)
            transform.forward = moveDir;
    }
}
