using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class TitleScene : BaseScene
{
    //private void StartLoadAsset()
    //{
    //    Managers.Resource.LoadAllAsync<UnityEngine.Object>("Preload", (key, count, totalCount) =>
    //    {
    //        print($"{key} {count}/{totalCount}");
    //    });
    //}

    public override bool Init()
    {
        if (base.Init() == false) return false;

        SceneType = EScene.TitleScene;
        //StartLoadAsset();

        return true;
    }

    public override void Clear()
    {
        
    }
}
