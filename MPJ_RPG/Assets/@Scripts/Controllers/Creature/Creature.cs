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
            if(_creatureState != value)
            {
                _creatureState = value;
                UpdateAnimation();
            }
        }
    }

    public override bool Init()
    {
        if (base.Init() == false) return false;

        ObjectType = EObjectType.Creature;

        return true;
    }

    public virtual void SetInfo(int templateID)
    {
        DataTemplateID = templateID;

        if(CreatureType == ECreatureType.Hero)
            CreatureData = Managers.Data.HeroDic[templateID];
        else if (CreatureType == ECreatureType.Monster)
            CreatureData = Managers.Data.MonsterDic[templateID];

        gameObject.name = $"{CreatureData.DataId}_{CreatureData.DescriptionTextID}";

        //Collider
        Collider.offset = new Vector2(CreatureData.ColliderOffsetX, CreatureData.ColliderOffstY);
        Collider.radius = CreatureData.ColliderRadius;

        // RigidBody
        Rigidbody.mass = CreatureData.Mass;

        // Spine
        SetSpineAnimation(CreatureData.SkeletonDataID, SortingLayers.CREATURE);

        // Register AnimEvent
        if (SkeletonAnim.AnimationState != null)
        {
            SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
            SkeletonAnim.AnimationState.Event += OnAnimEventHandler;
        }

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
    }

    protected override void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case ECreatureState.Idle:
                PlayAnimation(0, AnimName.IDLE, true);
                break;
            case ECreatureState.Move:
                PlayAnimation(0, AnimName.MOVE, true);
                break;
            case ECreatureState.Skill:
                PlayAnimation(0, AnimName.ATTACK_A, true);
                break;
            case ECreatureState.Dead:
                PlayAnimation(0, AnimName.DEAD, true);
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

    public void ChangeColliderSize(EColliderSize size = EColliderSize.Normal)
    {
        switch (size)
        {
            case EColliderSize.Small:
                Collider.radius = CreatureData.ColliderRadius * 0.8f;
                break;
            case EColliderSize.Normal:
                Collider.radius = CreatureData.ColliderRadius;
                break;
            case EColliderSize.Big:
                Collider.radius = CreatureData.ColliderRadius * 1.2f;
                break;
        }
    }

    #region Battle
    protected void ChaseOrAttackTarget(float attackRange, float chaseRange)
    {
        Vector3 dir = (Target.transform.position - transform.position);
        float distToTargetSqr = dir.sqrMagnitude;
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
            SetRigidBodyVelocity(dir.normalized * MoveSpeed);

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

            if (func == null || func.Invoke(obj) == false) 
                continue;

            target = obj;
            bestDistanceSqr = distToTargetSqr;
        }

        return target;
    }

    public override void OnDamaged(BaseObject attacker)
    {
        base.OnDamaged(attacker);

        if (attacker.IsValid() == false)
            return;

        Creature creature = attacker as Creature;
        if (creature == null)
            return;

        // TODO
        float finalDamage = creature.Atk;
        Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp);

        if (Hp <= 0)
        {
            OnDead(attacker);
            CreatureState = ECreatureState.Dead;
        }
    }

    public override void OnDead(BaseObject attacker)
    {
        base.OnDead(attacker);


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
}
