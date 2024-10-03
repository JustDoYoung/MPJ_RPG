using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : SkillBase
{
	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		return true;
	}

	public override void SetInfo(Creature owner, int skillTemplateID)
	{
		base.SetInfo(owner, skillTemplateID);
	}

	public override void DoSkill()
	{
		base.DoSkill();

		Owner.CreatureState = Define.ECreatureState.Skill;
		Owner.PlayAnimation(0, SkillData.AnimName, false);

		Owner.LookAtTarget(Owner.Target);
	}

	void PickupTargetAndProcessHit()
	{
	}

	protected override void OnAttackEvent()
	{
		if (Owner.Target.IsValid() == false)
			return;

		if (SkillData.ProjectileId == 0)
		{
			// Melee
			Owner.Target.OnDamaged(Owner, this);
		}
		else
		{
			// Ranged
			GenerateProjectile(Owner, Owner.CenterPosition);
		}
	}
}
