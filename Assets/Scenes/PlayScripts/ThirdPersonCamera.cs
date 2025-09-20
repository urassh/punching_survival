using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform Target;
    public Vector3 offset = new Vector3(0, 4.0f, -5.0f);
    public float smoothSpeed = 5.0f;
    readonly Quaternion _BASE_ROTATION = Quaternion.Euler(90, 0, 0);

    void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        Vector3 desiredPosition = Target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // ジャイロセンサーの向きを適用
        if (SystemInfo.supportsGyroscope)
        {
            Quaternion gyro = Input.gyro.attitude;
            // カメラをジャイロの向きに合わせる
            transform.localRotation = _BASE_ROTATION * new Quaternion(-gyro.x, -gyro.y, gyro.z, gyro.w);
        }
        else
        {
            // ジャイロが非対応の場合は、これまで通りターゲットを向く
            transform.LookAt(Target);
        }
    }
}
