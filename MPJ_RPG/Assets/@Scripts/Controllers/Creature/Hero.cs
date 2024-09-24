using Spine;
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

                switch (value)
                {
                    case ECreatureState.Move:
                        Rigidbody.mass = CreatureData.Mass * 5.0f;
                        break;
                    case ECreatureState.Skill:
                        Rigidbody.mass = CreatureData.Mass * 500.0f;
                        break;
                    default:
                        Rigidbody.mass = CreatureData.Mass;
                        break;
                }
                    
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

            if (value)
                ChangeColliderSize(EColliderSize.Big);
            else
                TryResizeCollider();
        }
    }
    #endregion
    protected override void UpdateIdle() 
    {
        print("Hero Idle");
        SetRigidBodyVelocity(Vector2.zero);

        //0. 이동 상태라면 강제 변경
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            CreatureState = ECreatureState.Move;
            return;
        }

        //0. 너무 멀어졌다면 강제로 이동

        //1. 몬스터
        Creature creature = FindClosestInRange(HERO_SEARCH_DISTANCE, Managers.Object.Monsters) as Creature;
        if (creature != null)
        {
            Target = creature;
            CreatureState = ECreatureState.Move;
            HeroMoveState = EHeroMoveState.TargetMonster;
            return;
        }

        //2. 주변 Env 채굴
        Env env = FindClosestInRange(HERO_SEARCH_DISTANCE, Managers.Object.Envs) as Env;
        if (env != null)
        {
            Target = env;
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
            if (Target.IsValid() == false)
            {
                HeroMoveState = EHeroMoveState.None;
                CreatureState = ECreatureState.Move;
                return;
            }

            //현재 사용가능 스킬
            SkillBase skill = Skills.GetReadySkill();
            ChaseOrAttackTarget(HERO_SEARCH_DISTANCE, skill);
            return;
        }

        //2. 주변 Env 채굴
        if (HeroMoveState == EHeroMoveState.CollectEnv)
        {
            // 몬스터가 있으면 포기.
            Creature creature = FindClosestInRange(HERO_SEARCH_DISTANCE, Managers.Object.Monsters) as Creature;
            if (creature != null)
            {
                Target = creature;
                HeroMoveState = EHeroMoveState.TargetMonster;
                CreatureState = ECreatureState.Move;
                return;
            }

            // Env 이미 채집했으면 포기.
            if (Target.IsValid() == false)
            {
                HeroMoveState = EHeroMoveState.None;
                CreatureState = ECreatureState.Move;
                return;
            }


            SkillBase skill = Skills.GetReadySkill();
            ChaseOrAttackTarget(HERO_SEARCH_DISTANCE, skill);
            return;
        }

        // 3. Camp 주변으로 모이기
        if (HeroMoveState == EHeroMoveState.ReturnToCamp)
        {
            Vector3 dir = HeroCampDest.position - transform.position;
            float stopDistanceSqr = HERO_DEFAULT_STOP_RANGE * HERO_DEFAULT_STOP_RANGE;
            if (dir.sqrMagnitude <= stopDistanceSqr)
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
        print("Hero Skill");
        SetRigidBodyVelocity(Vector2.zero);
        
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            CreatureState = ECreatureState.Move;
            return;
        }

        if(Target.IsValid() == false)
        {
            CreatureState = ECreatureState.Move;
            return;
        }
    }
    protected override void UpdateDead() 
    {
        base.UpdateDead();
    }
    #endregion

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        //State
        CreatureState = ECreatureState.Idle;

        //Skill
        Skills = gameObject.GetOrAddComponent<SkillComponent>();
        Skills.SetInfo(this, CreatureData.SkillIdList);
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

    public override void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e)
    {
        base.OnAnimEventHandler(trackEntry, e);
    }

    private void TryResizeCollider()
    {
        // 일단 충돌체 아주 작게.
        ChangeColliderSize(EColliderSize.Small);

        foreach (var hero in Managers.Object.Heros)
        {
            if (hero.HeroMoveState == EHeroMoveState.ReturnToCamp)
                return;
        }

        // ReturnToCamp가 한 명도 없으면 콜라이더 조정.
        foreach (var hero in Managers.Object.Heros)
        {
            // 단 채집이나 전투중이면 스킵.
            if (hero.CreatureState == ECreatureState.Idle)
                hero.ChangeColliderSize(EColliderSize.Big);
        }
    }
}
