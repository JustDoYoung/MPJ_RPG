using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public abstract class BaseScene : InitBase
{
    public EScene SceneType { get; protected set; } = EScene.Unknown;

    public override bool Init()
    {
        //이미 초기화된 상태라면
        if(base.Init() == false) return false;

        //초기화 해야하는 상태라면
        Object obj = GameObject.FindObjectOfType<EventSystem>();

        if(obj == null)
        {
            GameObject go = new GameObject("@EventSystem");
            go.AddComponent<EventSystem>(); //UI 입력 이벤트 처리
            go.AddComponent<StandaloneInputModule>(); //마우스, 키보드 입력 처리
        }

        return true;
    }

    public abstract void Clear();
}
