using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class JoyStickMovement : MonoBehaviour
{
	public GameObject JoyStick;
	private RectTransform joyStickRectTransform;
	public GameObject Background;

	public int StickRange = 3;
	private int stickMovement = 0;

	public static float JoyStickPositionX;
	public static float JoyStickPositionY;

	// Start is called before the first frame update
	void Start()
	{
		Initialization();
	}

	// Update is called once per frame
	void Update()
	{

	}

	private void Initialization()
	{
		stickMovement = StickRange * (Screen.width + Screen.height) / 100;
		joyStickRectTransform = JoyStick.GetComponent<RectTransform>();
		JoyStickDisplay(false);
	}

	private void JoyStickDisplay(bool x)
	{
		Background.SetActive(x);
		JoyStick.SetActive(x);
	}

	public void PointerDown(BaseEventData data)
	{
		PointerEventData pointer = data as PointerEventData;
		JoyStickDisplay(true);
		Background.transform.position = pointer.position;
	}

	public void PointerUp(BaseEventData data)
	{
		PositionInitialization();
		JoyStickDisplay(false);
	}

	public void Move(BaseEventData data)
	{
		PointerEventData pointer = data as PointerEventData;
		float x = Background.transform.position.x - pointer.position.x;
		float y = Background.transform.position.y - pointer.position.y;
		float angle = Mathf.Atan2(y, x);

		if (UnityEngine.Vector2.Distance(Background.transform.position, pointer.position) > stickMovement)
		{
			y = stickMovement * Mathf.Sin(angle);
			x = stickMovement * Mathf.Cos(angle);
		}

		JoyStickPositionX = -x / stickMovement;
		JoyStickPositionY = -y / stickMovement;
		JoyStick.transform.position = new UnityEngine.Vector2(Background.transform.position.x - x, Background.transform.position.y - y);
	}

	public void PositionInitialization()
	{
		joyStickRectTransform.anchoredPosition = UnityEngine.Vector2.zero;
		JoyStickPositionX = 0;
		JoyStickPositionY = 0;
	}
}
