using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Define;

public struct ObjectSpawnInfo
{
	public ObjectSpawnInfo(string name, int dataId, int x, int y, Vector3 worldPos, EObjectType type)
	{
		Name = name;
		DataId = dataId;
		Vector3Int pos = new Vector3Int(x, y, 0);
		CellPos = pos;
		WorldPos = worldPos;
		ObjectType = type;
	}

	public string Name;
	public int DataId;
	public Vector3Int CellPos;
	public Vector3 WorldPos;
	public EObjectType ObjectType;
}

public class Stage : MonoBehaviour
{
    [SerializeField]
    private List<BaseObject> _spawnObjects = new List<BaseObject>();
    private List<ObjectSpawnInfo> _spawnInfos = new List<ObjectSpawnInfo>();

    private ObjectSpawnInfo _startSpawnInfo;
    public ObjectSpawnInfo StartSpawnInfo
    {
        get { return _startSpawnInfo; }
        set { _startSpawnInfo = value; }
    }

    public ObjectSpawnInfo WaypointSpawnInfo;
    public int StageIndex { get; set; }
    public Tilemap TilemapObject; // 하이어라키에서 추가
    public Tilemap TilemapTerrain;
    public bool IsActive = false;
    
    private Grid _grid;

    public void SetInfo(int stageIdx)
    {
        StageIndex = stageIdx;
        if (TilemapObject == null)
            Debug.LogError("TilemapObject must be assigned in the inspector.", this);
        
        TilemapTerrain = Util.FindChild<Tilemap>(gameObject, "Terrain_01", true);
        SaveSpawnInfos();
    }

    public bool IsPointInStage(Vector3 position)
    {
        Vector3Int pos = TilemapTerrain.layoutGrid.WorldToCell(position);
        TileBase tile = TilemapTerrain.GetTile(pos);

        if (tile == null)
            return false;

        return true;
    }

    public void LoadStage()
    {
        if (IsActive)
            return;

        IsActive = true;
        gameObject.SetActive(true);
        SpawnObjects();
    }

    public void UnLoadStage()
    {
        if (IsActive == false)
            return;

        IsActive = false;
        gameObject.SetActive(false);
        DespawnObjects();
    }
    
    private void SpawnObjects()
    {
        foreach (ObjectSpawnInfo info in _spawnInfos)
        {
            Vector3 worldPos = info.WorldPos;
            Vector3Int cellPos = info.CellPos;
            
            if (Managers.Map.CanGo(null, cellPos) == false)
                return;
            
            switch (info.ObjectType)
            {
                case EObjectType.Monster:
                    Monster monster = Managers.Object.Spawn<Monster>(worldPos, info.DataId);
                    monster.SetCellPos(cellPos, true);
                    _spawnObjects.Add(monster);
                    break;
                case EObjectType.Env:
                    Env env = Managers.Object.Spawn<Env>(worldPos, info.DataId);
                    env.SetCellPos(cellPos, true);
                    _spawnObjects.Add(env);
                    break;
                //case EObjectType.Npc:
                //    Npc npc = Managers.Object.Spawn<Npc>(worldPos, info.DataId);
                //    npc.SetCellPos(cellPos, true);
                //    _spawnObjects.Add(npc);
                //    break;
            }
        }
    }

    private void DespawnObjects()
    {
        foreach (BaseObject obj in _spawnObjects)
        {
            switch (obj.ObjectType)
            {
                case EObjectType.Monster:
                    Managers.Object.Despawn(obj as Monster);
                    break;
                case EObjectType.Env:
                    Managers.Object.Despawn(obj as Env);
                    break;
                //case EObjectType.Npc:
                //    Managers.Object.Despawn(obj as Npc);
                //    break;
            }
        }

        _spawnObjects.Clear();
    }
    
    private void SaveSpawnInfos()
    {
        if (TilemapObject != null)
            TilemapObject.gameObject.SetActive(false);

        for (int y = TilemapObject.cellBounds.yMax; y >= TilemapObject.cellBounds.yMin; y--)
        {
            for (int x = TilemapObject.cellBounds.xMin; x <= TilemapObject.cellBounds.xMax; x++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                CustomTile tile = TilemapObject.GetTile(new Vector3Int(x, y, 0)) as CustomTile;

                if (tile == null)
                    continue;

                Vector3 worldPos = Managers.Map.Cell2World(cellPos);
                ObjectSpawnInfo info = new ObjectSpawnInfo(tile.Name, tile.DataId, x, y, worldPos, tile.ObjectType);
                
                if (tile.isStartPos)
                {
                    StartSpawnInfo = info;
                    continue;
                }
                
                Debug.Log($"{tile.name} , {tile.isWayPoint}, {tile.ObjectType}");
                if (tile.isWayPoint)
                {
                    WaypointSpawnInfo = info;
                }

                _spawnInfos.Add(info);
            }
        }
    }
}