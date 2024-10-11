using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;
using Data;

[Serializable]
public class GameSaveData
{
    public int Wood = 0;
    public int Mineral = 0;
    public int Meat = 0;
    public int Gold = 0;

    public List<HeroSaveData> Heroes = new List<HeroSaveData>();
}

[Serializable]
public class HeroSaveData
{
    public int DataId = 0;
    public int Level = 1;
    public int Exp = 0;
    public HeroOwningState OwningState = HeroOwningState.Unowned;
}

public enum HeroOwningState
{
    Unowned,
    Owned,
    Picked,
}

public class GameManager
{
    #region GameData
    GameSaveData _saveData = new GameSaveData();
    public GameSaveData SaveData { get { return _saveData; } set { _saveData = value; } }

    public int Wood
    {
        get { return _saveData.Wood; }
        private set
        {
            _saveData.Wood = value;
            (Managers.UI.SceneUI as UI_GameScene)?.RefreshWoodText();
        }
    }

    public int Mineral
    {
        get { return _saveData.Mineral; }
        private set
        {
            _saveData.Mineral = value;
            (Managers.UI.SceneUI as UI_GameScene)?.RefreshMineralText();
        }
    }

    public int Meat
    {
        get { return _saveData.Meat; }
        private set
        {
            _saveData.Meat = value;
            (Managers.UI.SceneUI as UI_GameScene)?.RefreshMeatText();
        }
    }

    public int Gold
    {
        get { return _saveData.Gold; }
        private set
        {
            _saveData.Gold = value;
            (Managers.UI.SceneUI as UI_GameScene)?.RefreshGoldText();
        }
    }

    public List<HeroSaveData> AllHeroes { get { return _saveData.Heroes; } }
    public int TotalHeroCount { get { return _saveData.Heroes.Count; } }
    public int UnownedHeroCount { get { return _saveData.Heroes.Where(h => h.OwningState == HeroOwningState.Unowned).Count(); } }
    public int OwnedHeroCount { get { return _saveData.Heroes.Where(h => h.OwningState == HeroOwningState.Owned).Count(); } }
    public int PickedHeroCount { get { return _saveData.Heroes.Where(h => h.OwningState == HeroOwningState.Picked).Count(); } }

    #endregion

    #region Save & Load	
    public string Path { get { return Application.persistentDataPath + "/SaveData.json"; } }

    //파일이 없는 상태에서 최초로 만들 때
    public void InitGame()
    {
        if (File.Exists(Path))
            return;

        var heroes = Managers.Data.HeroDic.Values.ToList();
        foreach (HeroData hero in heroes)
        {
            HeroSaveData saveData = new HeroSaveData()
            {
                DataId = hero.DataId,
            };

            SaveData.Heroes.Add(saveData);
        }

        // TEMP
        SaveData.Heroes[0].OwningState = HeroOwningState.Picked;
        SaveData.Heroes[1].OwningState = HeroOwningState.Owned;
    }

    public void SaveGame()
    {
        string jsonStr = JsonUtility.ToJson(Managers.Game.SaveData);
        File.WriteAllText(Path, jsonStr);
        Debug.Log($"Save Game Completed : {Path}");
    }

    public bool LoadGame()
    {
        if (File.Exists(Path) == false)
            return false;

        string fileStr = File.ReadAllText(Path);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(fileStr);

        if (data != null)
            Managers.Game.SaveData = data;

        Debug.Log($"Save Game Loaded : {Path}");
        return true;
    }
    #endregion

    #region Hero
    private Vector2 _moveDir;
    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set
        {
            _moveDir = value;
            OnMoveDirChanged?.Invoke(value);
        }
    }

    private EJoystickState _joystickState;
    public EJoystickState JoystickState
    {
        get { return _joystickState; }
        set
        {
            _joystickState = value;
            OnJoystickStateChanged?.Invoke(value);
        }
    }
    #endregion

    #region Teleport
    public void TeleportHeroes(Vector3 position)
    {
        TeleportHeroes(Managers.Map.World2Cell(position));
    }

    public void TeleportHeroes(Vector3Int cellPos)
    {
        foreach (var hero in Managers.Object.Heroes)
        {
            Vector3Int randCellPos = Managers.Game.GetNearbyPosition(hero, cellPos);
            Managers.Map.MoveTo(hero, randCellPos, forceMove: true);
        }

        Vector3 worldPos = Managers.Map.Cell2World(cellPos);
        Managers.Object.Camp.ForceMove(worldPos);
        Camera.main.transform.position = worldPos;
    }
    #endregion

    #region Helper
    public Vector3Int GetNearbyPosition(BaseObject hero, Vector3Int pivot, int range = 5)
    {
        for (int i = 0; i < 100; i++)
        {
            int x = Random.Range(-range, range);
            int y = Random.Range(-range, range);
            Vector3Int randCellPos = pivot + new Vector3Int(x, y, 0);
            if (Managers.Map.CanGo(hero, randCellPos))
                return randCellPos;
        }

        Debug.LogError($"GetNearbyPosition Failed");

        return Vector3Int.zero;
    }
    #endregion

    #region Action
    public event Action<Vector2> OnMoveDirChanged;
    public event Action<EJoystickState> OnJoystickStateChanged;
    #endregion
}
