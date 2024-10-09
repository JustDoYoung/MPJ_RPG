using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

//인터페이스
public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, Data.HeroData> HeroDic = new Dictionary<int, Data.HeroData>();
    public Dictionary<int, Data.MonsterData> MonsterDic = new Dictionary<int, Data.MonsterData>();
    public Dictionary<int, Data.EnvData> EnvDic = new Dictionary<int, Data.EnvData>();
    public Dictionary<int, Data.SkillData> SkillDic = new Dictionary<int, Data.SkillData>();
    public Dictionary<int, Data.ProjectileData> ProjectileDic = new Dictionary<int, Data.ProjectileData>();
    public Dictionary<int, Data.EffectData> EffectDic { get; private set; } = new Dictionary<int, Data.EffectData>();
    public Dictionary<int, Data.AoEData> AoEDic { get; private set; } = new Dictionary<int, Data.AoEData>();
    public Dictionary<int, Data.NpcData> NpcDic { get; private set; } = new Dictionary<int, Data.NpcData>();

    public void Init()
    {
        HeroDic = LoadJson<Data.HeroDataLoader, int, Data.HeroData>("HeroData").MakeDict();
        MonsterDic = LoadJson<Data.MonsterDataLoader, int, Data.MonsterData>("MonsterData").MakeDict();
        EnvDic = LoadJson<Data.EnvDataLoader, int, Data.EnvData>("EnvData").MakeDict();
        SkillDic = LoadJson<Data.SkillDataLoader, int, Data.SkillData>("SkillData").MakeDict();
        ProjectileDic = LoadJson<Data.ProjectileDataLoader, int, Data.ProjectileData>("ProjectileData").MakeDict();
        EffectDic = LoadJson<Data.EffectDataLoader, int, Data.EffectData>("EffectData").MakeDict();
        AoEDic = LoadJson<Data.AoEDataLoader, int, Data.AoEData>("AoEData").MakeDict();
        NpcDic = LoadJson<Data.NpcDataLoader, int, Data.NpcData>("NpcData").MakeDict();
    }

    private Loader LoadJson<Loader, Key, Value>(string key)where Loader : ILoader<Key, Value>
    {
        //json 파일(바이트 스트림, 직렬화)
        TextAsset testAsset = Managers.Resource.Load<TextAsset>(key);

        if (testAsset == null) return default(Loader);

        //객체로 변환(역직렬화)
        return JsonConvert.DeserializeObject<Loader>(testAsset.text);
    }
}
