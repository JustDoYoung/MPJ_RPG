using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class UI_WaypointPopup : UI_Popup
{
    enum GameObjects
    {
        WaypointList
    }

    enum Buttons
    {
        CloseButton,
    }

    List<UI_StageItem> _items = new List<UI_StageItem>();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        Refresh();
        return true;
    }

    public void SetInfo()
    {
		Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        _items.Clear();

		GameObject parent = GetObject((int)GameObjects.WaypointList);

        foreach (var stage in Managers.Map.StageTransition.Stages)
        {
            UI_StageItem item = Managers.UI.MakeSubItem<UI_StageItem>(parent.transform);

            item.SetInfo(stage, () =>
            {
                Managers.UI.ClosePopupUI(this);
            });

            _items.Add(item);
		}
    }

    void OnClickCloseButton(PointerEventData evt)
    {
        Managers.UI.ClosePopupUI(this);
    }
}

