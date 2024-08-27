using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitBase : MonoBehaviour
{
    protected bool _init = false;

    //초기화를 최초 1회만 실행하도록 구현
    public virtual bool Init()
    {
        if (_init) return false;

        _init = true;
        return true;
    }

    private void Awake()
    {
        Init();
    }
}
