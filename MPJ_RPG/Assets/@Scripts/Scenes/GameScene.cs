using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        Hero hero = Managers.Object.Spawn<Hero>(new Vector3(-10, -5, 0));
        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        camera.Target = hero;

        Managers.UI.ShowBaseUI<UI_Joystick>();

        Monster monster = Managers.Object.Spawn<Monster>(new Vector3(0, 1, 0));

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
