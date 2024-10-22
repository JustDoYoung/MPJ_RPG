using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebPacket;

public class Managers : MonoBehaviour
{
	public static string Jwt {  get; set; }
	public static bool Initialized { get; set; } = false;

	private static Managers s_instance;
	public static Managers Instance { get { Init(); return s_instance; } }

	#region Contents
	private GameManager _game = new GameManager();
	private ObjectManager _object = new ObjectManager();
	private MapManager _map = new MapManager();
	private InventoryManager _inventory = new InventoryManager();
	private QuestManager _quest = new QuestManager();
    private HeroManager _hero = new HeroManager();
    private AuthManager _auth = new AuthManager();

	public static GameManager Game { get { return Instance?._game; } }
	public static ObjectManager Object { get { return Instance?._object; } }
	public static MapManager Map { get { return Instance?._map; } }
	public static InventoryManager Inventory { get { return Instance?._inventory; } }
	public static QuestManager Quest { get { return Instance?._quest; } }
	public static HeroManager Hero { get { return Instance?._hero; } }
    public static AuthManager Auth { get { return Instance?._auth; } }
    #endregion

    #region Core
    private DataManager _data = new DataManager();
	private PoolManager _pool = new PoolManager();
	private ResourceManager _resource = new ResourceManager();
	private SceneManagerEx _scene = new SceneManagerEx();
	private SoundManager _sound = new SoundManager();
	private UIManager _ui = new UIManager();
    private WebManager _web = new WebManager();

    public static DataManager Data { get { return Instance?._data; } }
	public static PoolManager Pool { get { return Instance?._pool; } }
	public static ResourceManager Resource { get { return Instance?._resource; } }
	public static SceneManagerEx Scene { get { return Instance?._scene; } }
	public static SoundManager Sound { get { return Instance?._sound; } }
	public static UIManager UI { get { return Instance?._ui; } }
    public static WebManager Web { get { return Instance?._web; } }
    #endregion

    #region Language
    private static Define.ELanguage _language = Define.ELanguage.Korean;
	public static Define.ELanguage Language
	{
		get { return _language; }
		set
		{
			_language = value;
		}
	}

	public static string GetText(string textId)
	{
		switch (_language)
		{
			case Define.ELanguage.Korean:
				return Data.TextDic[textId].KOR;
			case Define.ELanguage.English:
				break;
			case Define.ELanguage.French:
				break;
			case Define.ELanguage.SimplifiedChinese:
				break;
			case Define.ELanguage.TraditionalChinese:
				break;
			case Define.ELanguage.Japanese:
				break;
		}

		return "";
	}
	#endregion

	public static void Init()
	{
		if (s_instance == null && Initialized == false)
		{
			Initialized = true;

			GameObject go = GameObject.Find("@Managers");
			if (go == null)
			{
				go = new GameObject { name = "@Managers" };
				go.AddComponent<Managers>();
			}

			DontDestroyOnLoad(go);

			// 초기화
			s_instance = go.GetComponent<Managers>();
			s_instance._quest.Init();
            s_instance._web.Init();
        }
	}

}
