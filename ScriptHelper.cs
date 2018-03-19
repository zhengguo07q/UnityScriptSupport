// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: ScriptHelper.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using SLua;
using System;


public static class ScriptHelper
{
    public static LuaFunction GetFunction(LuaTable table, string functionName, bool isThrow=false)
    {
        LuaFunction retFunction = null;

        object function = table[functionName];
        if (function != null)
        {
            retFunction = function as LuaFunction;
        }
        else
        {
            if (isThrow)
            {
                throw new Exception("lua function not exist : " + functionName);
            }
        }
        return retFunction;
    }


    public static object CallFunction(LuaTable table, string functionName, bool isThrow=false)
    {
        LuaFunction function = GetFunction(table, functionName, isThrow);
        return function.call();
    }


    public static object CallFunction(LuaTable table, string functionName, params object[] args)
    {
        LuaFunction function = GetFunction(table, functionName);
        return function.call(args);
    }


    public static object CallFunction(LuaTable table, string functionName, object a1)
    {
        LuaFunction function = GetFunction(table, functionName);
        return function.call(a1);
    }


    public static object CallFunction(LuaTable table, string functionName, object a1, object a2)
    {
        LuaFunction function = GetFunction(table, functionName);
        return function.call(a1, a2);
    }


    public static object CallFunction(LuaTable table, string functionName, object a1, object a2, object a3)
    {
        LuaFunction function = GetFunction(table, functionName);
        return function.call(a1, a2, a3);
    }


    public static object CallFunction(LuaTable table, string functionName, object a1, object a2, object a3, object a4)
    {
        LuaFunction function = GetFunction(table, functionName);
        return function.call(a1, a2, a3, a4);
    }

}

