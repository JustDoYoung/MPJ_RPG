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

        LoadMap();
        Managers.Object.Spawn<Hero>(Vector3.zero);
        Managers.UI.ShowBaseUI<UI_Joystick>();

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
