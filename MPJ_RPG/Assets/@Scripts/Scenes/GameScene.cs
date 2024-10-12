using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		SceneType = EScene.GameScene;

		Managers.Map.LoadMap("BaseMap");
		Managers.Map.StageTransition.SetInfo();

		var cellPos = Managers.Map.World2Cell(new Vector3(-100, -66));

		HeroCamp camp = Managers.Object.Spawn<HeroCamp>(Vector3.zero, 0);
		camp.SetCellPos(cellPos, true);

		for (int i = 0; i < 1; i++)
		{
            //int heroTemplateID = HERO_WIZARD_ID + Random.Range(0, 2);
            //int heroTemplateID = HERO_LION_ID;
            int heroTemplateID = HERO_KNIGHT_ID;

            Vector3Int randCellPos = new Vector3Int(0 + Random.Range(-3, 3), 0 + Random.Range(-3, 3), 0);
			if (Managers.Map.CanGo(null, randCellPos) == false)
				continue;

			Hero hero = Managers.Object.Spawn<Hero>(new Vector3Int(1, 0, 0), heroTemplateID);
			//hero.SetCellPos(randCellPos, true);
			Managers.Map.MoveTo(hero, cellPos, true);
		}

		CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
		camera.Target = camp;

		//     {
		//         Monster monster = Managers.Object.Spawn<Monster>(new Vector3(0, 2, 0), MONSTER_BEAR_ID);
		//monster.ExtraCells = 1;
		//     }

		UI_GameScene sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();
		sceneUI.SetInfo();

		Managers.UI.ShowBaseUI<UI_Joystick>();
		// TODO
		Managers.UI.CacheAllPopups();
		return true;
	}

	public override void Clear()
	{

	}
}
