using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_HeroesList_HeroItem : UI_Base
{
	enum Buttons
	{
		HeroButton,
	}

	enum Images
	{
		HeroImage,
	}

	enum Texts
	{
		ExpText,
		LevelText,
	}

	enum Sliders
	{
		HeroExpSlider,
	}

	int _heroDataId = -1;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		BindButtons(typeof(Buttons));
		BindTexts(typeof(Texts));
		BindSliders(typeof(Sliders));
		BindImages(typeof(Images));

		GetButton((int)Buttons.HeroButton).gameObject.BindEvent(OnClickHeroButton);

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

		GetImage((int)Images.HeroImage).sprite = Managers.Resource.Load<Sprite>(Managers.Data.HeroDic[_heroDataId].IconImage);
	}

	void OnClickHeroButton(PointerEventData evt)
	{
		UI_HeroInfoPopup popup = Managers.UI.ShowPopupUI<UI_HeroInfoPopup>();
		popup.SetInfo(_heroDataId);
	}
}
