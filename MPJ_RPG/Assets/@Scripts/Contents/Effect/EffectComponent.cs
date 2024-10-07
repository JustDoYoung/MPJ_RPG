using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class EffectComponent : MonoBehaviour
{
	public List<EffectBase> ActiveEffects = new List<EffectBase>();
	private Creature _owner;

	public void SetInfo(Creature Owner)
	{
		_owner = Owner;
	}

	public List<EffectBase> GenerateEffects(IEnumerable<int> effectIds, EEffectSpawnType spawnType, SkillBase skill)
	{
		List<EffectBase> generatedEffects = new List<EffectBase>();

		foreach (int id in effectIds)
		{
			string className = Managers.Data.EffectDic[id].ClassName;
			Type effectType = Type.GetType(className);

			if (effectType == null)
			{
				Debug.LogError($"Effect Type not found: {className}");
				return null;
			}

			GameObject go = Managers.Object.SpawnGameObject(_owner.CenterPosition, "EffectBase");			
			go.name = Managers.Data.EffectDic[id].ClassName;
			EffectBase effect = go.AddComponent(effectType) as EffectBase;
			effect.transform.parent = _owner.Effects.transform;
			effect.transform.localPosition = Vector2.zero;
			Managers.Object.Effects.Add(effect);

			ActiveEffects.Add(effect);
			generatedEffects.Add(effect);

			effect.SetInfo(id, _owner, spawnType, skill);
			effect.ApplyEffect();
		}

		return generatedEffects;
	}

	public void RemoveEffects(EffectBase effects)
	{

	}

	public void ClearDebuffsBySkill()
	{
		foreach (var buff in ActiveEffects.ToArray())
		{
			if (buff.EffectType != EEffectType.Buff)
			{
				buff.ClearEffect(EEffectClearType.ClearSkill);
			}
		}
	}
}
