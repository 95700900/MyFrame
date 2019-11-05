// *************************************************
// Copyright (C): 猫眼视觉
// 文件名:        TestDemo.cs
// 作者:          韦伟
// 创建时间:      2019-11-04 08:00:22
// UnityVersion:  2018.2.14f1
// *************************************************
using UnityEngine;
 
public class TestDemo : MonoBehaviour
{
    private void Start()
    {
        LoadFromAssetBundles.Instance.LoadFromFile("AssetBundles");         
    } 
}
