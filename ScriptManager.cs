// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: ScriptManager.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using SLua;
using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

[CustomLuaClass]
public class ScriptManager : SingletonMono<ScriptManager>
{
    public delegate void ScriptTickDelegate(int progress);
    public delegate void ScriptCompleteDelegate();

    public ScriptTickDelegate OnScriptTick;
    public ScriptCompleteDelegate OnScriptComplete;

    private LuaSvr luaSvr;


    public override void Initialize()
    {
        LuaState.loaderDelegate += LuaLoader;
        luaSvr = new LuaSvr();
    }

    
    public void Startup()
    {
#if !UNITY_EDITOR
        LuaFileCache.Instance.InitLuaBytecode();
#endif
        luaSvr.init(ScriptLoadTick, ScriptLoadCompleted, LuaSvrFlag.LSF_BASIC);
    }

    public void ReInitLuaBytecode()
    {
        LuaFileCache.Instance.InitLuaBytecode();
    }

    public void ReStart()
    {
        ReInitLuaBytecode();
        GameApplication apl = GameObject.Find("UI Root").GetComponent<GameApplication>();
        apl.ReStart();
    }


    private void ScriptLoadTick(int progress)
    {
        if (OnScriptTick != null)
        {
            OnScriptTick(progress);
        }
    }


    private void ScriptLoadCompleted()
    {
        SetPath("");
        if (OnScriptComplete != null)
        {
            OnScriptComplete();
        }
    }

    private byte[] LuaLoader(string fn)
    {
        string script = "";
        byte[] bytes;
        //如果是Editor
#if UNITY_EDITOR
        if (fn.EndsWith(".lua"))
        {
            script = "/" + fn;
        }
        else
        {
            fn = fn.Replace(".", "/");
            script = "/" + fn + ".lua";
        }
        string path = PathUtility.LuaPath + script;     //获取lua绝对路径
        bytes = LuaFileCache.Instance.LoadFile(path);

#else
        //Bytecode读取
        if (!fn.EndsWith(".lua"))
            script = fn + ".lua";
        else
            script = fn;
        bytes = LuaFileCache.Instance.LoadLuaByteCode(script);
#endif

        return bytes;
    }


    public LuaState Env
    {
        get
        {
            return LuaSvr.mainState;
        }
    }



    public void SetPath(string path)
    {
        Env["package.path"] = Env["package.path"] + ";" + PathUtility.LuaPath + "/?.lua;";
    }



    public object DoStart(string path)
    {
        return luaSvr.start(path);
    }


    public object DoFile(string path)
    {
        try
        {
            return LuaSvr.mainState.doFile(path);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        return null;
    }


    public object CallMethod(LuaTable luaTable, string functionName, params object[] arguments)
    {
        if (luaTable == null)
            return null;

        LuaFunction luaFunction = luaTable[functionName] as LuaFunction;
        if (luaFunction == null)
            return null;

        if (arguments != null)
        {
            return luaFunction.call(arguments);
        }
        else
        {
            luaFunction.call();
            return null;
        }
    }


    //直接资源文件夹获取， 如果发现没有， 则去包体里获取，这里的资源不走引用流程
    [DoNotToLua]
    public T LoadScriptBehaviourFromStartup<T>(string resourcePath) where T : ScriptBehaviour
    {
        UnityEngine.Object resourceObject = ResourceLoader.LoadFromResources(resourcePath);
        GameObject gameObject = GameObject.Instantiate(resourceObject) as GameObject;
        T behaviour = GameObjectUtility.GetIfNotAdd<T>(gameObject);
        return behaviour;
    }

    //直接资源文件夹获取
    public LuaTable LoadScriptBehaviourFromStartup(string resourcePath)
    {
        //UnityEngine.Object resourceObject = ResourceLoader.LoadFromResources(resourcePath);
        //GameObject gameObject = GameObject.Instantiate(resourceObject) as GameObject;
        //ScriptBehaviour behaviour = GameObjectUtility.GetIfNotAdd<ScriptBehaviour>(gameObject);
        ScriptBehaviour behaviour = LoadScriptBehaviourFromStartup<ScriptBehaviour>(resourcePath);
        return behaviour.LuaTable;
    }


