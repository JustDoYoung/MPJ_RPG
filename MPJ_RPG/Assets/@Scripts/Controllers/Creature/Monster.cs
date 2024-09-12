using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Monster : Creature
{
    public override ECreatureState CreatureState { get
        {
            return base.CreatureState;
        }

        set
        {
            if (_creatureState != value)
            {
                base.CreatureState = value;
                switch (value)
                {
                    case ECreatureState.Idle:
                        UpdateAITick = 0.5f;
                        break;
                    case ECreatureState.Move:
                        UpdateAITick = 0.0f;
                        break;
                    case ECreatureState.Skill:
                        UpdateAITick = 0.0f;
                        break;
                    case ECreatureState.Dead:
                        UpdateAITick = 1.0f;
                        break;
                }
            }
        }
    }

    public override bool Init()
    {
        if (base.Init() == false) return false;

        CreatureType = ECreatureType.Monster;

        StartCoroutine(CoUpdateAI());

        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);


    }

    #region AI
    public float AttackDistance { get; private set; } = 4.0f;
    Vector3 _destPos;
    Vector3 _initPos;

    protected override void UpdateIdle() {
        print("Idle");

        //Patrol
        {
            int patrolPercent = 10;
            int rand = Random.Range(0, 100);
            if(rand <= patrolPercent)
            {
                print("Patrol");
                _destPos = _initPos + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
                CreatureState = ECreatureState.Move;
                return;
            }
        }

        //Search Player
        Creature creature = FindClosestInRange(MONSTER_SEARCH_DISTANCE, Managers.Object.Heros, IsValid) as Creature;

        if(creature != null)
        {
            print("Search Player");
            Target = creature;
            CreatureState = ECreatureState.Move;
            return;
        }
    }

    protected override void UpdateMove()
    {
        //print("Move");

        if (Target == null)
        {
            Vector3 dir = _destPos - transform.position;
            SetRigidBodyVelocity(dir.normalized * MoveSpeed);

            if(dir.sqrMagnitude <= 0.01f)
                CreatureState = ECreatureState.Idle;
        }
        else
        {
            //Chase
            Vector3 dir = Target.transform.position - transform.position;
            float distToTargetSqr = dir.sqrMagnitude;
            float attackDistanceSqr = AttackDistance * AttackDistance;

            //공격 범위 이내
            if(distToTargetSqr < attackDistanceSqr)
            {
                //Attack 전환
                CreatureState = ECreatureState.Skill;
                StartWait(2.0f);
            }
            //공격 범위 밖
            else
            {
                //Chase
                ChaseOrAttackTarget(AttackDistance, MONSTER_SEARCH_DISTANCE);

                // 타겟이 유효하지 않으면 포기(죽을 때)
                if (Target.IsValid() == false)
                {
                    Target = null;
                    _destPos = _initPos;
                    return;
                }
            }
        }
    }

    protected override void UpdateSkill()
    {
        //Debug.Log("Skill");

        //공격 끝날 때까지 대기
        if (_coWait != null)
            return;

        CreatureState = ECreatureState.Move;
    }

    protected override void UpdateDead()
    {
        //Debug.Log("Dead");

    }
    #endregion

    #region Battle
    public override void OnDamaged(BaseObject attacker)
    {
        base.OnDamaged(attacker);
    }

    public override void OnDead(BaseObject attacker)
    {
        base.OnDead(attacker);

        Managers.Object.DeSpawn(this);
    }
    #endregion


    void Start()
    {
        _initPos = transform.position;
    }
}
