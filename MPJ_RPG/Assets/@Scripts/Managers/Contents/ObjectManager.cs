using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectManager
{
    public HashSet<Hero> Heros { get; } = new HashSet<Hero>();
    public HashSet<Monster> Monsters { get; } = new HashSet<Monster>();
    public HashSet<Env> Envs { get; } = new HashSet<Env>();

    #region Root
    public Transform HeroRoot { get { return GetRootTransform("@Heros"); } }
    public Transform MonsterRoot { get { return GetRootTransform("@Mosnters"); } }
    public Transform EnvRoot { get { return GetRootTransform("@Envs"); } }

    public Transform GetRootTransform(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
            root = new GameObject(name);

        return root.transform;
    }
    #endregion

    public T Spawn<T>(Vector3 position, int templateID) where T : BaseObject
    {
        string prefabName = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(prefabName);
        go.name = prefabName;
        go.transform.position = position;

        BaseObject obj = go.GetComponent<BaseObject>();

        switch (obj.ObjectType)
        {
            case EObjectType.Creature:
                {
                    //Data Check
                    if (templateID != 0 && Managers.Data.CreatureDic.TryGetValue(templateID, out Data.CreatureData data) == false)
                    {
                        Debug.LogError($"ObjectManager Spawn Creature Failed! TryGetValue TemplateID : {templateID}");
                        return null;
                    }

                    Creature creature = go.GetComponent<Creature>();
                    SpawnCreature(creature);
                    creature.SetInfo(templateID);
                    break;
                }
            case EObjectType.Projectile:
                break;
            case EObjectType.Env:
                {
                    //Data Check
                    if (templateID != 0 && Managers.Data.EnvDic.TryGetValue(templateID, out Data.EnvData data) == false)
                    {
                        Debug.LogError($"ObjectManager Spawn Creature Failed! TryGetValue TemplateID : {templateID}");
                        return null;
                    }

                    obj.transform.parent = EnvRoot;
                    Env env = obj.GetComponent<Env>();
                    Envs.Add(env);
                    env.SetInfo(templateID);
                    break;
                }
            default:
                break;
        }

        return obj as T;
    }

    public void DeSpawn<T>(T obj) where T : BaseObject
    {
        switch (obj.ObjectType)
        {
            case EObjectType.Creature:
                {
                    Creature creature = obj.gameObject.GetComponent<Creature>();
                    DeSpawnCreature(creature);
                }
                break;
            case EObjectType.Projectile:
                break;
            case EObjectType.Env:
                {
                    Envs.Remove(obj as Env);
                }
                break;
            default:
                break;
        }

        Managers.Resource.Destroy(obj.gameObject);
    }

    private void SpawnCreature(Creature creature)
    {
        switch (creature.CreatureType)
        {
            case ECreatureType.Hero:
                creature.transform.parent = HeroRoot;
                Hero hero = creature as Hero;
                Heros.Add(hero);
                break;
            case ECreatureType.Monster:
                creature.transform.parent = MonsterRoot;
                Monster monster = creature as Monster;
                Monsters.Add(monster);
                break;
            case ECreatureType.Npc:
                break;
            default:
                break;
        }
    }

    private void DeSpawnCreature(Creature creature)
    {
        switch (creature.CreatureType)
        {
            case ECreatureType.Hero:
                Hero hero = creature as Hero;
                Heros.Remove(hero);
                break;
            case ECreatureType.Monster:
                Monster monster = creature as Monster;
                Monsters.Remove(monster);
                break;
            case ECreatureType.Npc:
                break;
            default:
                break;
        }
    }
}
