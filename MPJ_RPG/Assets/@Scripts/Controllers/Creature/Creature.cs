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

	public Data.CreatureData CreatureData { get; private set; }
	public ECreatureType CreatureType { get; protected set; } = ECreatureType.None;

	#region Stats
	public float Hp { get; set; }
	public float MaxHp { get; set; }
	public float MaxHpBonusRate { get; set; }
	public float HealBonusRate { get; set; }
	public float HpRegen { get; set; }
	public float Atk { get; set; }
	public float AttackRate { get; set; }
	public float Def { get; set; }
	public float DefRate { get; set; }
	public float CriRate { get; set; }
	public float CriDamage { get; set; }
	public float DamageReduction { get; set; }
	public float MoveSpeedRate { get; set; }
	public float MoveSpeed { get; set; }
	#endregion

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

		ObjectType = EObjectType.Creature;

		return true;
	}

	public virtual void SetInfo(int templateID)
	{
		DataTemplateID = templateID;

		if (CreatureType == ECreatureType.Hero)
			CreatureData = Managers.Data.HeroDic[templateID];
		else
			CreatureData = Managers.Data.MonsterDic[templateID];

		gameObject.name = $"{CreatureData.DataId}_{CreatureData.DescriptionTextID}";

		// Collider
		Collider.offset = new Vector2(CreatureData.ColliderOffsetX, CreatureData.ColliderOffstY);
		Collider.radius = CreatureData.ColliderRadius;

		// RigidBody
		RigidBody.mass = CreatureData.Mass;

		// Spine
		SkeletonAnim.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>(CreatureData.SkeletonDataID);
		SkeletonAnim.Initialize(true);

		// Register AnimEvent
		if (SkeletonAnim.AnimationState != null)
		{
			SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
			SkeletonAnim.AnimationState.Event += OnAnimEventHandler;
		}

		// Spine SkeletonAnimation은 SpriteRenderer 를 사용하지 않고 MeshRenderer을 사용함.
		// 그렇기떄문에 2D Sort Axis가 안먹히게 되는데 SortingGroup을 SpriteRenderer, MeshRenderer을같이 계산함.
		SortingGroup sg = Util.GetOrAddComponent<SortingGroup>(gameObject);
		sg.sortingOrder = SortingLayers.CREATURE;

		// Skills
		// CreatureData.SkillIdList;

		// Stat
		MaxHp = CreatureData.MaxHp;
		Hp = CreatureData.MaxHp;
		Atk = CreatureData.MaxHp;
		MaxHp = CreatureData.MaxHp;
		MoveSpeed = CreatureData.MoveSpeed;

		// State
		CreatureState = ECreatureState.Idle;

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
	protected virtual void UpdateSkill() { }
	protected virtual void UpdateDead() { }
	#endregion

	#region Battle
	public override void OnDamaged(BaseObject attacker, SkillBase skill)
	{
		base.OnDamaged(attacker, skill);

		if (attacker.IsValid() == false)
			return;

		Creature creature = attacker as Creature;
		if (creature == null)
			return;

		// TEMP
		if (CreatureType == ECreatureType.Hero)
			return;

		float finalDamage = creature.Atk; // TODO
		Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp);

		if (Hp <= 0)
		{
			OnDead(attacker, skill);
			CreatureState = ECreatureState.Dead;
		}
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

	protected void ChaseOrAttackTarget(float chaseRange, SkillBase skill)
	{
		Vector3 dir = (Target.transform.position - transform.position);
		float distToTargetSqr = dir.sqrMagnitude;

		// TEMP
		float attackRange = HERO_DEFAULT_MELEE_ATTACK_RANGE;
		if (skill.SkillData.ProjectileId != 0)
			attackRange = HERO_DEFAULT_RANGED_ATTACK_RANGE;

		float finalAttackRange = attackRange + Target.ColliderRadius + ColliderRadius;
		float attackDistanceSqr = finalAttackRange * finalAttackRange;

		if (distToTargetSqr <= attackDistanceSqr)
		{
			// 공격 범위 이내로 들어왔다면 공격.
			CreatureState = ECreatureState.Skill;
			skill.DoSkill();
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
        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destCellPos, maxDepth);
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
