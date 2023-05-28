
# Unity资源加载第一版

## Demo示例

![github](https://github.com/xieliujian/UnityDemo_ResourceLoad_V1/blob/main/Video/1.png?raw=true)

## 代码信息

### 资源加载

通过AssetBundleManifest建立Bundle依赖信息

```cs

void InitAllBundle()
{
    string manifest_path = File.GetFilePath() + AppConst.APP_NAME + "/" + AppConst.APP_NAME;
    var manifestBundle = AssetBundle.LoadFromFile(manifest_path);

    if (manifestBundle != null)
    {
        m_Manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        var allbundlearray = m_Manifest.GetAllAssetBundles();
        if (allbundlearray != null)
        {
            foreach (var bundlename in allbundlearray)
            {
                if (bundlename == null)
                    continue;

                var bundle = new Bundle(bundlename);
                if (bundle != null)
                {
                    bundle.load = this;
                    bundle.InitDependencies(m_Manifest);

                    if (!m_BundleDict.ContainsKey(bundlename))
                    {
                        m_BundleDict.Add(bundlename, bundle);
                    }
                }
            }
        }

        manifestBundle.Unload(true);
    }
}

```

分为`离散资源`的同步加载和异步加载，和`Bundle资源`的同步和异步加载

`离散资源`通过`AssetDataBase`模拟Bundle加载

`Bundle资源`的同步加载

```cs

public object LoadSync(string respath, string filename, Type type)
{
    var fullpath = respath + AppConst.BUNDLE_SUFFIX;
    var bundle = GetBundle(fullpath);
    if (bundle == null)
        return null;

    return bundle.LoadSync(filename, type);
}

public object LoadSync(string resname, Type type)
{
    LoadBundleSync();

    if (m_AssetBundle == null)
        return null;

    return m_AssetBundle.LoadAsset(resname, type);
}

```

`Bundle资源`的异步加载

```cs

public void LoadAsync(string realpath, string filename, Type type, ResourceLoadComplete callback)
{
    var fullpath = realpath + AppConst.BUNDLE_SUFFIX;
    var bundle = GetBundle(fullpath);
    if (bundle == null)
        return;

    bundle.LoadAsync(filename, type, callback);
}

public void LoadAsync(string resname, Type type, ResourceLoadComplete callback)
{
    LoadBundleAsync();

    AsyncTask asynctask = new AsyncAssetRequest(this, resname, type, callback);
    if (asynctask != null)
    {
        if (isLoaded)
        {
            BaseAsyncTaskManager.instance.AddTask(asynctask);
        }
        else
        {
            m_PendingLoadList.Add(asynctask);
        }
    }
}

```

### 打包逻辑

通过收集`AssetBundleBuild`的方式进行打包

```cs

static void AddAssetBundleBuild(string assetBundleName, string[] assetNames, string assetBundleVariant = "unity3d")
{
    AssetBundleBuild build = new AssetBundleBuild();
    build.assetBundleName = assetBundleName;
    build.assetBundleVariant = assetBundleVariant;
    build.assetNames = assetNames;
    m_BundleBuildList.Add(build);
}

BuildPipeline.BuildAssetBundles(resPath, m_BundleBuildList.ToArray(), BuildAssetBundleOptions.None, target);

```