// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: LuaFileCache.cs
//  Creator 	:  
//  Date		: 
//  Comment		: 
// ***************************************************************


using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct EncryptFormat
{
    public string version;                                       //版本号
    public int luaFileLength;                   //所有脚本文件长度
    public Dictionary<string, LuaFileFormat> luaList;
    public List<string> fileinfoList;
    public int fileStarIndex;
}

public struct LuaFileFormat
{
    public string name;
    public int pos;
    public int len;
    public LuaFileFormat(string name, int pos, int len)
    {
        this.name = name;
        this.pos = pos;
        this.len = len;
    }
}

public class LuaFileCache
{
    private Dictionary<string, byte[]> fileCache = new Dictionary<string, byte[]>();
    private EncryptFormat bytecodeData;
    public EncryptFormat BytecodeData { get { return bytecodeData; } }

    //初始化 存储字节码读取位置和长度
    public void InitLuaBytecode()
    {
        FileStream fs = new FileStream(PathUtility.StoragePath + "/Script/Data"+ScriptManager.Instance.GetJitFileSuffix(), FileMode.OpenOrCreate);
        BinaryReader br = new BinaryReader(fs);
        byte[] fileData = br.ReadBytes((int)fs.Length);
        string fileString = System.Text.Encoding.UTF8.GetString(fileData);
        br.Close();
        fs.Close();

        int index1 = fileString.IndexOf("[xList]:");
        int index2 = fileString.IndexOf("[info]:");
        string temp = fileString.Substring(index1 + 8, index2 - index1 - 9);
        temp = EncryptUtility.DecryptStr(temp);          //解密
        string[] arr = temp.Split('-');
        bytecodeData.luaList = new Dictionary<string, LuaFileFormat>();
        for (int i = 0, len = arr.Length; i < len; ++i)
        {
            string[] d = arr[i].Split('|');
            LuaFileFormat fdata = new LuaFileFormat(d[0], Convert.ToInt32(d[1]), Convert.ToInt32(d[2]));
            bytecodeData.luaList[d[0]] = fdata;
        }
        bytecodeData.fileStarIndex = index2 + 7;

        LogUtility.Log("InitLuaBytecode success!");
    }

    //读取lua源文件（Editor使用）
    public byte[] LoadFile(string path)
    {
        if(fileCache.ContainsKey(path))
            return fileCache[path];

        byte[] bytes = null;
        if (FileUtility.IsFileExist(path))
            bytes = FileUtility.GetBytes(path);
        if (bytes!=null)
            fileCache.Add(path, bytes);
        return bytes;
    }

    //读取lua字节码（非Editor使用）
    public byte[] LoadLuaByteCode(string luaName)
    {
        byte[] bytes = null;
        //读取Data
        FileStream fs = new FileStream(PathUtility.StoragePath + "/Script/Data" + ScriptManager.Instance.GetJitFileSuffix(), FileMode.OpenOrCreate);
        BinaryReader br = new BinaryReader(fs);
        LuaFileFormat fdata = bytecodeData.luaList[luaName];

        fs.Seek((long)bytecodeData.fileStarIndex + fdata.pos, SeekOrigin.Begin);

        bytes = br.ReadBytes(fdata.len);
        if (luaName.Contains("protocal/") || luaName.Contains("conf/"))
            bytes = EncryptUtility.DecryptByte(bytes);
        
        br.Close();
        fs.Close();

        if (bytes != null)
            fileCache[luaName] = bytes;

        else
            throw new Exception("LoadLuabytecode Error! Bytes Is Null!");

        return bytes;
    }
    

    public static readonly LuaFileCache Instance = new LuaFileCache();
}

