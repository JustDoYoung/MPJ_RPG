using System;
using Data;
using UnityEngine;
using static Define;

public class ThornsBuff : BuffBase
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
        AddModifier(Owner.ThornsDamageRate, this);
    }

    public override bool ClearEffect(EEffectClearType clearType)
    {
        if (base.ClearEffect(clearType) == true) 
            RemoveModifier(Owner.ThornsDamageRate, this);

        return true;
    }
}