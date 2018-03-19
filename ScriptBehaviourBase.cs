// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: ScriptBehaviourBase.cs
//  Creator 	:  
//  Date		: 2017-8-10
//  Comment		: 
// ***************************************************************


using SLua;
using System.Collections;
using System.IO;
using UnityEngine;


[CustomLuaClass]
public class ScriptBehaviourBase : MonoBehaviour
{
    protected PrefabBinder binder;
    protected LuaTable luaTable;
    
    protected GameObject _gameObject;
    protected Transform _transform;
    private UIScrollView a;
    
    public LuaTable LuaTable
    {
        get { return luaTable; } 
    }


    public void SetPrefabBinder(PrefabBinder prefabBinder)
    {
        binder = prefabBinder;
        if (binder != null)
        {
            binder.Initialize();
            LoadScript();
        }
    }


    public Object Find(string name, bool isThrow=true)
    {
        if (binder == null)
            return null;

        return binder.Find(name, isThrow);
    }


    public System.Object FindConstant(string name)
    {
        if (binder == null)
            return null;

        return binder.FindConstant(name);
    }


    private void LoadScript()
    {
        if (string.IsNullOrEmpty(binder.scriptPath))
            return;

        NewLuaObject();

        RegistLuaVariable();
        CacheLuaFunction();
    }


    protected virtual void NewLuaObject()
    {
        string className = Path.GetFileNameWithoutExtension(binder.scriptPath);
        className = className.Substring(0, 1).ToUpper() + className.Substring(1);

        LuaTable clazz = ScriptManager.Instance.Env[className] as LuaTable;
        LuaTable applicationConfig = ScriptManager.Instance.Env["applicationConfig"] as LuaTable;
        LuaTable gameConfig = applicationConfig["gameConfig"] as LuaTable;

        try
        {
            if (clazz == null)
            {
                ScriptManager.Instance.DoFile(binder.scriptPath);
            }
            clazz = ScriptManager.Instance.Env[className] as LuaTable;

            luaTable = ScriptHelper.CallFunction(clazz, "new", false) as LuaTable;
            Debug.Log("instance lua script : " + binder.scriptPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("load lua file failure : " + binder.scriptPath);
        }
    }


    protected virtual void CacheMonoObject()
    {
        _gameObject = gameObject;
        _transform = transform;
    }


    protected virtual void CacheLuaFunction()
    {
    }


    protected virtual void RegistLuaVariable()
    {
        LuaTable["this"] = this;
        LuaTable["transform"] = transform;
        LuaTable["gameObject"] = gameObject;
        LuaTable["varCache"] = binder.LuaVariableCache;
        LuaTable["constCache"] = binder.LuaKeyValueCache;
    }
    

    protected virtual void Awake()
    {
        CacheMonoObject();

        binder = gameObject.GetComponent<PrefabBinder>();
        SetPrefabBinder(binder);
    }

    
    protected virtual void OnDestroy()
    {
        if (binder != null)             binder.Dispose();
        if (luaTable != null)           luaTable.Dispose();
    }


    protected void RunCoroutine(YieldInstruction instruction, LuaFunction luaFunction, params System.Object[] arguments)
    {
        StartCoroutine(DoCoroutine(instruction, luaFunction, arguments));
    }


    protected void CancelCoroutine(YieldInstruction instruction, LuaFunction luaFunction, params System.Object[] arguments)
    {
        StopCoroutine(DoCoroutine(instruction, luaFunction, arguments));
    }


    private IEnumerator DoCoroutine(YieldInstruction instruction, LuaFunction luaFunction, params System.Object[] arguments)
    {
        yield return instruction;
        if (arguments != null)
        {
            luaFunction.call(arguments);
        }
        else
        {
            luaFunction.call();
        }
    }


    protected void LuaInvoke(float delayTime, LuaFunction luaFunction, params object[] arguments)
    {
        StartCoroutine(DoInvoke(delayTime, luaFunction, arguments));
    }


    private IEnumerator DoInvoke(float delayTime, LuaFunction luaFunction, params object[] arguments)
    {
        yield return new WaitForSeconds(delayTime);

        if (arguments != null)
        {
            luaFunction.call(arguments);
        }
        else
        {
            luaFunction.call();
        }
    }


    protected void CallMethod(string functionName, params object[] arguments)
    {
        ScriptManager.Instance.CallMethod(LuaTable, functionName, arguments);
    }


    public virtual void Dispose()
    {
    }
}

