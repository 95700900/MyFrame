// *************************************************
// Copyright (C): 猫眼视觉
// 文件名:        ResManager.cs
// 作者:          韦伟
// 创建时间:      2019-11-04 07:13:24
// UnityVersion:  2018.2.14f1
// *************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//资源管理器 
public class ResManager : Singleton<ResManager>
{
    //已经加载的资源字典
    private Dictionary<string, AssetInfo> dicAssetInfo = null;

    void Awake()
    {
        if (dicAssetInfo == null)
        {
            dicAssetInfo = new Dictionary<string, AssetInfo>();
        }
    }

    #region 加载实例
    /// <summary>
    /// 加载资源和实例化对象
    /// </summary>
    /// <param name="_path">资源路径</param>
    /// <returns>实例化对象</returns>
    public UnityEngine.Object LoadInstance(string _path)
    {
        UnityEngine.Object obj = Load(_path);
        return Instantiate(obj);
    }
 
    /// <summary>
    /// 通过协程加载实例
    /// </summary>
    /// <param name="_path">资源路径</param>
    /// <param name="_loaded">回调</param>
    public void LoadCoroutineInstance(string _path, Action<UnityEngine.Object> _loaded = null)
    {
        LoadCoroutine(_path, (_obj) => { Instantiate(_obj, _loaded); });
    }

    /// <summary>
    /// 异步加载实例
    /// </summary>
    /// <param name="_path">资源路径</param>
    /// <param name="_loaded"></param>
    /// <param name="_progress"></param>
    public void LoadAsyncInstance(string _path, Action<UnityEngine.Object> _loaded = null, Action<float> _progress = null)
    {
        LoadAsync(_path, (_obj) => { Instantiate(_obj, _loaded); }, _progress);
    }
    #endregion

    /// <summary>
    /// 加载不生成对象，需要自己实例化
    /// </summary>
    /// <param name="path">资源路径</param>
    public UnityEngine.Object Load(string path)
    {
        AssetInfo _assetInfo = GetAssetInfo(path);
        if (_assetInfo != null)
            return _assetInfo.AssetObject;
        return null;
    } 
 
    /// <summary>
    ///  使用协程加载资源
    /// </summary>
    /// <param name="_path">资源路径</param>
    /// <param name="_loaded">为加载完成后回调</param>
    public void LoadCoroutine(string _path, Action<UnityEngine.Object> _loaded = null)
    {
        AssetInfo _assetInfo = GetAssetInfo(_path, _loaded);
        if (null != _assetInfo)
            StartCoroutine(_assetInfo.GetCoroutineObject(_loaded));
    }
 
    /// <summary>
    /// 异步加载
    /// </summary>
    /// <param name="_path">资源地址</param>
    /// <param name="_loaded">加载完成后回调</param>
    /// <param name="_progress">为进度回调</param>
    public void LoadAsync(string _path, Action<UnityEngine.Object> _loaded = null, Action<float> _progress = null)
    {
        AssetInfo _assetInfo = GetAssetInfo(_path, _loaded);
        if (null != _assetInfo)
            StartCoroutine(_assetInfo.GetAsyncObject(_loaded, _progress));
    }


    #region  获取资源和生成对象
    private AssetInfo GetAssetInfo(string _path, Action<UnityEngine.Object> _loaded = null)
    {
        //判断路径是否为空
        if (string.IsNullOrEmpty(_path))
        {
            Debug.Log("错误：空路径。");
            if (_loaded != null)
                _loaded(null);
        }

        //加入资源字典
        AssetInfo _assetInfo = null;
        if (!dicAssetInfo.TryGetValue(_path, out _assetInfo))
        {
            _assetInfo = new AssetInfo();
            _assetInfo.Path = _path;
            dicAssetInfo.Add(_path, _assetInfo);
        }
        //资源引用数量
        _assetInfo.RefCount++;
        return _assetInfo;
    }
    //生成对象
    private UnityEngine.Object Instantiate(UnityEngine.Object _obj, Action<UnityEngine.Object> _loaded = null)
    {
        UnityEngine.Object _retObj = null;
        if (null != _obj)
        {
            _retObj = MonoBehaviour.Instantiate(_obj);
            if (null != _retObj)
            {
                if (null != _loaded)
                {
                    _loaded(_retObj);
                    return null;
                }
                return _retObj;
            }
            else
            {
                Debug.LogError("错误：实例化对象失败。");
            }
        }
        else
        {
            Debug.LogError("错误：资源不存在。");
        }
        return null;
    }
    #endregion

    //销毁并释放资源
    void Destroy()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}