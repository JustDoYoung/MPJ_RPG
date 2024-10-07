using System;
using Data;
using UnityEngine;

public class CleanDebuff : BuffBase
{
	public override bool Init()
    {
        if (base.Init() == false)
            return false;
        return true;
    }
    
    public override void ApplyEffect()
    {
        //Base 호출X
        Owner.Effects.ClearDebuffsBySkill();
        ClearEffect(Define.EEffectClearType.TimeOut);
    }

}