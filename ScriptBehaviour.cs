// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: ScriptBehaviour.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using SLua;
using System.Collections;
using System.IO;
using UnityEngine;


[CustomLuaClass]
public class ScriptBehaviour : ScriptBehaviourBase
{
    private LuaFunction awakeFunc;
    private LuaFunction startFunc;
    private LuaFunction updateFunc;
    private LuaFunction fixedUpdateFunc;
    private LuaFunction lateUpdateFunc;
    private LuaFunction onEnableFunc;
    private LuaFunction onDisableFunc;
    private LuaFunction onDestroyFunc;
    private LuaFunction deleteFunc;

    
    protected override void CacheLuaFunction()
    {
        binder.AddButtonListener(ClickButtonDelegete);

        awakeFunc = ScriptHelper.GetFunction(luaTable, "awake");
        startFunc = ScriptHelper.GetFunction(luaTable, "start");
        updateFunc = ScriptHelper.GetFunction(luaTable, "update");
        fixedUpdateFunc = ScriptHelper.GetFunction(luaTable, "fixedUpdate");
        lateUpdateFunc = ScriptHelper.GetFunction(luaTable, "lateUpdate");
        onEnableFunc = ScriptHelper.GetFunction(luaTable, "onEnable");
        onDisableFunc = ScriptHelper.GetFunction(luaTable, "onDisable");
        onDestroyFunc = ScriptHelper.GetFunction(luaTable, "onDestroy");
        deleteFunc = ScriptHelper.GetFunction(luaTable, "delete");
    }
    

    public void ClickButtonDelegete(GameObject go)
    {
        UIWidgetContainer button = GameObjectUtility.FindAndGet<UIButton>("", go);
        if(button == null)
            button = GameObjectUtility.FindAndGet<UIToggle>("", go);
        string btnName = binder.FindReverseButton(button);
        ScriptHelper.CallFunction(luaTable, "clickButton", luaTable, go, btnName);
    }

    protected override void Awake()
    {
        base.Awake();

        if (awakeFunc != null)
            awakeFunc.call(LuaTable);
    }


    protected virtual void Start()
    {
        if (startFunc != null)
            startFunc.call(LuaTable);
    }


    protected virtual void Update()
    {
        if (updateFunc != null)
            updateFunc.call(LuaTable);
    }


    protected virtual void FixedUpdate()
    {
        if (fixedUpdateFunc != null)
            fixedUpdateFunc.call(LuaTable);
    }


    protected virtual void LateUpdate()
    {
        if (lateUpdateFunc != null)
            lateUpdateFunc.call(LuaTable);
    }


    protected virtual void OnEnable()
    {
        if (onEnableFunc != null)
            onEnableFunc.call(LuaTable);
    }


    protected virtual void OnDisable()
    {
        if (onDisableFunc != null)
            onDisableFunc.call(LuaTable);
    }


    protected override void OnDestroy()
    {
        if (onDestroyFunc != null) onDestroyFunc.call(luaTable);
        if (deleteFunc != null) deleteFunc.call(luaTable);

        base.OnDestroy();

        if (awakeFunc != null)          awakeFunc.Dispose();
        if (startFunc != null)          startFunc.Dispose();
        if (updateFunc != null)         updateFunc.Dispose();
        if (fixedUpdateFunc != null)    fixedUpdateFunc.Dispose();
        if (lateUpdateFunc != null)     lateUpdateFunc.Dispose();
        if (onEnableFunc != null)       onEnableFunc.Dispose();
        if (onDisableFunc != null)      onDisableFunc.Dispose();
        if (onDestroyFunc != null)      onDestroyFunc.Dispose();
        if (deleteFunc != null)         deleteFunc.Dispose();        
    }
    

    public override void Dispose()
    {
        binder.RemoveButtonListener(ClickButtonDelegete);
    }
}

