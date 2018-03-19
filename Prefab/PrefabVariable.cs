// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: PrefabVariable.cs
//  Creator 	:  
//  Date		: 2015-6-26
//  Comment		: 
// ***************************************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


[Serializable]
public class PrefabVariable
{
    public string Name;
    private GameObject _gameObject;
    public UnityEngine.Object value;
    public string TypeName;


    public UnityEngine.Object Value
    {
        get
        {
            if (isGameObject)
            {
                return gameObject;
            }
            else
            {
                if (gameObject)
                {
                    return _gameObject.GetComponent(TypeName);
                }
                return null;
            }
        }
    }


    public bool isGameObject
    {
        get
        {
            return TypeName == "GameObject";
        }
    }


    public GameObject gameObject
    {
        get
        {
            if (_gameObject)
            {
                return _gameObject;
            }
            if (isGameObject)
            {
                _gameObject = value as GameObject;
            }
            else
            {
                if (value == null)
                {
                    return null;
                }

                if (value.GetType() == typeof(GameObject))
                {
                    _gameObject = (GameObject)value;
                    return _gameObject;
                }

                MonoBehaviour monoValue = ((MonoBehaviour)value);
                if (monoValue)
                {
                    _gameObject = monoValue.gameObject;
                }
            }
            return _gameObject;
        }
    }


    void Dispose()
    {
        value = null;
    }

}

