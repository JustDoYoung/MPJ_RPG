using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    private int _order = 10;

    private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();

    private UI_Scene _sceneUI = null;
    public UI_Scene SceneUI
    {
        get { return _sceneUI; }
        set { _sceneUI = value; }
    }

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");

            if (root == null)
                root = new GameObject("@UI_Root");

            return root;
        }
    }
    public void SetCanvas(GameObject go, bool sort = true, int sortOrder = 0)
    {
        Canvas canvas = Utils.GetOrAddComponent<Canvas>(go);

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true; //부모 canvas의 sort order를 무시

        CanvasScaler cs = go.GetOrAddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1080, 2340);

        go.GetOrAddComponent<GraphicRaycaster>();

        if (sort)
        {
            canvas.sortingOrder = _order++;
        }
        else
        {
            canvas.sortingOrder = sortOrder;
        }
    }

    //public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    //{
    //	if (string.IsNullOrEmpty(name))
    //		name = typeof(T).Name;

    //	GameObject go = Managers.Resource.Instantiate($"{name}");
    //	if (parent != null)
    //		go.transform.SetParent(parent);

    //	Canvas canvas = go.GetOrAddComponent<Canvas>();
    //	canvas.renderMode = RenderMode.WorldSpace;
    //	canvas.worldCamera = Camera.main;

    //	return Utils.GetOrAddComponent<T>(go);
    //}

    //public T MakeSubItem<T>(Transform parent = null, string name = null, bool pooling = true) where T : UI_Base
    //{
    //	if (string.IsNullOrEmpty(name))
    //		name = typeof(T).Name;

    //	GameObject go = Managers.Resource.Instantiate(name, parent, pooling);
    //	go.transform.SetParent(parent);

    //	return Utils.GetOrAddComponent<T>(go);
    //}

    public T ShowBaseUI<T>(string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(name);
        T baseUI = Utils.GetOrAddComponent<T>(go);

        go.transform.SetParent(Root.transform);

        return baseUI;
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(name);
        T sceneUI = Utils.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);

        return sceneUI;
    }

    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(name);
        T popup = Utils.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        go.transform.SetParent(Root.transform);

        return popup;
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
        Managers.Resource.Destroy(popup.gameObject);
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
}
