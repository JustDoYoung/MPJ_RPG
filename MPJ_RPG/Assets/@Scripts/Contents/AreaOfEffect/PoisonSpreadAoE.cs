using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class PoisonSpreadAoE : AoEBase
{
	protected override void OnDisable()
	{
		base.OnDisable();
		StopAllCoroutines();
	}

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		return true;
	}

	public override void SetInfo(int dataId, BaseObject owner, SkillBase skill)
	{
		base.SetInfo(dataId, owner, skill);

		SetSpineAnimation(_aoEData.SkeletonDataID, SortingLayers.SPELL_INDICATOR);
		PlayAnimation(0, _aoEData.AnimName, false);

		StartCoroutine(CoReserveDestroy());
		StartCoroutine(CoDetectTargetsPeriodically());
	}

	private IEnumerator CoDetectTargetsPeriodically()
	{
		while (true)
		{
			DetectTargets();
			yield return new WaitForSeconds(1f);
		}
	}

	private void DetectTargets()
	{
		List<Creature> detectedCreatures = new List<Creature>();
		List<Creature> rangeTargets = Managers.Object.FindCircleRangeTargets(Owner, transform.position, _radius);

		foreach (var target in rangeTargets)
		{
			Creature t = target as Creature;

			detectedCreatures.Add(target);

			if (t.IsValid() && _targets.Contains(target) == false)
			{
				_targets.Add(t);

				List<EffectBase> effects = target.Effects.GenerateEffects(_aoEData.EnemyEffects.ToArray(), EEffectSpawnType.External, _skillBase);
				_activeEffects.AddRange(effects);
			}
		}

		// 이전에 탐지되었으나 이제 범위 밖에 있는 Creature 제거
		foreach (var target in _targets.ToArray())
		{
			if (target.IsValid() && detectedCreatures.Contains(target) == false)
			{
				// 범위 밖으로 나간 Creature 처리
				_targets.Remove(target);
				RemoveEffect(target);
			}
		}
	}

	private void RemoveEffect(Creature target)
	{
		List<EffectBase> effectsToRemove = new List<EffectBase>();

		foreach (var effect in _activeEffects)
		{
			if (target.Effects.ActiveEffects.Contains(effect))
			{
				effect.ClearEffect(EEffectClearType.TriggerOutAoE);
				effectsToRemove.Add(effect);
			}
		}

		foreach (var effect in effectsToRemove)
		{
			_activeEffects.Remove(effect);
		}
	}
}
