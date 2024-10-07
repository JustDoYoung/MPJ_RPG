using System;
using Data;
using UnityEngine;
using static Define;

public class AttackBuff : BuffBase
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
        AddModifier(Owner.Atk,this);
    }

    public override bool ClearEffect(EEffectClearType clearType)
    {
        if(base.ClearEffect(clearType) == true)
            RemoveModifier(Owner.Atk, this);

        return true;
    }

}