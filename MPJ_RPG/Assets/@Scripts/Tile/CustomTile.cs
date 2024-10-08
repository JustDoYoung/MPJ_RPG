using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

public class CustomTile : Tile
{
    [Space] [Space] [Header("For Zombie")] 
    public Define.EObjectType ObjectType;
    public int DataId;
    public string Name;
    public bool isStartPos = false;
    public bool isWayPoint = false;
}