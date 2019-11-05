// *************************************************
// Copyright (C): 猫眼视觉
// 文件名:        AssetInfo.cs
// 作者:          韦伟
// 创建时间:      2019-11-04 07:12:08
// UnityVersion:  2018.2.14f1
// *************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AssetInfo
{
    //资源对象
    private UnityEngine.Object mObject;
    //资源类型
    public Type AssetType { get; set; }
    //路径
    public string Path { get; set; }
    //读取次数
    public int RefCount { get; set; }
    //是否已加载
    public bool IsLoaded
    {
        get
        {
            return mObject != null ? true : false;
        }
    }

    public UnityEngine.Object AssetObject
    {
        get
        {
            if (mObject == null)
            {
                ResourcesLoad();
            }
            return mObject;
        }
    }

    //协程加载
    public IEnumerator GetCoroutineObject(Action<UnityEngine.Object> _loaded)
    {
        while (true)
        {
            yield return null;
            if (!mObject)
            {
                ResourcesLoad();
                yield return null;
            }
            else
            {
                if (_loaded != null)
                    _loaded(mObject);
            }
            yield break;
        }
    }

    //加载
    private void ResourcesLoad()
    {
        try
        {
            mObject = Resources.Load(Path);
            if (!mObject)
                Debug.Log("资源加载失败 : " + Path);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    /// <summary>
    /// 异步加载
    /// </summary>
    /// <param name="_loaded">加载回调</param>
    /// <param name="_progress">进度回调</param>
    /// <returns>ResourceRequest</returns>
    public IEnumerator GetAsyncObject(Action<UnityEngine.Object> _loaded, Action<float> _progress = null)
    {
        if (mObject != null)
        {
            _loaded(mObject);
            yield break;
        }

        ResourceRequest _resRequest = Resources.LoadAsync(Path);

        while (_resRequest.isDone)
        {
            if (_progress != null)
                _progress(_resRequest.progress);
            yield return null;
        }

        mObject = _resRequest.asset;
        if (_loaded != null)
        {
            _loaded(mObject);
        }

        yield return _resRequest;
    }
}
