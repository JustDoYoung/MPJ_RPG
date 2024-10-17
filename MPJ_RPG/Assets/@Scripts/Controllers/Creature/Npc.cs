using Data;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public interface INpcInteraction
{
	public void SetInfo(Npc owner);
	public void HandleOnClickEvent();
	public bool CanInteract();
}

public class Npc : BaseObject
{
	public NpcData Data { get; set; }

	private SkeletonAnimation _skeletonAnim;
	private UI_NpcInteraction _ui;

	public ENpcType NpcType { get { return Data.NpcType; } }

	public INpcInteraction Interaction { get; private set; }

	private void Update()
	{
		if (Interaction != null && Interaction.CanInteract())
		{
			_ui.gameObject.SetActive(true);
		}
		else
		{
			_ui.gameObject.SetActive(false);
		}
	}

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		ObjectType = EObjectType.Npc;
		return true;
	}

	public void SetInfo(int dataId)
	{
		Data = Managers.Data.NpcDic[dataId];
		gameObject.name = $"{Data.DataId}_{Data.Name}";

		#region Spine Animation
		SetSpineAnimation(Data.SkeletonDataID, SortingLayers.NPC);
		PlayAnimation(0, AnimName.IDLE, true);
        #endregion

        // Npc 상호작용을 위한 버튼
        //GameObject button = Managers.Resource.Instantiate("UI_NpcInteraction", gameObject.transform);
        //button.transform.localPosition = new Vector3(0f, 3f);
        //_ui = button.GetComponent<UI_NpcInteraction>();
		_ui = Managers.UI.ShowBaseUI<UI_NpcInteraction>();
        _ui.SetInfo(DataTemplateID, this);

		//Npc가 보유한 퀘스트 설정
		switch (Data.NpcType)
		{
			case ENpcType.Quest:
				Interaction = new QuestInteraction();
				break;
		}

		Interaction?.SetInfo(this);
	}

	public virtual void OnClickEvent()
	{
		Interaction?.HandleOnClickEvent();
	}
}
