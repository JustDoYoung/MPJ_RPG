using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StageItem : UI_Base
{
    private Stage _owner;
    public event Action OnCloseItem;
    
    enum Texts
    {
        NameText,
    }

	public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTexts(typeof(Texts));

        gameObject.BindEvent(OnClickStageItem);

        Refresh();

        return true;
    }

    public void SetInfo(Stage stage, Action action)
    {
        _owner = stage;
        OnCloseItem = action;
        
        GetText((int)Texts.NameText).text = stage.name;
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;
    }

    void OnClickStageItem(PointerEventData evt)
    {
        Debug.Log("OnClickStageItem");

		Managers.Map.StageTransition.OnMapChanged(_owner.StageIndex);
		Managers.Game.TeleportHeroes(_owner.WaypointSpawnInfo.CellPos);

		OnCloseItem?.Invoke();
    }

}
