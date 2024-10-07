using System;
using Data;
using UnityEngine;

public class MoveSpeedBuff : BuffBase
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
        AddModifier(Owner.MoveSpeed, this);
    }

    public override bool ClearEffect(Define.EEffectClearType clearType)
    {
        if (base.ClearEffect(clearType) == true)
            RemoveModifier(Owner.MoveSpeed, this);
        return true;
    }
}