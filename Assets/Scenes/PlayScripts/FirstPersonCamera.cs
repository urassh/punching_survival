using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
	public Transform Target;
	readonly Quaternion _BASE_ROTATION = Quaternion.Euler(90, 0, 0);

	void LateUpdate()
	{
		if (Target == null)
		{
			return;
		}
		transform.position = Target.position;
		if (!SystemInfo.supportsGyroscope)
		{
			return;
		}
		Quaternion gyro = Input.gyro.attitude;
		transform.localRotation = _BASE_ROTATION * new Quaternion(-gyro.x, -gyro.y, gyro.z, gyro.w);
		
	}
}
