using System;
using Data;
using UnityEngine;
using static Define;

public class BuffBase : EffectBase
{
	public override bool Init()
    {
        if (base.Init() == false)
            return false;

        EffectType = EEffectType.Buff;
        return true;
    }

    public override void SetInfo(int templateID, Creature owner, EEffectSpawnType spawnType, SkillBase skill)
    {
        base.SetInfo(templateID, owner, spawnType, skill);

        //이펙트 효과 수치가 음수면 디버프로 설정
        if (EffectData.Amount < 0 || EffectData.PercentAdd < 0)
        {
            EffectType = EEffectType.Debuff;
        }
    }
}