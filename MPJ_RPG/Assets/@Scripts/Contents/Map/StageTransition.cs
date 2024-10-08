using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StageTransition : InitBase
{
    public List<Stage> Stages = new List<Stage>();
    public Stage CurrentStage { get; set; }
    public int CurrentStageIndex { get; private set; } = -1;
    private Hero _leader;

    public void SetInfo()
    {
        int currentMapIndex = 0;

        for (int i = 0; i < Stages.Count; i++)
        {
            Stages[i].SetInfo(i);  
            
            if (Stages[i].StartSpawnInfo.WorldPos != Vector3.zero)
            {
                currentMapIndex = i;
            }
        }

        OnMapChanged(currentMapIndex);
	}

    public void CheckMapChanged(Vector3 position)
    {
        if (CurrentStage.IsPointInStage(position) == false)
        {
            int stageIndex = GetStageIndex(position);
            OnMapChanged(stageIndex);
        }
    }

    private int GetStageIndex(Vector3 position)
    {
        for (int i = 0; i < Stages.Count; i++)
        {
            if(Stages[i].IsPointInStage(position))
            {
                return i;
            }
        }

        Debug.LogError("Cannot Find CurrentMapZone");
        return -1;
    }

    public void OnMapChanged(int newMapIndex)
    {
        CurrentStageIndex = newMapIndex;
        CurrentStage = Stages[CurrentStageIndex];
        
        LoadMapsAround(newMapIndex);
        UnloadOtherMaps(newMapIndex);
    }

    private void LoadMapsAround(int mapIndex)
    {
        // 이전, 현재, 다음 맵을 로드
        for (int i = mapIndex - 1; i <= mapIndex + 1; i++)
        {
            if (i > -1 && i < Stages.Count) 
            {
                Debug.Log($"{i} Stage Load -> {Stages[i].name}");
                Stages[i].LoadStage();
            }
        }
    }

    private void UnloadOtherMaps(int mapIndex)
    {
        for (int i = 0; i < Stages.Count; i++)
        {
            if (i < mapIndex - 1 || i > mapIndex + 1)
            {
                Debug.Log($"{i} Stage UnLoad -> {Stages[i].name}");
                Stages[i].UnLoadStage();
            }
        }
    }
}
