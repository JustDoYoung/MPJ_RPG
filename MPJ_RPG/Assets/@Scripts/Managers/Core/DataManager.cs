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
    public Dictionary<int, Data.TestData> TestDic = new Dictionary<int, Data.TestData>();

    public void Init()
    {
        TestDic = LoadJson<Data.TestDataLoader, int, Data.TestData>("TestData").MakeDict();
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
