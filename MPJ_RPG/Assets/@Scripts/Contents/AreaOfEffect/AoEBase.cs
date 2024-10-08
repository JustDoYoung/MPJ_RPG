using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class AoEBase : BaseObject
{
	[SerializeField]
	protected List<EffectBase> _activeEffects = new List<EffectBase>();

	public Creature Owner;
	protected HashSet<Creature> _targets = new HashSet<Creature>();
	protected SkillBase _skillBase;
	protected AoEData _aoEData;
	protected Vector3 _skillDir;
	protected float _radius;

	private CircleCollider2D _collider;
	private EEffectSize _effectSize;

	protected override void OnDisable()
	{
		base.OnDisable();

		// 1. clear target
		_targets.Clear();

		// 2. clear Effect
		foreach (var effect in _activeEffects)
		{
			if (effect.IsValid())
				effect.ClearEffect(EEffectClearType.TriggerOutAoE);
		}
		_activeEffects.Clear();
	}

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		_collider = GetComponent<CircleCollider2D>();
		_collider.isTrigger = true;
		return true;
	}

	public virtual void SetInfo(int dataId, BaseObject owner, SkillBase skill)
	{
		transform.localEulerAngles = Vector3.zero;
		_aoEData = Managers.Data.AoEDic[dataId];
		Owner = owner as Creature;
		_skillBase = skill;
		_effectSize = skill.SkillData.EffectSize;
		_radius = Util.GetEffectRadius(_effectSize);
		_collider.radius = _radius;
		_skillDir = (Owner.Target.transform.position - Owner.transform.position).normalized;
	}

	protected void ApplyEffectsInRange(int angle)
	{
		// 아군에게 버프 적용
		var allies = FindTargets(angle, true);
		ApplyEffectsToTargets(allies, _aoEData.AllyEffects.ToArray(), false);

		// 적군에게 버프 적용
		var enemies = FindTargets(angle, false);
		ApplyEffectsToTargets(enemies, _aoEData.EnemyEffects.ToArray(), true);
	}

	private List<Creature> FindTargets(int angle, bool isAlly)
	{
		if (angle == 360)
			return Managers.Object.FindCircleRangeTargets(Owner, Owner.transform.position, _radius, isAlly);
		else
			return Managers.Object.FindConeRangeTargets(Owner, _skillDir, _radius, angle, isAlly);
	}

	private void ApplyEffectsToTargets(List<Creature> targets, int[] effects, bool applyDamage)
	{
		foreach (var target in targets)
		{
			Creature t = target as Creature;
			if (t.IsValid() == false)
				continue;

			//AoE에 의한 효과
			t.Effects.GenerateEffects(effects, EEffectSpawnType.Skill, _skillBase);

			//스킬에 의한 효과
			if (applyDamage)
				t.OnDamaged(Owner, _skillBase);
		}
	}

	protected virtual IEnumerator CoReserveDestroy()
	{
		yield return new WaitForSeconds(_aoEData.Duration);
		DestroyAoE();
	}

	protected void DestroyAoE()
	{
		Managers.Object.Despawn(this);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, 3);
	}

}
