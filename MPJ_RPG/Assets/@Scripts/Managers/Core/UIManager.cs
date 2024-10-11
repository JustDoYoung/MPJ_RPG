using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager
{
	private int _order = 10;

	private Dictionary<string, UI_Popup> _popups = new Dictionary<string, UI_Popup>();
	private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();

	private UI_Scene _sceneUI = null;
	public UI_Scene SceneUI
	{
		set { _sceneUI = value; }
		get { return _sceneUI; }
	}

	public GameObject Root
	{
		get
		{
			GameObject root = GameObject.Find("@UI_Root");
			if (root == null)
				root = new GameObject { name = "@UI_Root" };
			return root;
		}
	}

	public void SetCanvas(GameObject go, bool sort = true, int sortOrder = 0)
	{
		Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
		if (canvas == null)
		{
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.overrideSorting = true;
		}

		CanvasScaler cs = go.GetOrAddComponent<CanvasScaler>();
		if (cs != null)
		{
			cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			cs.referenceResolution = new Vector2(1080, 1920);
		}

		go.GetOrAddComponent<GraphicRaycaster>();

		if (sort)
		{
			canvas.sortingOrder = _order;
			_order++;
		}
		else
		{
			canvas.sortingOrder = sortOrder;
		}
	}

	public T GetSceneUI<T>() where T : UI_Base
	{
		return _sceneUI as T;
	}

	public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate($"{name}");
		if (parent != null)
			go.transform.SetParent(parent);

		Canvas canvas = go.GetOrAddComponent<Canvas>();
		canvas.renderMode = RenderMode.WorldSpace;
		canvas.worldCamera = Camera.main;

		return Util.GetOrAddComponent<T>(go);
	}

	public T MakeSubItem<T>(Transform parent = null, string name = null, bool pooling = true) where T : UI_Base
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate(name, parent, pooling);
		go.transform.SetParent(parent);

		return Util.GetOrAddComponent<T>(go);
	}

	public T ShowBaseUI<T>(string name = null) where T : UI_Base
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate(name);
		T baseUI = Util.GetOrAddComponent<T>(go);

		go.transform.SetParent(Root.transform);

		return baseUI;
	}

	public T ShowSceneUI<T>(string name = null) where T : UI_Scene
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate(name);
		T sceneUI = Util.GetOrAddComponent<T>(go);
		_sceneUI = sceneUI;

		go.transform.SetParent(Root.transform);

		return sceneUI;
	}

	public T ShowPopupUI<T>(string name = null) where T : UI_Popup
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		if (_popups.TryGetValue(name, out UI_Popup popup) == false)
		{
			GameObject go = Managers.Resource.Instantiate(name);
			popup = Util.GetOrAddComponent<T>(go);
			_popups[name] = popup;
		}

		_popupStack.Push(popup);

		popup.transform.SetParent(Root.transform);
		popup.gameObject.SetActive(true);

		return popup as T;
	}

	public void ClosePopupUI(UI_Popup popup)
	{
		if (_popupStack.Count == 0)
			return;

		if (_popupStack.Peek() != popup)
		{
			Debug.Log("Close Popup Failed!");
			return;
		}

		ClosePopupUI();
	}

	public void ClosePopupUI()
	{
		if (_popupStack.Count == 0)
			return;

		UI_Popup popup = _popupStack.Pop();

		popup.gameObject.SetActive(false);
		//Managers.Resource.Destroy(popup.gameObject);

		_order--;
	}

	public void CloseAllPopupUI()
	{
		while (_popupStack.Count > 0)
			ClosePopupUI();
	}

	public int GetPopupCount()
	{
		return _popupStack.Count;
	}

	public void Clear()
	{
		CloseAllPopupUI();
		_sceneUI = null;
	}

	#region 팝업창 캐싱
	public void CacheAllPopups()
	{
		var list = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(type => type.IsSubclassOf(typeof(UI_Popup)));

		foreach (Type type in list)
		{
			CachePopupUI(type);
		}

		CloseAllPopupUI();
	}

    public void CachePopupUI(Type type)
	{
		string name = type.Name;

		if (_popups.TryGetValue(name, out UI_Popup popup) == false)
		{
			GameObject go = Managers.Resource.Instantiate(name);
			popup = go.GetComponent<UI_Popup>();
			_popups[name] = popup;
		}

		_popupStack.Push(popup);
	}
    #endregion
}
