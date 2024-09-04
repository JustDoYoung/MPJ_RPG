using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    GameObject map;

    public override bool Init()
    {
        if (base.Init() == false) return false;

        SceneType = EScene.GameScene;

        //Managers.Resource.Instantiate("BaseMap");

        Managers.Resource.LoadAllAsync<UnityEngine.Object>("Preload", (key, count, totalCount) =>
        {
            print($"{key} {count}/{totalCount}");

            if(count == totalCount)
            {
                LoadMap();
                Managers.Resource.Instantiate("Hero");
            }
        });

        return true;
    }

    private void LoadMap()
    {
        map = Managers.Resource.Instantiate("BaseMap");
        map.transform.position = Vector3.zero;
        map.name = "@BaseMap";
    }

    public override void Clear()
    {

    }
}
