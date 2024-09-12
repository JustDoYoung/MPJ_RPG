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

    public void Init()
    {
        HeroDic = LoadJson<Data.HeroDataLoader, int, Data.HeroData>("HeroData").MakeDict();
        MonsterDic = LoadJson<Data.MonsterDataLoader, int, Data.MonsterData>("MonsterData").MakeDict();
        EnvDic = LoadJson<Data.EnvDataLoader, int, Data.EnvData>("EnvData").MakeDict();
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
