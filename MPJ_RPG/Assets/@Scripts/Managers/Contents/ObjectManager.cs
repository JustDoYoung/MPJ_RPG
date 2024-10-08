using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class ObjectManager
{
    public HashSet<Hero> Heroes { get; } = new HashSet<Hero>();
    public HashSet<Monster> Monsters { get; } = new HashSet<Monster>();
    public HashSet<Projectile> Projectiles { get; } = new HashSet<Projectile>();
    public HashSet<Env> Envs { get; } = new HashSet<Env>();
    public HeroCamp Camp { get; private set; }
    public HashSet<EffectBase> Effects { get; } = new HashSet<EffectBase>();

    #region Roots
    public Transform GetRootTransform(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
            root = new GameObject { name = name };

        return root.transform;
    }

    public Transform HeroRoot { get { return GetRootTransform("@Heroes"); } }
    public Transform MonsterRoot { get { return GetRootTransform("@Monsters"); } }
    public Transform ProjectileRoot { get { return GetRootTransform("@Projectiles"); } }
    public Transform EnvRoot { get { return GetRootTransform("@Envs"); } }
    #endregion
    public T Spawn<T>(Vector3Int cellPos, int templateID) where T : BaseObject
    {
        Vector3 spawnPos = Managers.Map.Cell2World(cellPos);
        return Spawn<T>(spawnPos, templateID);
    }

    public T Spawn<T>(Vector3 position, int templateID) where T : BaseObject
    {
        string prefabName = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(prefabName);
        go.name = prefabName;
        go.transform.position = position;

        BaseObject obj = go.GetComponent<BaseObject>();

        if (obj.ObjectType == EObjectType.Hero)
        {
            obj.transform.parent = HeroRoot;
            Hero hero = go.GetComponent<Creature>() as Hero;
            Heroes.Add(hero);
            hero.SetInfo(templateID);
        }
        else if (obj.ObjectType == EObjectType.Monster)
        {
            obj.transform.parent = MonsterRoot;
            Monster monster = go.GetComponent<Creature>() as Monster;
            Monsters.Add(monster);
            monster.SetInfo(templateID);
        }
        else if (obj.ObjectType == EObjectType.Projectile)
        {
            obj.transform.parent = ProjectileRoot;

            Projectile projectile = go.GetComponent<Projectile>();
            Projectiles.Add(projectile);

            projectile.SetInfo(templateID);
        }
        else if (obj.ObjectType == EObjectType.Env)
        {
            obj.transform.parent = EnvRoot;

            Env env = go.GetComponent<Env>();
            Envs.Add(env);

            env.SetInfo(templateID);
        }
        else if (obj.ObjectType == EObjectType.HeroCamp)
        {
            Camp = go.GetComponent<HeroCamp>();
        }

        return obj as T;
    }

    public void Despawn<T>(T obj) where T : BaseObject
    {
        EObjectType objectType = obj.ObjectType;

        if (obj.ObjectType == EObjectType.Hero)
        {
            Hero hero = obj.GetComponent<Hero>();
            Heroes.Remove(hero);
        }
        else if (obj.ObjectType == EObjectType.Monster)
        {
            Monster monster = obj.GetComponent<Monster>();
            Monsters.Remove(monster);
        }
        else if (obj.ObjectType == EObjectType.Projectile)
        {
            Projectile projectile = obj as Projectile;
            Projectiles.Remove(projectile);
        }
        else if (obj.ObjectType == EObjectType.Env)
        {
            Env env = obj as Env;
            Envs.Remove(env);
        }
        else if (obj.ObjectType == EObjectType.HeroCamp)
        {
            Camp = null;
        }
        else if (obj.ObjectType == EObjectType.Effect)
        {
            EffectBase effect = obj as EffectBase;
            Effects.Remove(effect);
        }

        Managers.Resource.Destroy(obj.gameObject);
    }

    public void ShowDamageFont(Vector2 position, float damage, Transform parent, bool isCritical = false)
    {
        GameObject go = Managers.Resource.Instantiate("DamageFont", pooling: true);
        DamageFont damageText = go.GetOrAddComponent<DamageFont>();
        damageText.SetInfo(position, damage, parent, isCritical);
    }

    public GameObject SpawnGameObject(Vector3 position, string prefabName)
    {
        GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
        go.transform.position = position;
        return go;
    }

    #region Skill 판정
    public List<Creature> FindCircleRangeTargets(Creature owner, Vector3 startPos, float range, bool isAllies = false)
    {
        HashSet<Creature> targets = new HashSet<Creature>();
        HashSet<Creature> ret = new HashSet<Creature>();

        EObjectType targetType = Util.DetermineTargetType(owner.ObjectType, isAllies);

        if (targetType == EObjectType.Monster)
        {
            var objs = Managers.Map.GatherObjects<Monster>(owner.transform.position, range, range);
            targets.AddRange(objs);
        }
        else if (targetType == EObjectType.Hero)
        {
            var objs = Managers.Map.GatherObjects<Hero>(owner.transform.position, range, range);
            targets.AddRange(objs);
        }

        foreach (var target in targets)
        {
            // 1. 거리안에 있는지 확인
            var targetPos = target.transform.position;
            float distSqr = (targetPos - startPos).sqrMagnitude;

            if (distSqr < range * range)
                ret.Add(target);
        }

        return ret.ToList();
    }

    public List<Creature> FindConeRangeTargets(Creature owner, Vector3 dir, float range, int angleRange, bool isAllies = false)
    {
        HashSet<Creature> targets = new HashSet<Creature>();
        HashSet<Creature> ret = new HashSet<Creature>();

        EObjectType targetType = Util.DetermineTargetType(owner.ObjectType, isAllies);

        if (targetType == EObjectType.Monster)
        {
            var objs = Managers.Map.GatherObjects<Monster>(owner.transform.position, range, range);
            targets.AddRange(objs);
        }
        else if (targetType == EObjectType.Hero)
        {
            var objs = Managers.Map.GatherObjects<Hero>(owner.transform.position, range, range);
            targets.AddRange(objs);
        }

        foreach (var target in targets)
        {
            // 1. 거리안에 있는지 확인
            var targetPos = target.transform.position;
            float distance = Vector3.Distance(targetPos, owner.transform.position);

            if (distance > range)
                continue;

            // 2. 각도 확인
            if (angleRange != 360)
            {
                BaseObject ownerTarget = (owner as Creature).Target;

                // 2. 부채꼴 모양 각도 계산
                float dot = Vector3.Dot((targetPos - owner.transform.position).normalized, dir.normalized);
                float degree = Mathf.Rad2Deg * Mathf.Acos(dot);

                if (degree > angleRange / 2f)
                    continue;
            }

            ret.Add(target);
        }

        return ret.ToList();
    }

    #endregion
}
