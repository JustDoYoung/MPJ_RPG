using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class QuestTask
{
	public QuestTaskData TaskData { get; private set; }
	public int Count { get; set; }

	public QuestTask(QuestTaskData questTaskData, int count)
	{
		TaskData = questTaskData;
		Count = count;
	}

	public bool IsCompleted()
	{
		if (TaskData.ObjectiveCount <= Count)
			return true;

		return false;
	}

	public void OnHandleBroadcastEvent(EBroadcastEventType eventType, int value)
	{
		switch (TaskData.ObjectiveType)
		{
			case EQuestObjectiveType.KillMonster:
				if (eventType == EBroadcastEventType.KillMonster)
				{
					Count += value;
				}
				break;
			case EQuestObjectiveType.EarnMeat:
			case EQuestObjectiveType.SpendMeat:
				if (eventType == EBroadcastEventType.ChangeMeat)
				{
					Count += value;
				}
				break;
			case EQuestObjectiveType.EarnWood:
			case EQuestObjectiveType.SpendWood:
				if (eventType == EBroadcastEventType.ChangeWood)
				{
					Count += value;
				}
				break;
			case EQuestObjectiveType.EarnMineral:
			case EQuestObjectiveType.SpendMineral:
				if (eventType == EBroadcastEventType.ChangeWood)
				{
					Count += value;
				}
				break;
			case EQuestObjectiveType.EarnGold:
			case EQuestObjectiveType.SpendGold:
				if (eventType == EBroadcastEventType.ChangeGold)
				{
					Count += value;
				}
				break;
			case EQuestObjectiveType.UseItem:
				break;
			case EQuestObjectiveType.Survival:
				break;
			case EQuestObjectiveType.ClearDungeon:
				if (eventType == EBroadcastEventType.DungeonClear)
				{
					Count += value;
				}
				break;
		}
	}
}
