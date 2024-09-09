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


        Hero hero = Managers.Object.Spawn<Hero>(new Vector3(-10, -5, 0), HERO_KNIGHT_ID);
        Monster monster = Managers.Object.Spawn<Monster>(new Vector3(0, 1, 0), MONSTER_BEAR_ID);
        Env env = Managers.Object.Spawn<Env>(new Vector3(0, 2, 0), ENV_TREE1_ID);

        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        camera.Target = hero;

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
