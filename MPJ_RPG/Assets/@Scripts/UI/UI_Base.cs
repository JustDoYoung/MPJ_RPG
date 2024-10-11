using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Base : InitBase
{
	protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

	private void Awake()
	{
		Init();
	}

	protected void Bind<T>(Type type) where T : UnityEngine.Object
	{
		string[] names = Enum.GetNames(type);
		UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
		_objects.Add(typeof(T), objects);

		for (int i = 0; i < names.Length; i++)
		{
			if (typeof(T) == typeof(GameObject))
				objects[i] = Util.FindChild(gameObject, names[i], true);
			else
				objects[i] = Util.FindChild<T>(gameObject, names[i], true);

			if (objects[i] == null)
				Debug.Log($"Failed to bind({names[i]})");
		}
	}

	protected void BindObjects(Type type) { Bind<GameObject>(type); }
	protected void BindImages(Type type) { Bind<Image>(type); }
	protected void BindTexts(Type type) { Bind<TMP_Text>(type); }
	protected void BindButtons(Type type) { Bind<Button>(type); }
	protected void BindToggles(Type type) { Bind<Toggle>(type); }
	protected void BindSliders(Type type) { Bind<Slider>(type); }

	protected T Get<T>(int idx) where T : UnityEngine.Object
	{
		UnityEngine.Object[] objects = null;
		if (_objects.TryGetValue(typeof(T), out objects) == false)
			return null;

		return objects[idx] as T;
	}

	protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
	protected TMP_Text GetText(int idx) { return Get<TMP_Text>(idx); }
	protected Button GetButton(int idx) { return Get<Button>(idx); }
	protected Image GetImage(int idx) { return Get<Image>(idx); }
	protected Toggle GetToggle(int idx) { return Get<Toggle>(idx); }

	public static void BindEvent(GameObject go, Action<PointerEventData> action = null, Define.EUIEvent type = Define.EUIEvent.Click)
	{
		UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

		switch (type)
		{
			case Define.EUIEvent.Click:
				evt.OnClickHandler -= action;
				evt.OnClickHandler += action;
				break;
			case Define.EUIEvent.PointerDown:
				evt.OnPointerDownHandler -= action;
				evt.OnPointerDownHandler += action;
				break;
			case Define.EUIEvent.PointerUp:
				evt.OnPointerUpHandler -= action;
				evt.OnPointerUpHandler += action;
				break;
			case Define.EUIEvent.Drag:
				evt.OnDragHandler -= action;
				evt.OnDragHandler += action;
				break;
		}
	}
}
