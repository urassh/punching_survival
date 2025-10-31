using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform Target;
	public float Distance = 8.0f;
	public Vector3 LookAtOffset = new(0, 6f, 0);

	// ジャイロの基準となる回転をリセットするためのオフセット
    private Quaternion _gyroOffset = Quaternion.identity;

    void Start()
    {
        // ジャイロセンサーが利用可能か確認し、有効にする
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            ResetGyroRotation(); // 起動時の向きを基準にする
        }
    }

	/// <summary>
	/// 現在のジャイロの向きを正面としてリセットする
	/// </summary>
	public void ResetGyroRotation()
	{
		if (SystemInfo.supportsGyroscope)
		{
			// 現在のジャイロの向きの逆回転を保存しておく
			_gyroOffset = Quaternion.Inverse(Input.gyro.attitude);
		}
	}

    void LateUpdate()
    {
        if (Target == null || !SystemInfo.supportsGyroscope)
        {
            return;
        }

        // 1. ジャイロの現在の回転を取得し、オフセットを適用して基準を補正
        Quaternion gyroRotation = _gyroOffset * Input.gyro.attitude;
        
        // 2. Unityの座標系に合わせて回転を調整
        gyroRotation = new Quaternion(gyroRotation.x, gyroRotation.y, -gyroRotation.z, -gyroRotation.w);

        // 3. 基準となるカメラの位置（キャラクターの真後ろ）を、ジャイロの回転で動かす
        Vector3 cameraDirection = gyroRotation * Vector3.back; // Vector3.backは(0, 0, -1)
        Vector3 desiredPosition = Target.position + cameraDirection * Distance;

        // 4. カメラの位置と向きをセット
        transform.position = desiredPosition;
        transform.LookAt(Target.position + LookAtOffset); // 常にターゲットの中心（少し上）を見る
    }
}
