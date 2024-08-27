using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

internal class Pool
{
    private GameObject _prefab;
    private IObjectPool<GameObject> _pool;

    private Transform _root;
    private Transform Root
    {
        get
        {
            if(_root == null)
            {
                GameObject go = new GameObject($"@{_prefab.name}Pool");
                _root = go.transform;
            }

            return _root;
        }
    }

    //생성자
    /// <summary>
    /// OnCreate 메서드는 객체 풀에서 새로운 객체가 필요할 때 자동으로 호출됩니다. 예를 들어, 풀에 사용할 수 있는 객체가 없을 때 ObjectPool이 새로운 객체를 생성하기 위해 OnCreate를 호출합니다. 따라서 OnCreate는 직접 호출하지 않아도, 풀에서 객체를 요청할 때 필요한 경우 자동으로 호출됩니다.
    /// </summary>
    /// <param name="prefab"></param>
    public Pool(GameObject prefab)
    {
        _prefab = prefab;
        _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
    }

    //객체를 풀에 반환
    public void Push(GameObject go)
    {
        if (go.activeSelf)
            _pool.Release(go);
    }

    //객체를 풀에서 가져옴
    public GameObject Pop()
    {
        return _pool.Get();
    }

    public void Clear()
    {
        _pool.Clear();
    }

    #region 내부함수
    private GameObject OnCreate()
    {
        GameObject go = GameObject.Instantiate(_prefab, Root);
        go.name = _prefab.name;
        
        return go;
    }

    private void OnGet(GameObject go)
    {
        go.SetActive(true);
    }

    private void OnRelease(GameObject go)
    {
        go.SetActive(false);
    }

    private void OnDestroy(GameObject go)
    {
        GameObject.Destroy(go);
    }
    #endregion
}
public class PoolManager
{
    private Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

    public GameObject Pop(GameObject prefab) {

        if (_pools.ContainsKey(prefab.name) == false) CreatePool(prefab);

        return _pools[prefab.name].Pop();
    }

    public bool Push (GameObject go) {
        if (_pools.ContainsKey(go.name) == false) return false;

        _pools[go.name].Push(go);
        return true;
    }

    public void Clear() 
    {
        foreach (var pool in _pools.Values)
            pool.Clear();

        _pools.Clear();
    }
    private void CreatePool(GameObject original) {
        Pool pool = new Pool(original);
        _pools.Add(original.name, pool);
    }
}
