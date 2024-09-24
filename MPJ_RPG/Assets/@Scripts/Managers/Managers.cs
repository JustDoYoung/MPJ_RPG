using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static bool Initialized = false;

    private static Managers s_instance;
    public static Managers Instance { get { Init(); return s_instance; } }

    #region Contents
    private GameManager _game = new GameManager();
    private ObjectManager _object = new ObjectManager();

    public static GameManager Game { get { return Instance?._game; } }
    public static ObjectManager Object { get { return Instance?._object; } }
    #endregion

    #region Core
    private DataManager _data = new DataManager();
    private PoolManager _pool = new PoolManager();
    private ResourceManager _resource = new ResourceManager();
    private SceneManagerEx _scene = new SceneManagerEx();
    private SoundManager _sound = new SoundManager();
    private UIManager _ui = new UIManager();

    public static DataManager Data { get { return Instance?._data; } }
    public static PoolManager Pool { get { return Instance?._pool; } }
    public static ResourceManager Resource { get { return Instance?._resource; } }
    public static SceneManagerEx Scene { get { return Instance?._scene; } }
    public static SoundManager Sound { get { return Instance?._sound; } }
    public static UIManager UI { get { return Instance?._ui; } }
    #endregion
    private static void Init()
    {
        if(s_instance == null && Initialized == false)
        {
            Initialized = true;

            GameObject go = GameObject.Find("@Managers");

            if(go == null)
            {
                go = new GameObject("@Managers");
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);

            s_instance = go.GetComponent<Managers>();
        }
    }
}
