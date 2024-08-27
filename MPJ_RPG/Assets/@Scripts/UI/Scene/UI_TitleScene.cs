using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        if (base.Init() == false) return false;

        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));

        GetObject((int)GameObjects.StartImage).BindEvent((evt) => {
            print("ChangeScene");
            Managers.Scene.LoadScene(EScene.GameScene);
        });

        GetObject((int)GameObjects.StartImage).gameObject.SetActive(false);
        GetText((int)Texts.DisplayText).text = $"";

        StartLoadAsset();

        return true;
    }

    private void StartLoadAsset()
    {
        Managers.Resource.LoadAllAsync<UnityEngine.Object>("Preload", (key, count, totalCount) =>
        {
            print($"{key} {count}/{totalCount}");

            if(count == totalCount)
            {
                GetObject((int)GameObjects.StartImage).gameObject.SetActive(true);
                GetText((int)Texts.DisplayText).text = $"Touch to Start";
            }
        });
    }
}
