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

		//StartLoadAssets();

		return true;
	}

	void StartLoadAssets()
    {
        Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) =>
        {
			Debug.Log($"{key} {count}/{totalCount}");

			if (count == totalCount)
			{
				//Managers.Data.Init();
			}
		});
    }

	public override void Clear()
	{

	}
}