    //装载资源并加载脚本
    public LuaTable LoadScriptBehaviourFromResource(string resourceName)
    {
        ScriptBehaviour scriptBehaviour = LoadScriptBehaviourFromResource<ScriptBehaviour>(resourceName);
        LuaTable scriptLua = scriptBehaviour.LuaTable;

        if (scriptLua == null)
        {
            Debug.LogError("resource error : " + resourceName);
            FastLuaUtility.Traceback();
        }
        return scriptLua;
    }

    [DoNotToLua]
    public T LoadScriptBehaviourFromResource<T>(string resourcePath) where T : ScriptBehaviour
    {
        GameObject gameObject = ResourceLoader.Instantiate(resourcePath);
        T behaviour = GameObjectUtility.GetIfNotAdd<T>(gameObject);
        return behaviour;
    }
    

    //实例化脚本
    public ScriptBehaviour LoadScriptBehaviour(GameObject scriptGameObject)
    {
        return LoadScriptBehaviour<ScriptBehaviour>(scriptGameObject);
    }


    [DoNotToLua]
    public T LoadScriptBehaviour<T>(GameObject scriptGameObject) where T : ScriptBehaviour
    {
        T behaviour = GameObjectUtility.GetIfNotAdd<T>(scriptGameObject);
        return behaviour;
    }


    [DoNotToLua]
    public T WrapperScriptBehaviour<T>(GameObject gameObject, string scriptResourcePath = "", string scriptName = null) where T : ScriptBehaviour
    {
        GameObject resourceGo = null;
        if (string.IsNullOrEmpty(scriptResourcePath) == false)
        {
            resourceGo = gameObject;
        }
        else
        {
            resourceGo = GameObjectUtility.Find(scriptResourcePath, gameObject);
        }

        if (string.IsNullOrEmpty(scriptName) == false)
        {
            PrefabBinder prefabBinder = GameObjectUtility.GetIfNotAdd<PrefabBinder>(resourceGo);
            prefabBinder.scriptPath = scriptName;
        }
        bool isActive = resourceGo.activeSelf;
        if (!isActive)
            resourceGo.SetActive(true);                   //保证active状态为false的时候也能获取到组件
        T behaviour = GameObjectUtility.GetIfNotAdd<T>(resourceGo);
        if (!isActive)
            resourceGo.SetActive(false);
        return behaviour;
    }


    //用来给LUA绑定， 直接返回table
    public LuaTable WrapperWindowControl(GameObject gameObject, string scriptName = null)
    {
        if (gameObject == null)
        {
            FastLuaUtility.Traceback();
        }
        ScriptBehaviour windowControl = WrapperScriptBehaviour<ScriptBehaviour>(gameObject, null, scriptName);
        return windowControl.LuaTable;
    }


    [DoNotToLua]
    public T LoadAndWrapperScriptBehaviour<T>(string resourcePath, string scriptName = null) where T : ScriptBehaviour
    {
        GameObject gameObject = ResourceLoader.Instantiate(resourcePath);
        return WrapperScriptBehaviour<T>(gameObject, null, scriptName);
    }


    public string[] GetListFiles(string path)
    {
        List<string> list = new List<string>();
        foreach (var v in LuaFileCache.Instance.BytecodeData.luaList.Keys)
        {
            if (v.IndexOf(path) == 0)
                list.Add(v);
        }
        return list.ToArray();
    }


    //根据不同的处理器返回对应字节码的文件后缀
    public string GetJitFileSuffix()
    {
#if UNITY_EDITOR
        return "";
#else
        if (System.IntPtr.Size == 8)                 //64bit
            return "64";
        else                                                      //32bit
            return "";
#endif

    }


    public override void Destroy()
    {
        luaSvr = null;
    }

    public new static ScriptManager GetInstance()
    {
        return Instance;
    }
}

