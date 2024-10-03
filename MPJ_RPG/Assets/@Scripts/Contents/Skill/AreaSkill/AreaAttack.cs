using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAttack : AreaSkill
{
	public override void SetInfo(Creature owner, int skillTemplateID)
	{
		base.SetInfo(owner, skillTemplateID);

		_angleRange = 90;

        AddIndicatorComponent();

        if (_indicator != null)
            _indicator.SetInfo(Owner, SkillData, Define.EIndicatorType.Cone);
    }

	public override void DoSkill()
	{
		base.DoSkill();

        SpawnSpellIndicator();
    }
}
