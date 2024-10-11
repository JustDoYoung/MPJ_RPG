using UnityEngine;
using UnityEngine.EventSystems;

public class UI_GameScene : UI_Scene
{
    enum Buttons
    {
        GoldPlusButton,
        DiaPlusButton,
        HeroesListButton,
        SetHeroesButton,
        SettingButton,
        InventoryButton,
        WorldMapButton,
        QuestButton,
        ChallengeButton,
        PortalButton,
        CampButton,
        CheatButton,
    }

    enum Texts
    {
        LevelText,
        BattlePowerText,
        GoldCountText,
        DiaCountText,
        MeatCountText,
        WoodCountText,
        MineralCountText,
    }

    enum Sliders
    {
        MeatSlider,
        WoodSlider,
        MineralSlider,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));
        BindSliders(typeof(Sliders));

        GetButton((int)Buttons.GoldPlusButton).gameObject.BindEvent(OnClickGoldPlusButton);
        GetButton((int)Buttons.DiaPlusButton).gameObject.BindEvent(OnClickDiaPlusButton);
        GetButton((int)Buttons.HeroesListButton).gameObject.BindEvent(OnClickHeroesListButton);
        GetButton((int)Buttons.SetHeroesButton).gameObject.BindEvent(OnClickSetHeroesButton);
        GetButton((int)Buttons.SettingButton).gameObject.BindEvent(OnClickSettingButton);
        GetButton((int)Buttons.InventoryButton).gameObject.BindEvent(OnClickInventoryButton);
        GetButton((int)Buttons.WorldMapButton).gameObject.BindEvent(OnClickWorldMapButton);
        GetButton((int)Buttons.QuestButton).gameObject.BindEvent(OnClickQuestButton);
        GetButton((int)Buttons.ChallengeButton).gameObject.BindEvent(OnClickChallengeButton);
        GetButton((int)Buttons.PortalButton).gameObject.BindEvent(OnClickPortalButton);
        GetButton((int)Buttons.CampButton).gameObject.BindEvent(OnClickCampButton);
        GetButton((int)Buttons.CheatButton).gameObject.BindEvent(OnClickCheatButton);

        Refresh();
        
        return true;
    }

    private float _elapsedTime = 0.0f;
    private float _updateInterval = 1.0f;

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= _updateInterval)
        {
            float fps = 1.0f / Time.deltaTime;
            float ms = Time.deltaTime * 1000.0f; // 1ms = 0.001초
            string text = string.Format("{0:N1} FPS ({1:N1}ms)", fps, ms);
            GetText((int)Texts.GoldCountText).text = text;

            _elapsedTime = 0;
        }
    }
    
    public void SetInfo()
    {
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;
    }

    void OnClickGoldPlusButton(PointerEventData evt)
    {
        Debug.Log("OnOnClickGoldPlusButton");
    }

    void OnClickDiaPlusButton(PointerEventData evt)
    {
        Debug.Log("OnClickDiaPlusButton");
    }

    void OnClickHeroesListButton(PointerEventData evt)
    {
		Debug.Log("OnClickHeroesListButton");
        UI_HeroesListPopup HeroesListPopup = Managers.UI.ShowPopupUI<UI_HeroesListPopup>();
        HeroesListPopup.SetInfo();
    }

    void OnClickSetHeroesButton(PointerEventData evt)
    {
		Debug.Log("OnClickSetHeroesButton");
	}

    void OnClickSettingButton(PointerEventData evt)
    {
		Debug.Log("OnClickSettingButton");
	}

    void OnClickInventoryButton(PointerEventData evt)
    {
        Debug.Log("OnClickInventoryButton");
    }

    void OnClickWorldMapButton(PointerEventData evt)
    {
        Debug.Log("OnClickWorldMapButton");
    }

    void OnClickQuestButton(PointerEventData evt)
    {
        Debug.Log("OnClickQuestButton");
    }

    void OnClickChallengeButton(PointerEventData evt)
    {
        Debug.Log("OnOnClickChallengeButton");
    }

    void OnClickCampButton(PointerEventData evt)
    {
        Debug.Log("OnClickCampButton");
    }

    void OnClickPortalButton(PointerEventData evt)
    {
        Debug.Log("OnClickPortalButton");
	}

    void OnClickCheatButton(PointerEventData evt)
    {
		Debug.Log("OnClickCheatButton");
	}

    public void RefreshWoodText()
    {
        GetText((int)Texts.WoodCountText).text = Managers.Game.Wood.ToString();
    }

	public void RefreshMineralText()
	{
		GetText((int)Texts.MineralCountText).text = Managers.Game.Mineral.ToString();
	}

	public void RefreshMeatText()
	{
		GetText((int)Texts.MeatCountText).text = Managers.Game.Meat.ToString();
	}

	public void RefreshGoldText()
	{
		GetText((int)Texts.GoldCountText).text = Managers.Game.Gold.ToString();
	}
}