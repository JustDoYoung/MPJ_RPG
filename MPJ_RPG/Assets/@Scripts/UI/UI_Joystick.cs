using System;
using System.Collections;
using System.Collections.Generic;
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
        if (base.Init() == false) return false;

        BindObjects(typeof(GameObjects));
        _background = GetObject((int)GameObjects.JoystickBG);
        _cursor = GetObject((int)GameObjects.JoystickCursor);
        _radius = _background.GetComponent<RectTransform>().sizeDelta.y / 5;

        gameObject.BindEvent(OnPointerDown, EUIEvent.PointerDown);
        gameObject.BindEvent(OnPointerUp, EUIEvent.PointerUp);
        gameObject.BindEvent(OnDrag, EUIEvent.Drag);

        return true;
    }

    #region Event
    private void OnPointerDown(PointerEventData eventdata)
    {
        _background.transform.position = eventdata.position;
        _cursor.transform.position = eventdata.position;
        _touchPos = eventdata.position;

        Managers.Game.JoystickState = EJoystickState.PointerDown;
    }

    private void OnPointerUp(PointerEventData eventdata)
    {
        _cursor.transform.position = _touchPos;

        Managers.Game.MoveDir = Vector2.zero;
        Managers.Game.JoystickState = EJoystickState.PointerUp;
    }

    private void OnDrag(PointerEventData eventdata)
    {
        Vector2 delta = eventdata.position - _touchPos;

        float dist = Mathf.Min(delta.magnitude, _radius);
        Vector2 dir = delta.normalized;
        Vector2 newPos = _touchPos + dir * dist;

        _cursor.transform.position = newPos;

        Managers.Game.MoveDir = dir;
        Managers.Game.JoystickState = EJoystickState.Drag;
    }
    #endregion
}
