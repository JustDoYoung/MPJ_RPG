using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_Joystick : UI_Base
{
	enum GameObjects
	{
		JoystickBG,
		JoystickCursor,
	}

	private GameObject _background;
	private GameObject _cursor;
	private float _radius;
	private Vector2 _touchPos;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindObjects(typeof(GameObjects));

		_background = GetObject((int)GameObjects.JoystickBG);
		_cursor = GetObject((int)GameObjects.JoystickCursor);
		_radius = _background.GetComponent<RectTransform>().sizeDelta.y / 5;

		gameObject.BindEvent(OnPointerDown, type: Define.EUIEvent.PointerDown);
		gameObject.BindEvent(OnPointerUp, type: Define.EUIEvent.PointerUp);
		gameObject.BindEvent(OnDrag, type: Define.EUIEvent.Drag);

		return true;
	}

	#region Event
	public void OnPointerDown(PointerEventData eventData)
	{
		_background.transform.position = eventData.position;
		_cursor.transform.position = eventData.position;
		_touchPos = eventData.position;

		Managers.Game.JoystickState = EJoystickState.PointerDown;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		_cursor.transform.position = _touchPos;

		Managers.Game.MoveDir = Vector2.zero;
		Managers.Game.JoystickState = EJoystickState.PointerUp;
	}

	public void OnDrag(PointerEventData eventData)
	{
		Vector2 touchDir = (eventData.position - _touchPos);

		float moveDist = Mathf.Min(touchDir.magnitude, _radius);
		Vector2 moveDir = touchDir.normalized;
		Vector2 newPosition = _touchPos + moveDir * moveDist;
		_cursor.transform.position = newPosition;

		Managers.Game.MoveDir = moveDir;
		Managers.Game.JoystickState = EJoystickState.Drag;
	}
	#endregion
}
