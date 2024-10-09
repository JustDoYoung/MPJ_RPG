using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameManager
{
    #region Hero
    private Vector2 _moveDir;
    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set
        {
            _moveDir = value;
            OnMoveDirChanged?.Invoke(value);
        }
    }

    private EJoystickState _joystickState;
    public EJoystickState JoystickState
    {
        get { return _joystickState; }
        set
        {
            _joystickState = value;
            OnJoystickStateChanged?.Invoke(value);
        }
    }
    #endregion

    #region Teleport
    public void TeleportHeroes(Vector3 position)
    {
        TeleportHeroes(Managers.Map.World2Cell(position));
    }

    public void TeleportHeroes(Vector3Int cellPos)
    {
        //foreach (var hero in Managers.Object.Heroes)
        //{
        //    Vector3Int randCellPos = Managers.Game.GetNearbyPosition(hero, cellPos);
        //    Managers.Map.MoveTo(hero, randCellPos, forceMove: true);
        //}

        //Vector3 worldPos = Managers.Map.Cell2World(cellPos);
        //Managers.Object.Camp.ForceMove(worldPos);
        //Camera.main.transform.position = worldPos;
    }
    #endregion

    #region Action
    public event Action<Vector2> OnMoveDirChanged;
    public event Action<EJoystickState> OnJoystickStateChanged;
    #endregion
}
