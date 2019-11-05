// *************************************************
// Copyright (C): 猫眼视觉
// 文件名:        LoadSceneManager.cs
// 作者:          韦伟
// 创建时间:      2019-11-04 05:18:31
// UnityVersion:  2018.2.14f1
// *************************************************
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 场景加载
/// </summary>
public class LoadSceneManager : Singleton<LoadSceneManager>
{

    //上一个场景
    int backIndex = 0;
    float progressValue = 0;
    string nextSceneName = "";
    AsyncOperation async = null;

    /// <summary>
    /// 使用场景ID异步加载场景
    /// </summary>
    /// <param name="_sceneID">场景ID</param>
    /// <returns>加载进度</returns>
    public float LoadSceneAsync(int _sceneID = 0)
    {
       
        AsyncOperation async = null;
        if (_sceneID >= 0)
        {
            //异步加载场景资源
            async = SceneManager.LoadSceneAsync(_sceneID);
        }
        //场景加载完成
        if (async.isDone)
        {
            //释放无用资源
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
        backIndex = _sceneID;
        return async.progress;
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="_loaded"></param>
    /// <param name="_sceneID">场景ID</param>
    /// <returns>是否完成</returns>
    /// <returns></returns>
    public bool LoadScene(int _sceneID = 0, Action _loaded = null)
    {
        AsyncOperation async = null;
        if (_sceneID >= 0)
        {
            //异步加载场景资源
            async = SceneManager.LoadSceneAsync(_sceneID);
        }
        //场景加载完成
        if (async.isDone)
        {
            //释放无用资源
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
        backIndex = _sceneID;
        return async.isDone;
    }

    //加载进度条示例
    IEnumerator LoadScene()
    {
        async = SceneManager.LoadSceneAsync(nextSceneName);
        async.allowSceneActivation = false;
        while (!async.isDone)
        {
            if (async.progress < 0.9f)
                progressValue = async.progress;
            else
                progressValue = 1.0f;

            if (progressValue >= 0.9)
            {            
                if (Input.anyKeyDown)
                {
                    async.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// 返回上一个场景
    /// </summary>
    /// <param name="_progress">加载进度返回值</param>
    public void BackScene(Action<float> _progress = null)
    {
        StartCoroutine(BackSceneIE(_progress));
    }

    private IEnumerator BackSceneIE(Action<float> _progress = null)
    {
        if (_progress == null)
        {
            yield break;
        }
        AsyncOperation async = null;
        if (backIndex >= 0)
        {
            //异步加载场景资源
            async = SceneManager.LoadSceneAsync(backIndex);
        }
        else
        {
            Debug.LogError("错误：场景索引异常");
        }
        while (!async.isDone)
        {
            if (_progress != null)
                _progress(async.progress);
            yield return null;
        }
 
        //场景加载完成
        if (async.isDone)
        {
            //释放无用资源
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
        yield break;
      
    }
}

