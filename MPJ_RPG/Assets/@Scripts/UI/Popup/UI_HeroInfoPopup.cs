using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_HeroInfoPopup : UI_Popup
{
	enum GameObjects
	{
		CloseArea,
		HeroAbilityList,
		ExpSlider
	}

	enum Buttons
	{
		CloseButton,
		LevelUpButton,
		Skill1Button,
		Skill2Button,
	}

	enum Texts
	{
		NameText,
		ExpText,
		LevelText,
		BattlePowerText,
		DamageText,
		HpText,
		Skill1NameText,
		Skill2NameText,
		MeatCountText,
	}

	enum Images
	{
		HeroIconImage,
	}

	int _heroDataId = -1;
	HeroSaveData SaveData { get { return Managers.Game.AllHeroes.Find(h => h.DataId == _heroDataId); } }

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindObjects(typeof(GameObjects));
		BindButtons(typeof(Buttons));
		BindTexts(typeof(Texts));
		BindImages(typeof(Images));

		GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
		GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
		GetButton((int)Buttons.LevelUpButton).gameObject.BindEvent(OnClickLevelUpButton);
		GetButton((int)Buttons.Skill1Button).gameObject.BindEvent(OnClickSkill1Button);
		GetButton((int)Buttons.Skill2Button).gameObject.BindEvent(OnClickSkill2Button);

		Refresh();

		return true;
	}

	public void SetInfo(int heroDataId)
	{
		_heroDataId = heroDataId;
		Refresh();
	}

	void Refresh()
	{
		if (_init == false)
			return;

		if (_heroDataId < 0)
			return;

		Data.HeroData data = Managers.Data.HeroDic[_heroDataId];

		GetImage((int)Images.HeroIconImage).sprite = Managers.Resource.Load<Sprite>(data.IconImage);
		GetText((int)Texts.NameText).text = data.DescriptionTextID;

		GetText((int)Texts.LevelText).text = SaveData.Level.ToString();
		GetText((int)Texts.ExpText).text = $"{SaveData.Exp} / ??";

		// TODO
		float atk = data.Atk;
		float hp = data.MaxHp;
		GetText((int)Texts.BattlePowerText).text = (hp + atk * 5).ToString("F0");
		GetText((int)Texts.DamageText).text = atk.ToString("F0");
		GetText((int)Texts.HpText).text = hp.ToString("F0");
	}

	void OnClickCloseArea(PointerEventData evt)
	{
		Debug.Log("OnClickCloseArea");
		Managers.UI.ClosePopupUI(this);
	}

	void OnClickCloseButton(PointerEventData evt)
	{
		Debug.Log("OnClickCloseButton");
		Managers.UI.ClosePopupUI(this);
	}

	void OnClickLevelUpButton(PointerEventData evt)
	{
		Debug.Log("OnClickLevelUpButton");
		Refresh();
	}

	void OnClickSkill1Button(PointerEventData evt)
	{
		Debug.Log("OnClickSkill1Button");
	}

	void OnClickSkill2Button(PointerEventData evt)
	{
		Debug.Log("OnClickSkill2Button");
	}
}
