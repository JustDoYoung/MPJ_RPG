using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : SkillBase
{
    public override bool Init()
    {
        if (base.Init() == false) return false;

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

    protected override void OnAnimCompleteHandler(TrackEntry trackEntry)
    {
        if (Owner.Target.IsValid() == false)
            return;

        if (Owner.CreatureState == Define.ECreatureState.Skill)
            Owner.CreatureState = Define.ECreatureState.Move;
    }

    protected override void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.ToString().Contains(SkillData.AnimName))
            OnAttackEvent();
    }

    protected virtual void OnAttackEvent()
    {
        if (Owner.Target.IsValid() == false) return;

        if(SkillData.ProjectileId == 0)
        {
            //Melee
            Owner.Target.OnDamaged(Owner, this);
        }
        else
        {
            //Ranged
        }
    }
}
