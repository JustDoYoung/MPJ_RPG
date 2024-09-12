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


        HeroCamp camp = Managers.Object.Spawn<HeroCamp>(new Vector3Int(-10, -5, 0), 0);
        for (int i = 0; i < 5; i++)
        {
            int heroID = HERO_WIZARD_ID + Random.Range(0, 5);
            Hero hero = Managers.Object.Spawn<Hero>(new Vector3Int(-10 + Random.Range(-5, 5), -5 + Random.Range(-5, 5), 0), heroID);
        }

        {
            Managers.Object.Spawn<Monster>(new Vector3(0, 1, 0), MONSTER_BEAR_ID);
            //Managers.Object.Spawn<Monster>(new Vector3(0, 1, 0), MONSTER_SLIME_ID);
            //Managers.Object.Spawn<Monster>(new Vector3(0, 1, 0), MONSTER_SPIDER_COMMON_ID);
        }
        
        Env env = Managers.Object.Spawn<Env>(new Vector3(0, 2, 0), ENV_TREE1_ID);

        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        camera.Target = camp;

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
