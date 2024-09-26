using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

public abstract class SkillBase : InitBase
{
	public Creature Owner { get; protected set; }

	public Data.SkillData SkillData { get; private set; }

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		return true;
	}

	public virtual void SetInfo(Creature owner, int skillTemplateID)
	{
		Owner = owner;
		SkillData = Managers.Data.SkillDic[skillTemplateID];

		// Register AnimEvent
		if (Owner.SkeletonAnim != null && Owner.SkeletonAnim.AnimationState != null)
		{
			Owner.SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
			Owner.SkeletonAnim.AnimationState.Event += OnAnimEventHandler;
			Owner.SkeletonAnim.AnimationState.Complete -= OnAnimCompleteHandler;
			Owner.SkeletonAnim.AnimationState.Complete += OnAnimCompleteHandler;
		}
	}

	private void OnDisable()
	{
		if (Managers.Game == null)
			return;
		if (Owner.IsValid() == false)
			return;
		if (Owner.SkeletonAnim == null)
			return;
		if (Owner.SkeletonAnim.AnimationState == null)
			return;

		Owner.SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
		Owner.SkeletonAnim.AnimationState.Complete -= OnAnimCompleteHandler;
	}

	public virtual void DoSkill()
	{
		//RemainCoolTime = SkillData.CoolTime;
	}

	protected virtual void GenerateProjectile(Creature owner, Vector3 spawnPos)
	{
		Projectile projectile = Managers.Object.Spawn<Projectile>(spawnPos, SkillData.ProjectileId);

		LayerMask excludeMask = 0;
		excludeMask.AddLayer(Define.ELayer.Default);
		excludeMask.AddLayer(Define.ELayer.Projectile);
		excludeMask.AddLayer(Define.ELayer.Env);
		excludeMask.AddLayer(Define.ELayer.Obstacle);

		switch (owner.CreatureType)
		{
			case Define.ECreatureType.Hero:
				excludeMask.AddLayer(Define.ELayer.Hero);
				break;
			case Define.ECreatureType.Monster:
				excludeMask.AddLayer(Define.ELayer.Monster);
				break;
		}

		projectile.SetSpawnInfo(Owner, this, excludeMask);
	}

	protected abstract void OnAnimEventHandler(TrackEntry trackEntry, Event e);
	protected abstract void OnAnimCompleteHandler(TrackEntry trackEntry);
}
