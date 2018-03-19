// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: PrefabKeyValuePair.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: zg
// ***************************************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class PrefabKeyValuePair
{
    public ValTypes type = ValTypes.Int;

    public string Key = "keyName";
    public string Str = "";
    public int Int = 0;
    public float Float = 0;


    public enum ValTypes
    {
        Int,
        Float,
        String
    }


    public object Value
    {
        get
        {
            if (type == ValTypes.Int)
            {
                return Int;
            }
            else if (type == ValTypes.Float)
            {
                return Float;
            }
            else if (type == ValTypes.String)
            {
                return Str;
            }
            return null;
        }
    }
}
