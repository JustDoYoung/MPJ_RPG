using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
            //print($"{key} {count}/{totalCount}");

            if(count == totalCount)
            {
                Managers.Data.Init();
                GetObject((int)GameObjects.StartImage).gameObject.SetActive(true);
                GetText((int)Texts.DisplayText).text = $"Touch to Start";

                Type type = typeof(Data.TestData);
                FieldInfo[] fields = type.GetFields(BindingFlags.Public|BindingFlags.Instance);

                foreach(FieldInfo field in fields)
                {
                    object value = field.GetValue(Managers.Data.TestDic[1]);

                    print($"{field.Name} : {value}");
                }
            }
        });
    }
}
