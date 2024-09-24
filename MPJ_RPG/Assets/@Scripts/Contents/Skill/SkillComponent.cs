using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillComponent : InitBase
{
	public List<SkillBase> SkillList { get; } = new List<SkillBase>();

	Creature _owner;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		return true;
	}

	public void SetInfo(Creature owner, List<int> skillTemplateIDs)
	{
		_owner = owner;

		foreach (int skillTemplateID in skillTemplateIDs)
			AddSkill(skillTemplateID);
	}

	public void AddSkill(int skillTemplateID = 0)
	{
		string className = Managers.Data.SkillDic[skillTemplateID].ClassName;

		SkillBase skill = gameObject.AddComponent(Type.GetType(className)) as SkillBase;
		if (skill == null)
			return;

		skill.SetInfo(_owner, skillTemplateID);

		SkillList.Add(skill);
	}

	public SkillBase GetReadySkill()
	{
		// TEMP
		return SkillList[0];
	}
}
