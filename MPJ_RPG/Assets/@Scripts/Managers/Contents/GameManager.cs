using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;
using Data;

#region Save Data
[Serializable]
public class GameSaveData
{
    public int Wood = 0;
    public int Mineral = 0;
    public int Meat = 0;
    public int Gold = 0;

    public int ItemDbIdGenerator = 1;
    public List<HeroSaveData> Heroes = new List<HeroSaveData>();
    public List<ItemSaveData> Items = new List<ItemSaveData>();
    public List<QuestSaveData> AllQuests = new List<QuestSaveData>();
}

[Serializable]
public class HeroSaveData
{
    public int DataId = 0;
    public int Level = 1;
    public int Exp = 0;
    public EHeroOwningState OwningState = EHeroOwningState.Unowned;
}

[Serializable]
public class ItemSaveData
{
    public int InstanceId;
    public int DbId;
    public int TemplateId;
    public int Count;
    public int EquipSlot; // 장착 + 인벤 + 창고
                          //public int OwnerId;
    public int EnchantCount;
}

[Serializable]
public class QuestSaveData
{
    public int TemplateId;
    public EQuestState State = EQuestState.None;
    public List<int> ProgressCount = new List<int>();
    public DateTime NextResetTime;
}
#endregion

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
            int diff = _saveData.Wood - value;
            _saveData.Wood = value;
            OnBroadcastEvent?.Invoke(EBroadcastEventType.ChangeWood, diff);
        }
    }

    public int Mineral
    {
        get { return _saveData.Mineral; }
        private set
        {
            int diff = _saveData.Mineral - value;
            _saveData.Mineral = value;
            OnBroadcastEvent?.Invoke(EBroadcastEventType.ChangeMineral, diff);
        }
    }

    public int Meat
    {
        get { return _saveData.Meat; }
        private set
        {
            int diff = _saveData.Meat - value;
            _saveData.Meat = value;
            OnBroadcastEvent?.Invoke(EBroadcastEventType.ChangeMeat, diff);
        }
    }

    public int Gold
    {
        get { return _saveData.Gold; }
        private set
        {
            int diff = _saveData.Gold - value;
            _saveData.Gold = value;
            OnBroadcastEvent?.Invoke(EBroadcastEventType.ChangeGold, diff);
        }
    }

    public List<HeroSaveData> AllHeroes { get { return _saveData.Heroes; } }
    public int TotalHeroCount { get { return _saveData.Heroes.Count; } }
    public int UnownedHeroCount { get { return _saveData.Heroes.Where(h => h.OwningState == EHeroOwningState.Unowned).Count(); } }
    public int OwnedHeroCount { get { return _saveData.Heroes.Where(h => h.OwningState == EHeroOwningState.Owned).Count(); } }
    public int PickedHeroCount { get { return _saveData.Heroes.Where(h => h.OwningState == EHeroOwningState.Picked).Count(); } }

    public int GenerateItemDbId()
    {
        int itemDbId = _saveData.ItemDbIdGenerator;
        _saveData.ItemDbIdGenerator++;
        return itemDbId;
    }

    public void BroadcastEvent(EBroadcastEventType eventType, int value)
    {
        OnBroadcastEvent?.Invoke(eventType, value);
    }

    public bool CheckResource(EResourceType eResourceType, int amount)
    {
        switch (eResourceType)
        {
            case EResourceType.Wood:
                return Wood >= amount;
            case EResourceType.Mineral:
                return Mineral >= amount;
            case EResourceType.Meat:
                return Meat >= amount;
            case EResourceType.Gold:
                return Gold >= amount;
            case EResourceType.Dia:
                return true;
            case EResourceType.Materials:
                return true;
            default:
                return false;
        }
    }

    public bool SpendResource(EResourceType eResourceType, int amount)
    {
        if (CheckResource(eResourceType, amount) == false)
            return false;

        switch (eResourceType)
        {
            case EResourceType.Wood:
                Wood -= amount;
                break;
            case EResourceType.Mineral:
                Mineral -= amount;
                break;
            case EResourceType.Meat:
                Meat -= amount;
                break;
            case EResourceType.Gold:
                Gold -= amount;
                break;
            case EResourceType.Dia:
                break;
            case EResourceType.Materials:
                break;
        }

        return true;
    }

    public void EarnResource(EResourceType eResourceType, int amount)
    {
        switch (eResourceType)
        {
            case EResourceType.Wood:
                Wood += amount;
                break;
            case EResourceType.Mineral:
                Mineral += amount;
                break;
            case EResourceType.Meat:
                Meat += amount;
                break;
            case EResourceType.Gold:
                Gold += amount;
                break;
            case EResourceType.Dia:
                break;
            case EResourceType.Materials:
                break;
        }
    }
    #endregion

    #region Save & Load	
    public string Path { get { return Application.persistentDataPath + "/SaveData.json"; } }

    //파일이 없는 상태에서 최초로 만들 때
    public void InitGame()
    {
        if (File.Exists(Path))
            return;

        // Hero
        var heroes = Managers.Data.HeroDic.Values.ToList();
        foreach (HeroData hero in heroes)
        {
            HeroSaveData saveData = new HeroSaveData()
            {
                DataId = hero.DataId,
            };

            SaveData.Heroes.Add(saveData);
        }

        // Item
        {

        }

        // Quest
        {
            var quests = Managers.Data.QuestDic.Values.ToList();

            foreach (QuestData questData in quests)
            {
                QuestSaveData saveData = new QuestSaveData()
                {
                    TemplateId = questData.DataId,
                    State = EQuestState.None,
                    ProgressCount = new List<int>(),
                    NextResetTime = DateTime.Now,
                };

                for (int i = 0; i < questData.QuestTasks.Count; i++)
                {
                    saveData.ProgressCount.Add(0);
                }

                Debug.Log("SaveDataQuest");
                Managers.Quest.AddQuest(saveData);
            }
        }

        // TEMP
        SaveData.Heroes[0].OwningState = EHeroOwningState.Picked;
        SaveData.Heroes[1].OwningState = EHeroOwningState.Owned;

        Wood = 100;
        Gold = 100;
        Mineral = 100;
        Meat = 100;

        string jsonStr = JsonUtility.ToJson(Managers.Game.SaveData);
        File.WriteAllText(Path, jsonStr);
        Debug.Log($"Save Game Completed : {Path}");
    }

    public void SaveGame()
    {
        // Hero
        {
            SaveData.Heroes.Clear();
            foreach (var heroinfo in Managers.Hero.AllHeroInfos.Values)
            {
                SaveData.Heroes.Add(heroinfo.SaveData);
            }
        }

        // Item
        {
            SaveData.Items.Clear();
            foreach (var item in Managers.Inventory.AllItems)
                SaveData.Items.Add(item.SaveData);
        }

        // Quest
        {
            SaveData.AllQuests.Clear();
            foreach (var quest in Managers.Quest.AllQuests.Values)
                SaveData.AllQuests.Add(quest.SaveData);
        }

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

        // Hero
        {
            Managers.Hero.AllHeroInfos.Clear();
            foreach (HeroSaveData heroSaveData in data.Heroes)
            {
                Managers.Hero.AddHeroInfo(heroSaveData);

            }

            Managers.Hero.AddUnknownHeroes();
        }

        // Item
        {
            Managers.Inventory.Clear();
            foreach (ItemSaveData itemSaveData in data.Items)
            {
                Managers.Inventory.AddItem(itemSaveData);
            }
        }

        // Quest
        {
            Managers.Quest.Clear();
            foreach (QuestSaveData questSaveData in data.AllQuests)
            {
                Managers.Quest.AddQuest(questSaveData);
            }

            //새로운 퀘스트 생길 시 추가
            Managers.Quest.AddUnknownQuests();
        }

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

    public event Action<EBroadcastEventType, int> OnBroadcastEvent;
    #endregion
}
