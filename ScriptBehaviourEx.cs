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
public class ScriptBehaviourEx : ScriptBehaviourBase
{
    private LuaFunction renderImageFunc;

    
    protected override void CacheLuaFunction()
    {
        renderImageFunc = ScriptHelper.GetFunction(luaTable, "onRenderImage");
    }


    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (renderImageFunc != null)
            renderImageFunc.call(LuaTable, sourceTexture, destTexture);
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        if (renderImageFunc != null) renderImageFunc.Dispose();
    }
}

