// *************************************************
// Copyright (C): 猫眼视觉
// 文件名:        LoadFromAssetBundles.cs
// 作者:          韦伟
// 创建时间:      2019-11-05 02:21:21
// UnityVersion:  2018.2.14f1
// *************************************************
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows;

public class LoadFromAssetBundles : Singleton<LoadFromAssetBundles>
{
    #region
    ///// <summary>
    ///// 从本地AB包加载文件
    ///// </summary>
    ///// <returns></returns>
    //private IEnumerator LoadFromFileIE_demo()
    //{
    //    //这里需要注意的是，如果你想加载的资源有依赖的资源 如贴图等资源，则必须先加载依赖资源后加载你所想加载的资源。
    //    //即A依赖B 则必须先加载B到内存中，然后再加载A 并实例化---这里注意依赖项一般可以不用实例化只需要在内存中即可。
    //    AssetBundle abShare = AssetBundle.LoadFromFile("AssetBundles/materials/mapmaterials");

    //    AssetBundle ab = AssetBundle.LoadFromFile("AssetBundles/scene/map_001");

    //    //这里需要注意的是 单独加载和加载所有压缩包内的对象处理方式

    //    //加载单独的
    //    GameObject panelPrefab = ab.LoadAsset<GameObject>("Plane"); //注意名字不能写错了

    //    Instantiate(panelPrefab);//实例化资源

    //    ////加载所有
    //    //Object[] panelPrefab = ab.LoadAllAssets();

    //    //foreach (var obj in panelPrefab)
    //    //{
    //    //    Instantiate(obj);//实例化资源
    //    //}

    //    yield return null;
    //}
    ///// <summary>
    ///// 从内存加载AB资源
    ///// </summary>
    ///// <returns></returns>
    //private IEnumerator LoadFromMemoryIE()
    //{
    //    //这里需要注意的是，如果你想加载的资源有依赖的资源 如贴图等资源，则必须先加载依赖资源后加载你所想加载的资源。
    //    //即A依赖B 则必须先加载B到内存中，然后再加载A 并实例化---这里注意依赖项一般可以不用实例化只需要在内存中即可。
    //    AssetBundle abShare = AssetBundle.LoadFromFile("AssetBundles/materials/mapmaterials");

    //    string path = "AssetBundles/scene/map_01";

    //    //LoadFromMemoryAsync 和 LoadFromMemory --内存读取
    //    AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(path)); //异步的方式
    //    yield return request; //异步的方式 必须等待完成后才继续执行
    //    AssetBundle ab = request.assetBundle;

    //    //AssetBundle ab = AssetBundle.LoadFromMemory(File.ReadAllBytes(path));//同步的方式

    //    //使用资源
    //    GameObject gameObj = ab.LoadAsset<GameObject>("map_001");
    //    Instantiate(gameObj);

    //    yield return null;
    //}


    //private IEnumerator LoadFromWebIE()
    //{
    //    //第四种远程加载方式---（专门用于替换WWW的远程加载方式）
    //    //string uri = @"E:/03_TestDemo/01_UnityDemo/ShowMap_ABPackageTest_001/AssetBundles/scene/map_001";
    //    string uri = @"http://localhost/AssetBundles/scene/map_001";
    //    UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri);
    //    yield return request.SendWebRequest();  //注意是异步方式所以必须等待他完成后再执行

    //    //两种获取ab对象的方式
    //    AssetBundle ab = DownloadHandlerAssetBundle.GetContent(request); //第一种方法
    //    //AssetBundle ab = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle; //第二种方法

    //    //使用资源
    //    GameObject gameObj = ab.LoadAsset<GameObject>("map_001");
    //    Instantiate(gameObj);

    //    yield return null; //注意上述的远程加载方式是没有加载其依赖的资源
    //}
    #endregion

    /// <summary>
    /// 加载路径
    /// </summary>
    StringBuilder path;
    string tempPath;
    readonly string manifestStr = "AssetBundleManifest";
    private void Awake()
    {
        tempPath = Application.dataPath + "/Resources/AssetBundles/";
        path = new StringBuilder(tempPath);
        
    }
    /// <summary>
    /// 从服务器加载AB资源
    /// </summary>
    public void LoadFromWeb(string _path)
    {
        if (!string.IsNullOrEmpty(_path))
        {
            path.Append(_path);
        }    
        StartCoroutine(LoadFromWebOrDependentIE(path.ToString()));
    }

    /// <summary>
    /// 从本地加载AB资源
    /// </summary>
    /// <param name="_path">资源文件夹名</param>
    public void LoadFromFile(string _path)
    {
        if (!string.IsNullOrEmpty(_path))
        {
            path.Append(_path);
        }
        StartCoroutine(LoadFromFileIE(path.ToString()));
    }

    //服务端加载AB资源包 和依赖包
    private IEnumerator LoadFromWebOrDependentIE(string _path)
    {
        //从一个服务器下载一个AB包的管理文件AssetBundles 和 AssetBundles.manifest 
        string uri = @"http://D:/HostDemo";
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri);
        yield return request.SendWebRequest();
      
        AssetBundle ab = DownloadHandlerAssetBundle.GetContent(request); //加载资源
        AssetBundleManifest manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundles");

        string[] str = manifest.GetAllAssetBundles();

        foreach (string s in str)
        {
            UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(@"http://localhost/AssetBundles/" + s);
            yield return uwr.SendWebRequest();
            AssetBundle TmpAB = DownloadHandlerAssetBundle.GetContent(uwr);//第二种方法
            Object[] obj = TmpAB.LoadAllAssets();

            foreach (Object o in obj)
            {
                if (o is GameObject)
                {
                    Instantiate(o);
                }

                print(obj.Length);
            }
        }
        yield return null;
    }


    //本地加载AB资源包 和依赖包 
    private IEnumerator LoadFromFileIE(string _path)
    {
        //加载得到Manifest文件
        AssetBundle ab = AssetBundle.LoadFromFile(path.ToString());
        AssetBundleManifest manifest = ab.LoadAsset<AssetBundleManifest>(manifestStr);

        //从Manifest文件中得到所有的AB包的路径（包括依赖项）
        string[] str = manifest.GetAllAssetBundles(); 
       
        foreach (string s in str)
        {           
            AssetBundle assetBundle = AssetBundle.LoadFromFile(path.Clear().Append(tempPath).Append(s).ToString());
            Object[] o = assetBundle.LoadAllAssets();
            foreach (Object temp in o)
            {
                if (temp is GameObject)
                {
                    Instantiate(temp);
                }
            }
        }
        yield return null;
    }
}
