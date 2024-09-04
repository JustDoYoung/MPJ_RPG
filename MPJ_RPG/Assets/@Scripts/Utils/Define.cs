using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum EScene
    {
        Unknown,
        TitleScene,
        GameScene,
    }

    public enum EUIEvent
    {
        Click,
        PointerDown,
        PointerUp,
        Drag,
    }

    public enum ESound
    {
        Bgm,
        Effect,
        Max,
    }

    public enum EObjectType {
        None,
        Creature,
        Projectile,
        Env,
    }

    public enum ECreatureType
    {
        None,
        Hero,
        Monster,
        Npc,
    }
    
    public enum ECreatureState
    {
        None,
        Idle,
        Move,
        Skill,
        Dead,
    }

    public enum EJoystickState
    {
        PointerDown,
        PointerUp,
        Drag,
    }
}

public static class AnimName
{
    public static string IDLE = "idle";
    public static string ATTACK_A = "attack_a";
    public static string ATTACK_B = "attack_b";
    public static string MOVE = "move";
    public static string DEAD = "dead";
}
