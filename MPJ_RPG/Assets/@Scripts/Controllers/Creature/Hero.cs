using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Hero : Creature
{
    Vector2 _moveDir = Vector2.zero;

    public override ECreatureState CreatureState
    {
        get { return _creatureState; }
        set
        {
            if (_creatureState != value)
            {
                base.CreatureState = value;

                if (value == ECreatureState.Move)
                    Rigidbody.mass = CreatureData.Mass;
                else
                    Rigidbody.mass = CreatureData.Mass * 0.1f;
            }
        }
    }

    public override bool Init()
    {
        if (base.Init() == false) return false;

        CreatureType = ECreatureType.Hero;

        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;

        StartCoroutine(CoUpdateAI());

        return true;
    }

    #region AI
    #region AI 필드
    BaseObject _target;
    public float StopDistance { get; private set; } = 1.0f;
    public float SearchDistance { get; private set; } = 8.0f;

    EHeroMoveState _heroMoveState = EHeroMoveState.None;
    public EHeroMoveState HeroMoveState
    {
        get { return _heroMoveState; }
        private set
        {
            _heroMoveState = value;
            switch (value)
            {
                case EHeroMoveState.CollectEnv:
                    NeedArrange = true;
                    break;
                case EHeroMoveState.TargetMonster:
                    NeedArrange = true;
                    break;
                case EHeroMoveState.ForceMove:
                    NeedArrange = true;
                    break;
            }
        }
    }
    public Transform HeroCampDest
    {
        get
        {
            HeroCamp camp = Managers.Object.Camp;
            if (HeroMoveState == EHeroMoveState.ReturnToCamp)
                return camp.Pivot;

            return camp.Destination;
        }
    }
    bool _needArrange = true;
    public bool NeedArrange
    {
        get { return _needArrange; }
        set
        {
            _needArrange = value;

            //if (value)
            //    ChangeColliderSize(EColliderSize.Big);
            //else
            //    TryResizeCollider();
        }
    }
    public float AttackDistance
    {
        get
        {
            float targetRadius = (_target.IsValid() ? _target.ColliderRadius : 0);
            return ColliderRadius + targetRadius + 2.0f;
        }
    }
    #endregion
    protected override void UpdateIdle() 
    {
        //0. 이동 상태라면 강제 변경
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            CreatureState = ECreatureState.Move;
            return;
        }

        //0. 너무 멀어졌다면 강제로 이동

        //1. 몬스터
        Creature creature = FindClosestInRange(3f, Managers.Object.Monsters) as Creature;
        if (creature != null)
        {
            _target = creature;
            CreatureState = ECreatureState.Move;
            HeroMoveState = EHeroMoveState.TargetMonster;
            return;
        }

        //2. 주변 Env 채굴
        Env env = FindClosestInRange(SearchDistance, Managers.Object.Envs) as Env;
        if (env != null)
        {
            _target = env;
            CreatureState = ECreatureState.Move;
            HeroMoveState = EHeroMoveState.CollectEnv;
            return;
        }

        //3. Camp 주변으로 모이기
        if (NeedArrange)
        {
            CreatureState = ECreatureState.Move;
            HeroMoveState = EHeroMoveState.ReturnToCamp;
            return;
        }
    }
    protected override void UpdateMove() 
    {
        //0. 조이스틱을 누르고 있으면 강제 이동
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            Vector3 dir = HeroCampDest.position - transform.position;
            SetRigidBodyVelocity(dir.normalized * MoveSpeed);
            return;
        }

        //1. 몬스터 쫓아가기
        if(HeroMoveState == EHeroMoveState.TargetMonster)
        {
            //몬스터가 죽었으면 포기
            if (_target.IsValid() == false)
            {
                HeroMoveState = EHeroMoveState.None;
                CreatureState = ECreatureState.Move;
                return;
            }

            ChaseOrAttackTarget(AttackDistance, SearchDistance);
            return;
        }

        //2. 주변 Env 채굴
        if (HeroMoveState == EHeroMoveState.CollectEnv)
        {
            // 몬스터가 있으면 포기.
            Creature creature = FindClosestInRange(SearchDistance, Managers.Object.Monsters) as Creature;
            if (creature != null)
            {
                _target = creature;
                HeroMoveState = EHeroMoveState.TargetMonster;
                CreatureState = ECreatureState.Move;
                return;
            }

            // Env 이미 채집했으면 포기.
            if (_target.IsValid() == false)
            {
                HeroMoveState = EHeroMoveState.None;
                CreatureState = ECreatureState.Move;
                return;
            }

            ChaseOrAttackTarget(AttackDistance, SearchDistance);
            return;
        }

        // 3. Camp 주변으로 모이기
        if (HeroMoveState == EHeroMoveState.ReturnToCamp)
        {
            Vector3 dir = HeroCampDest.position - transform.position;
            float stopDistanceSqr = StopDistance * StopDistance;
            if (dir.sqrMagnitude <= StopDistance)
            {
                HeroMoveState = EHeroMoveState.None;
                CreatureState = ECreatureState.Idle;
                NeedArrange = false;
                return;
            }
            else
            {
                // 멀리 있을 수록 빨라짐
                float ratio = Mathf.Min(1, dir.magnitude); // TEMP
                float moveSpeed = MoveSpeed * (float)Math.Pow(ratio, 3);
                SetRigidBodyVelocity(dir.normalized * moveSpeed);
                return;
            }
        }

        // 4. 기타 (누르다 뗐을 때)
        CreatureState = ECreatureState.Idle;
    }
    protected override void UpdateSkill() 
    {

    }
    protected override void UpdateDead() 
    { 

    }
    #endregion

    BaseObject FindClosestInRange(float range, IEnumerable<BaseObject> objs)
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

            target = obj;
            bestDistanceSqr = distToTargetSqr;
        }

        return target;
    }

    void ChaseOrAttackTarget(float attackRange, float chaseRange)
    {
        Vector3 dir = (_target.transform.position - transform.position);
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
                _target = null;
                HeroMoveState = EHeroMoveState.None;
                CreatureState = ECreatureState.Move;
            }
            return;
        }
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        CreatureState = ECreatureState.Idle;
    }

    private void HandleOnJoystickStateChanged(EJoystickState joystickState)
    {
        switch (joystickState)
        {
            case Define.EJoystickState.PointerDown:
                HeroMoveState = EHeroMoveState.ForceMove;
                break;
            case Define.EJoystickState.Drag:
                HeroMoveState = EHeroMoveState.ForceMove;
                break;
            case Define.EJoystickState.PointerUp:
                HeroMoveState = EHeroMoveState.None;
                break;
            default:
                break;
        }
    }
}
