using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSkill : SkillBase
{
    protected SpellIndicator _indicator;
    protected Vector2 _skillDir;
	protected Define.EIndicatorType _indicatorType = Define.EIndicatorType.Cone;
	protected int _angleRange = 360;

	public override void SetInfo(Creature owner, int skillTemplateID)
	{
		base.SetInfo(owner, skillTemplateID);
	}

	public override void DoSkill()
	{
		base.DoSkill();

		if (Owner.CreatureState != Define.ECreatureState.Skill)
			return;

		_skillDir = (Owner.Target.transform.position - Owner.transform.position).normalized;
	}

    public override void CancelSkill()
    {
        if (_indicator)
            _indicator.Cancel();
    }

    protected void AddIndicatorComponent()
    {
        _indicator = Util.FindChild<SpellIndicator>(gameObject, recursive: true);
        if (_indicator == null)
        {
            GameObject go = Managers.Resource.Instantiate(SkillData.PrefabLabel, gameObject.transform);
            
            _indicator = go.GetOrAddComponent<SpellIndicator>();
        }
    }

    protected void SpawnSpellIndicator()
    {
        if (Owner.Target.IsValid() == false)
            return;

        _indicator.ShowCone(Owner.transform.position, _skillDir.normalized, _angleRange);
    }

    protected override void OnAttackEvent()
    {
        float radius = Util.GetEffectRadius(SkillData.EffectSize);
        List<Creature> targets = Managers.Object.FindConeRangeTargets(Owner, _skillDir, radius, _angleRange);

        foreach (var target in targets)
        {
            if (target.IsValid())
            {
                target.OnDamaged(Owner, this);
            }
        }
    }
}
