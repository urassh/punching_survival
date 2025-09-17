using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
	public Transform Target;

	void LateUpdate()
	{
		if (Target == null)
		{
			return;
		}

		transform.position = Target.position;
	}
}
