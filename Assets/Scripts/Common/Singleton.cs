// *************************************************
// Copyright (C): 猫眼视觉
// 文件名:        Singleton.cs
// 作者:          韦伟
// 创建时间:      2019-11-04 07:15:04
// UnityVersion:  2018.2.14f1
// *************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 /// <summary>
 /// 单例
 /// </summary>
 /// <typeparam name="T">泛型</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_instance;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }

            lock (_lock)
            {
                if (m_instance == null)
                {
                    m_instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        return m_instance;
                    }

                    if (m_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        m_instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);
                    }
                }

                return m_instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;

    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}
