using System;
using Data;
using UnityEngine;
using static Define;

public class DotBase : EffectBase
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    protected override void ProcessDot()
    {
        Owner.HandleDotDamage(this);
    }
}