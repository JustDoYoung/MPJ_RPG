using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_NpcInteraction : UI_Base
{
    private Npc _owner;

    enum Buttons
    {
        InteractionButton
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));

        GetComponent<Canvas>().worldCamera = Camera.main;

        return true;
    }

    public void SetInfo(int dataId, Npc owner)
    {
        _owner = owner;
        GetButton((int)Buttons.InteractionButton).gameObject.BindEvent(OnClickInteractionButton);
    }

    private void OnClickInteractionButton(PointerEventData evt)
    {
        _owner?.OnClickEvent();

        Debug.Log("OnClickInteractionButton");
    }
}
