using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Env : BaseObject
{
	private Data.EnvData _data;

	private Define.EEnvState _envState = Define.EEnvState.Idle;
	public Define.EEnvState EnvState
	{
		get { return _envState; }
		set
		{
			_envState = value;
			UpdateAnimation();
		}
	}

	#region Stat
	public float Hp { get; set; }
	public float MaxHp { get; set; }
	#endregion

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		ObjectType = Define.EObjectType.Env;
		EnvState = Define.EEnvState.Idle;

		return true;
	}

	public void SetInfo(int templateID)
	{
		DataTemplateID = templateID;
		_data = Managers.Data.EnvDic[templateID];

		// Stat
		Hp = _data.MaxHp;
		MaxHp = _data.MaxHp;

		// Spine
		string ranSpine = _data.SkeletonDataIDs[Random.Range(0, _data.SkeletonDataIDs.Count)];
		SetSpineAnimation(ranSpine, SortingLayers.ENV);
	}

	protected override void UpdateAnimation()
	{
		switch (EnvState)
		{
			case Define.EEnvState.Idle:
				PlayAnimation(0, AnimName.IDLE, true);
				break;
			case Define.EEnvState.OnDamaged:
				PlayAnimation(0, AnimName.DAMAGED, false);
				break;
			case Define.EEnvState.Dead:
				PlayAnimation(0, AnimName.DEAD, false);
				break;
			default:
				break;
		}
	}

	#region Battle
	public override void OnDamaged(BaseObject attacker, SkillBase skill)
	{
		if (EnvState == Define.EEnvState.Dead) return;

		base.OnDamaged(attacker, skill);

		// TODO
		EnvState = Define.EEnvState.OnDamaged;
		float finalDamage = 1;
		Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp);

		if (Hp <= 0)
			OnDead(attacker);
	}

	public override void OnDead(BaseObject attacker)
	{
		base.OnDead(attacker);

		EnvState = Define.EEnvState.Dead;

		//To do: 아이템 떨구기
		Managers.Object.Despawn(this);
	}
	#endregion
}
