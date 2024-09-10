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
    public float SearchDistance { get; private set; } = 8.0f;
    public float AttackDistance { get; private set; } = 4.0f;
    Creature _target;
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
                _destPos = _initPos + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
                CreatureState = ECreatureState.Move;
                return;
            }
        }

        //Search Player
        {
            Creature target = null;
            float bestDistanceSqr = float.MaxValue;
            float searchDistanceSqr = SearchDistance * SearchDistance;

            foreach (Hero hero in Managers.Object.Heros)
            {
                Vector3 dir = hero.transform.position - transform.position;
                float distToTargetSqr = dir.sqrMagnitude;

                print(distToTargetSqr);

                if (distToTargetSqr > searchDistanceSqr) continue;
                if (distToTargetSqr > bestDistanceSqr) continue;

                target = hero;
                bestDistanceSqr = distToTargetSqr;
            }

            _target = target;

            if (_target != null)
                CreatureState = ECreatureState.Move;
        }
    }
    protected override void UpdateMove()
    {
        print("Move");

        if (_target == null)
        {
            Vector3 dir = _destPos - transform.position;
            float moveDist = Mathf.Min(dir.magnitude, Time.deltaTime * MoveSpeed);
            transform.TranslateEx(dir.normalized * moveDist);

            if(dir.sqrMagnitude <= 0.01f)
                CreatureState = ECreatureState.Idle;
        }
        else
        {
            //Chase
            Vector3 dir = _target.transform.position - transform.position;
            float distToTargetSqr = dir.sqrMagnitude;
            float attackDistanceSqr = AttackDistance * AttackDistance;

            if(distToTargetSqr < attackDistanceSqr)
            {
                //Attack 전환
                CreatureState = ECreatureState.Skill;
                StartWait(2.0f);
            }
            else
            {
                //Chase
                float moveDist = Mathf.Min(dir.magnitude, Time.deltaTime * MoveSpeed);
                transform.TranslateEx(dir.normalized * moveDist);

                //포기 when too far
                float searchDistanceSqr = SearchDistance * SearchDistance;
                if(distToTargetSqr > searchDistanceSqr)
                {
                    _destPos = _initPos;
                    _target = null;
                    CreatureState = ECreatureState.Move;
                }
            }
        }
    }

    protected override void UpdateSkill()
    {
        Debug.Log("Skill");

        //공격 끝날 때까지 대기
        if (_coWait != null)
            return;

        CreatureState = ECreatureState.Move;
    }

    protected override void UpdateDead()
    {
        Debug.Log("Dead");

    }
    #endregion


    void Start()
    {
        _initPos = transform.position;
    }
}
