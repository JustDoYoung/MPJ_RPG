using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene
{
	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		SceneType = Define.EScene.TitleScene;

		return true;
	}

	public override void Clear()
	{

	}
}
