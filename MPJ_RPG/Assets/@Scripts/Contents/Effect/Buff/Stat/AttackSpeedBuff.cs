using System;
using Data;
using UnityEngine;
using static Define;

public class AttackSpeedBuff : BuffBase
{
	public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }
   
    public override void ApplyEffect()
    {
        base.ApplyEffect();
        AddModifier(Owner.AttackSpeedRate,this);
    }

    public override bool ClearEffect(EEffectClearType clearType)
    {
        if(base.ClearEffect(clearType) == true)
            RemoveModifier(Owner.AttackSpeedRate, this);

        return true;
    }

}