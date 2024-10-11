using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_TitleScene : UI_Scene
{
    enum GameObjects
    {
        StartImage
    }

    enum Texts
    {
        DisplayText
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));

		GetObject((int)GameObjects.StartImage).BindEvent((evt) =>
		{
			Debug.Log("ChangeScene");
			Managers.Scene.LoadScene(EScene.GameScene);
		});

		GetObject((int)GameObjects.StartImage).gameObject.SetActive(false);
		GetText((int)Texts.DisplayText).text = $"";

		StartLoadAssets();

		return true;
    }

	void StartLoadAssets()
	{
		Managers.Resource.LoadAllAsync<Object>("Preload", (key, count, totalCount) =>
		{
			Debug.Log($"{key} {count}/{totalCount}");

			if (count == totalCount)
			{
				Managers.Data.Init();

				// 데이터 있는지 확인
				if (Managers.Game.LoadGame() == false)
				{
					Managers.Game.InitGame();
					Managers.Game.SaveGame();
				}

				GetObject((int)GameObjects.StartImage).gameObject.SetActive(true);
				GetText((int)Texts.DisplayText).text = "Touch To Start";

			}
		});
	}
}
