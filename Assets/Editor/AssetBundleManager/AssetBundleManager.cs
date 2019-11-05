// *************************************************
// Copyright (C): 猫眼视觉
// 文件名:        AssetBundleManager.cs
// 作者:          韦伟
// 创建时间:      2019-11-05 01:18:40
// UnityVersion:  2018.2.14f1
// *************************************************

using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public class AssetBundleManager
{
    [MenuItem("资源打包工具/资源打包")] //特性
    static void BuildAssetBundle()
    {
        string dir = Application.dataPath + "/Resources/AssetBundles/"; //相对路径
        if (!Directory.Exists(dir))   //判断是否存在
        {
            Directory.CreateDirectory(dir);
        }       
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
        AssetDatabase.Refresh();
    } 
}
