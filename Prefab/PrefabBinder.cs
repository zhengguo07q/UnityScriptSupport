// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: PrefabBinder.cs
//  Creator 	:  
//  Date		: 2016-2-1
//  Comment		: zg
// ***************************************************************

using SLua;
using System;
using System.Collections.Generic;
using UnityEngine;


[CustomLuaClass]
public class PrefabBinder :MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    public List<PrefabVariable> Variables = new List<PrefabVariable>();

    [HideInInspector]
    [SerializeField]
    public List<PrefabKeyValuePair> KeyMap = new List<PrefabKeyValuePair>();

    [HideInInspector]
    [SerializeField]
    public string scriptPath;

    private Dictionary<string, PrefabVariable> variableCaches;
    private Dictionary<string, PrefabKeyValuePair> keyVauleCaches;

    private Dictionary<object, string> buttonCaches;

    public LuaTable LuaVariableCache { set; get; }
    public LuaTable LuaKeyValueCache { set; get; }


    public void Initialize()
    {
        if (string.IsNullOrEmpty(scriptPath) == false)
        {
            LuaVariableCache = new LuaTable(ScriptManager.Instance.Env);
            LuaKeyValueCache = new LuaTable(ScriptManager.Instance.Env);
        }

        variableCaches = new Dictionary<string, PrefabVariable>();
        buttonCaches = new Dictionary<object, string>();
            for (int i = 0; i < Variables.Count; i++)
        {
            PrefabVariable variable = Variables[i];
            variableCaches.Add(variable.Name, variable);

            if (string.IsNullOrEmpty(scriptPath) == false)
            {
                LuaVariableCache[variable.Name] = variable.Value;
            }

            object button = variable.Value;
            if (button is UIButton||button is UIToggle)
            {
                buttonCaches.Add(button, variable.Name);
            }
        }

        keyVauleCaches = new Dictionary<string, PrefabKeyValuePair>();
        for (int i = 0; i < KeyMap.Count; i++)
        {
            PrefabKeyValuePair keyValuePair = KeyMap[i];
            keyVauleCaches.Add(keyValuePair.Key, keyValuePair);

            if (string.IsNullOrEmpty(scriptPath) == false)
            {
                LuaKeyValueCache[keyValuePair.Key] = keyValuePair.Value;
            }
        }
    }


    public UnityEngine.Object Find(string name, bool isThrow=true)
    {
        PrefabVariable variable;
        if (variableCaches.TryGetValue(name, out variable))
        {
            return variable.Value;
        }
        else
        {
            if(isThrow == true)
            {
                throw new Exception("gameobject : " + gameObject.name + ",  find error bind variable name : " + name);
            }
            return null;
        }
    }


    public System.Object FindConstant(string name)
    {
        PrefabKeyValuePair keyValuePair;
        if (keyVauleCaches.TryGetValue(name, out keyValuePair))
        {
            return keyValuePair.Value;
        }
        else
        {
            throw new Exception("gameobject : " + gameObject.name + ",  find error bind variable name : " + name);
        }
    }


    public string FindReverseButton(UIWidgetContainer button)
    {
        string name;
        if (buttonCaches.TryGetValue(button, out name))
        {
            return name;
        }
        else
        {
            throw new Exception("gameobject : " + gameObject.name + ",  find error bind button object : " + button.gameObject.name);
        }
    }


    public void AddButtonListener(UIEventListener.VoidDelegate callback)
    {
        for (int i = 0; i < variableCaches.Count; i++)
        {
            PrefabVariable variable = Variables[i];
           
            if(variable.Value!=null&&( variable.Value is UIButton || variable.Value is UIToggle))
            {
                UIWidgetContainer button = variable.Value as UIToggle;
                if(button == null)
                    button = variable.Value as UIButton;
                if(button == null)
                    continue;
                UIEventListener listener = UIEventListener.Get(button.gameObject);
                listener.onClick += callback;
            }
        }
    }


    public void RemoveButtonListener(UIEventListener.VoidDelegate callback)
    {
        for (int i = 0; i < Variables.Count; i++)
        {
            PrefabVariable variable = Variables[i];
            if (variable.Value != null && (variable.Value is UIButton || variable.Value is UIToggle))
            {
                UIWidgetContainer button = variable.Value as UIToggle;
                if (button == null)
                    button = variable.Value as UIButton;
                if (button == null)
                    continue;
                UIEventListener listener = UIEventListener.Get(button.gameObject);
                listener.onClick -= callback;
            }
        }
    }

    public void Dispose()
    {
        if (LuaVariableCache != null) LuaVariableCache.Dispose();
        if (LuaKeyValueCache != null) LuaKeyValueCache.Dispose();
    }
}


