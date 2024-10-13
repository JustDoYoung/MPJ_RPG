using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Quest
{
	public QuestSaveData SaveData { get; set; } //유저가 진행한 퀘스트 현황
	private QuestData QuestData { get; set; } //데이터 시트에 명시된 퀘스트 정의

	public List<QuestTask> _questTasks = new List<QuestTask>(); //퀘스트에 포함된 업무목록

	public int TemplateId
	{
		get { return SaveData.TemplateId; }
		set { SaveData.TemplateId = value; }
	}

	public EQuestState State
	{
		get { return SaveData.State; }
		set { SaveData.State = value; }
	}

	public QuestTask GetCurrentTask()
	{
		foreach (QuestTask task in _questTasks)
		{
			if (task.IsCompleted() == false)
				return task;
		}

		return null;
	}

	public Quest(QuestSaveData saveData)
	{
		SaveData = saveData;
		State = EQuestState.None;
		QuestData = Managers.Data.QuestDic[TemplateId];

		_questTasks.Clear();

		for (int i = 0; i < QuestData.QuestTasks.Count; i++)
		{
			_questTasks.Add(new QuestTask(QuestData.QuestTasks[i], saveData.ProgressCount[i]));
		}
	}

	public bool IsCompleted()
	{
		for (int i = 0; i < QuestData.QuestTasks.Count; i++)
		{
			if (i < SaveData.ProgressCount.Count)
				return false;

			QuestTaskData questTaskData = QuestData.QuestTasks[i];

			int progressCount = SaveData.ProgressCount[i];
			if (progressCount < questTaskData.ObjectiveCount)
				return false;
		}

		return true;
	}

	public static Quest MakeQuest(QuestSaveData saveData)
	{
		if (Managers.Data.QuestDic.TryGetValue(saveData.TemplateId, out QuestData questData) == false)
			return null;

		Quest quest = null;

		// TODO?

		quest = new Quest(saveData);

		if (quest != null)
		{
			quest.SaveData = saveData;
		}

		return quest;
	}

	public void OnHandleBroadcastEvent(EBroadcastEventType eventType, int value)
	{
		// ? Task?
		switch (eventType)
		{
			case EBroadcastEventType.ChangeMeat:
				break;
			case EBroadcastEventType.ChangeWood:
				break;
			case EBroadcastEventType.ChangeMineral:
				break;
			case EBroadcastEventType.ChangeGold:
				break;
			case EBroadcastEventType.KillMonster:
				break;
		}
	}
}

