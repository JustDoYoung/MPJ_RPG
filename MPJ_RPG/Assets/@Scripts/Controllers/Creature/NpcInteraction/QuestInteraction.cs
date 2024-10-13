using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class QuestInteraction : INpcInteraction
{
	private Npc _owner;
	private Quest _quest;

	public void SetInfo(Npc owner)
	{
		_owner = owner;

		if (Managers.Quest.AllQuests.TryGetValue(owner.Data.QuestDataId, out _quest) == false)
			return;
	}

	public bool CanInteract()
	{
		if (_quest == null)
			return false;
		if (_quest.State == EQuestState.Rewarded)
			return false;

		return true;
	}

	public void HandleOnClickEvent()
	{
		QuestTask questTask = _quest.GetCurrentTask();
		if (questTask == null)
			return;

		switch (questTask.TaskData.ObjectiveType)
		{
			case EQuestObjectiveType.SpendMeat:
			case EQuestObjectiveType.EarnMeat:

				_quest.State = EQuestState.Processing;
				if (Managers.Game.SpendResource(EResourceType.Meat, questTask.TaskData.ObjectiveCount) == false)
				{
					Debug.Log("Meat 부족");
				}
				break;

			case EQuestObjectiveType.KillMonster:
				break;
			case EQuestObjectiveType.ClearDungeon:
				break;
		}
	}	
}
