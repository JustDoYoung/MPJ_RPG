using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    public override bool Init()
    {
        if (base.Init() == false) return false;

        SceneType = EScene.GameScene;

        return true;
    }

    public override void Clear()
    {

    }
}
