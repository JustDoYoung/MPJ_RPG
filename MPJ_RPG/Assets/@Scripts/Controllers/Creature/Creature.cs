using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public class Creature : BaseObject
{
	public BaseObject Target { get; protected set; }
	public SkillComponent Skills { get; protected set; }
	public EffectComponent Effects { get; set; }

	public Data.CreatureData CreatureData { get; private set; }

	#region Stats
	public float Hp { get; set; }
	public CreatureStat MaxHp;
	public CreatureStat Atk;
	public CreatureStat CriRate;
	public CreatureStat CriDamage;
	public CreatureStat ReduceDamageRate;
	public CreatureStat LifeStealRate;
	public CreatureStat ThornsDamageRate; // 쏜즈
	public CreatureStat MoveSpeed;
	public CreatureStat AttackSpeedRate;
	#endregion

	float DistToTargetSqr
	{
		get
		{
			Vector3 dir = (Target.transform.position - transform.position);
			float distToTarget = Math.Max(0, dir.magnitude - Target.ExtraCells * 1f - ExtraCells * 1f); // TEMP
			return distToTarget * distToTarget;
		}
	}

	protected float AttackDistance
	{
		get
		{
			float env = 2.2f;
			if (Target != null && Target.ObjectType == EObjectType.Env)
				return Mathf.Max(env, Collider.radius + Target.Collider.radius + 0.1f);

			float baseValue = CreatureData.AtkRange;
			return baseValue;
		}
	}

	protected ECreatureState _creatureState = ECreatureState.None;
	public virtual ECreatureState CreatureState
	{
		get { return _creatureState; }
		set
		{
			if (_creatureState != value)
			{
				_creatureState = value;
				UpdateAnimation();
			}
		}
	}

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		return true;
	}

	public virtual void SetInfo(int templateID)
	{
		DataTemplateID = templateID;

		if (ObjectType == EObjectType.Hero)
			CreatureData = Managers.Data.HeroDic[templateID];
		else
			CreatureData = Managers.Data.MonsterDic[templateID];

		gameObject.name = $"{CreatureData.DataId}_{CreatureData.DescriptionTextID}";

		// Collider
		Collider.offset = new Vector2(CreatureData.ColliderOffsetX, CreatureData.ColliderOffsetY);
		Collider.radius = CreatureData.ColliderRadius;

		// RigidBody
		RigidBody.mass = 0;

		// Spine
		SetSpineAnimation(CreatureData.SkeletonDataID, SortingLayers.CREATURE);
		
        // Skills
        Skills = gameObject.GetOrAddComponent<SkillComponent>();
        Skills.SetInfo(this, CreatureData);

		// Stat
		Hp = CreatureData.MaxHp;
		MaxHp = new CreatureStat(CreatureData.MaxHp);
		Atk = new CreatureStat(CreatureData.Atk);
		CriRate = new CreatureStat(CreatureData.CriRate);
		CriDamage = new CreatureStat(CreatureData.CriDamage);
		ReduceDamageRate = new CreatureStat(0);
		LifeStealRate = new CreatureStat(0);
		ThornsDamageRate = new CreatureStat(0);
		MoveSpeed = new CreatureStat(CreatureData.MoveSpeed);
		AttackSpeedRate = new CreatureStat(1);

		// State
		CreatureState = ECreatureState.Idle;

		// Effect
		Effects = gameObject.AddComponent<EffectComponent>();
		Effects.SetInfo(this);

		// Map
		StartCoroutine(CoLerpToCellPos());
	}

	protected override void UpdateAnimation()
	{
		switch (CreatureState)
		{
			case ECreatureState.Idle:
				PlayAnimation(0, AnimName.IDLE, true);
				break;
			case ECreatureState.Skill:
				//PlayAnimation(0, AnimName.ATTACK_A, true);
				break;
			case ECreatureState.Move:
				PlayAnimation(0, AnimName.MOVE, true);
				break;
			case ECreatureState.OnDamaged:
				PlayAnimation(0, AnimName.IDLE, true);
				Skills.CurrentSkill.CancelSkill();
				break;
			case ECreatureState.Dead:
				PlayAnimation(0, AnimName.DEAD, true);
				RigidBody.simulated = false;
				break;
			default:
				break;
		}
	}

	#region AI
	public float UpdateAITick { get; protected set; } = 0.0f;

	protected IEnumerator CoUpdateAI()
	{
		while (true)
		{
			switch (CreatureState)
			{
				case ECreatureState.Idle:
					UpdateIdle();
					break;
				case ECreatureState.Move:
					UpdateMove();
					break;
				case ECreatureState.Skill:
					UpdateSkill();
					break;
				case ECreatureState.OnDamaged:
					UpdateOnDamaged();
					break;
				case ECreatureState.Dead:
					UpdateDead();
					break;
			}

			if (UpdateAITick > 0)
				yield return new WaitForSeconds(UpdateAITick);
			else
				yield return null;
		}
	}

	protected virtual void UpdateIdle() { }
	protected virtual void UpdateMove() { }
	protected virtual void UpdateSkill() 
	{
		if (_coWait != null) return;
		if (Target.IsValid() == false || Target.ObjectType == EObjectType.HeroCamp)
		{
			CreatureState = ECreatureState.Idle;
			return;
		}

		float distToTargetSqr = DistToTargetSqr;
		float attackDistanceSqr = AttackDistance * AttackDistance;
		if (distToTargetSqr > attackDistanceSqr)
		{
			CreatureState = ECreatureState.Idle;
			return;
		}

		// DoSkill
		Skills.CurrentSkill.DoSkill();

		LookAtTarget(Target);

		//Temp
		var trackEntry = SkeletonAnim.state.GetCurrent(0);
		float delay = trackEntry.Animation.Duration;

		StartWait(delay);
	}
	protected virtual void UpdateOnDamaged() { }
	protected virtual void UpdateDead() { }
	#endregion

	#region Battle
	public void HandleDotDamage(EffectBase effect)
	{
		if (effect == null)
			return;
		if (effect.Owner.IsValid() == false)
			return;

		// TEMP
		float damage = (Hp * effect.EffectData.PercentAdd) + effect.EffectData.Amount;
		if (effect.EffectData.ClassName.Contains("Heal"))
			damage *= -1f;

		float finalDamage = Mathf.Round(damage);
		Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp.Value);

		Managers.Object.ShowDamageFont(CenterPosition, finalDamage, transform, false);

		// TODO : OnDamaged 통합
		if (Hp <= 0)
		{
			OnDead(effect.Owner, effect.Skill);
			CreatureState = ECreatureState.Dead;
			return;
		}
	}

	public override void OnDamaged(BaseObject attacker, SkillBase skill)
	{
		base.OnDamaged(attacker, skill);

		if (attacker.IsValid() == false)
			return;

		Creature creature = attacker as Creature;
		if (creature == null)
			return;

        float finalDamage = creature.Atk.Value; // TODO
		Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp.Value);

		Managers.Object.ShowDamageFont(CenterPosition, finalDamage, transform, false);

		if (Hp <= 0)
		{
			OnDead(attacker, skill);
			CreatureState = ECreatureState.Dead;
		}

		// 스킬로 생기는 Effect 적용
		if (skill.SkillData.EffectIds != null)
			Effects.GenerateEffects(skill.SkillData.EffectIds.ToArray(), EEffectSpawnType.Skill, skill);

		// AOE로 생기는 Effect
		if (skill != null && skill.SkillData.AoEId != 0)
			skill.GenerateAoE(transform.position);
	}

	public override void OnDead(BaseObject attacker, SkillBase skill)
	{
		base.OnDead(attacker, skill);
	}

	protected BaseObject FindClosestInRange(float range, IEnumerable<BaseObject> objs, Func<BaseObject, bool> func = null)
	{
		BaseObject target = null;
		float bestDistanceSqr = float.MaxValue;
		float searchDistanceSqr = range * range;

		foreach (BaseObject obj in objs)
		{
			Vector3 dir = obj.transform.position - transform.position;
			float distToTargetSqr = dir.sqrMagnitude;

			// 서치 범위보다 멀리 있으면 스킵.
			if (distToTargetSqr > searchDistanceSqr)
				continue;

			// 이미 더 좋은 후보를 찾았으면 스킵.
			if (distToTargetSqr > bestDistanceSqr)
				continue;

			// 추가 조건
			if (func != null && func.Invoke(obj) == false)
				continue;

			target = obj;
			bestDistanceSqr = distToTargetSqr;
		}

		return target;
	}

	protected void ChaseOrAttackTarget(float chaseRange, float attackRange)
	{

        Vector3 dir = (Managers.Object.Camp.Pivot.position - transform.position);
        if (dir.magnitude >= HERO_SEARCH_DISTANCE)
        {
            Target = null;
            CreatureState = ECreatureState.Move;
            return;
        }

        float distToTargetSqr = DistToTargetSqr;
		float attackDistanceSqr = attackRange * attackRange;

		
		if (distToTargetSqr <= attackDistanceSqr)
		{
			// 공격 범위 이내로 들어왔다면 공격.
			CreatureState = ECreatureState.Skill;
			return;
		}
		else
		{
			// 공격 범위 밖이라면 추적.
			FindPathAndMoveToCellPos(Target.transform.position, HERO_DEFAULT_MOVE_DEPTH);


			// 너무 멀어지면 포기.
			float searchDistanceSqr = chaseRange * chaseRange;
			if (distToTargetSqr > searchDistanceSqr)
			{
				Target = null;
				CreatureState = ECreatureState.Move;
			}
			return;
		}
	}
	#endregion

	#region Wait
	protected Coroutine _coWait;

	protected void StartWait(float seconds)
	{
		CancelWait();
		_coWait = StartCoroutine(CoWait(seconds));
	}

	IEnumerator CoWait(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		_coWait = null;
	}

	protected void CancelWait()
	{
		if (_coWait != null)
			StopCoroutine(_coWait);
		_coWait = null;
	}
	#endregion

	#region Misc
	protected bool IsValid(BaseObject bo)
	{
		return bo.IsValid();
	}
	#endregion

	#region Map
	public EFindPathResult FindPathAndMoveToCellPos(Vector3 destWorldPos, int maxDepth, bool forceMoveCloser = false)
	{
		Vector3Int destCellPos = Managers.Map.World2Cell(destWorldPos);
		return FindPathAndMoveToCellPos(destCellPos, maxDepth, forceMoveCloser);
	}

	public EFindPathResult FindPathAndMoveToCellPos(Vector3Int destCellPos, int maxDepth, bool forceMoveCloser = false)
	{
		if (LerpCellPosCompleted == false)
			return EFindPathResult.Fail_LerpCell;

        //A*
        List<Vector3Int> path = Managers.Map.FindPath(this, CellPos, destCellPos, maxDepth);
        if (path.Count < 2)
            return EFindPathResult.Fail_NoPath;

        if (forceMoveCloser)
        {
            Vector3Int diff1 = CellPos - destCellPos;
            Vector3Int diff2 = path[1] - destCellPos;
            if (diff1.sqrMagnitude <= diff2.sqrMagnitude)
                return EFindPathResult.Fail_NoPath;
        }

        Vector3Int dirCellPos = path[1] - CellPos;
        //Vector3Int dirCellPos = destCellPos - CellPos;
        Vector3Int nextPos = CellPos + dirCellPos;

		//nextPos로 이동을 할 예정
		if (Managers.Map.MoveTo(this, nextPos) == false)
			return EFindPathResult.Fail_MoveTo;

		return EFindPathResult.Success;
	}

	public bool MoveToCellPos(Vector3Int destCellPos, int maxDepth, bool forceMoveCloser = false)
	{
		if (LerpCellPosCompleted == false)
			return false;

		return Managers.Map.MoveTo(this, destCellPos);
	}

	//이동
	protected IEnumerator CoLerpToCellPos()
	{
		while (true)
		{
			Hero hero = this as Hero;
			if (hero != null)
			{
				float div = 5;
				Vector3 campPos = Managers.Object.Camp.Destination.transform.position;
				Vector3Int campCellPos = Managers.Map.World2Cell(campPos);
				float ratio = Math.Max(1, (CellPos - campCellPos).magnitude / div);

				LerpToCellPos(CreatureData.MoveSpeed * ratio);
			}
			else
				LerpToCellPos(CreatureData.MoveSpeed);

			//매 프레임
			yield return null;
		}
	}
	#endregion
}
