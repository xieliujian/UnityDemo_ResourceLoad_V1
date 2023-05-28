using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;

namespace gtm
{
    public class AssetBundleLoad
    {
        /// <summary>
        /// bundle dict
        /// </summary>
        Dictionary<string, Bundle> m_BundleDict = new Dictionary<string, Bundle>(ConstDefine.LIST_CONST_1024);

        /// <summary>
        /// manifest
        /// </summary>
        AssetBundleManifest m_Manifest = null;

        /// <summary>
        /// .
        /// </summary>
        public void DoInit()
        {
            InitAllBundle();
        }
        
        /// <summary>
        /// .
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Bundle GetBundle(string name)
        {
            Bundle bundle = null;
            m_BundleDict.TryGetValue(name, out bundle);
            return bundle;
        }

        /// <summary>
        /// .
        /// </summary>
        /// <param name="respath"></param>
        /// <returns></returns>
        public object[] LoadAllSync(string respath)
        {
            var fullpath = respath + AppConst.BUNDLE_SUFFIX;
            var bundle = GetBundle(fullpath);
            if (bundle == null)
                return null;

            return bundle.LoadAllSync();
        }

        /// <summary>
        /// .
        /// </summary>
        /// <param name="respath"></param>
        /// <param name="filename"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object LoadSync(string respath, string filename, Type type)
        {
            var fullpath = respath + AppConst.BUNDLE_SUFFIX;
            var bundle = GetBundle(fullpath);
            if (bundle == null)
                return null;

            return bundle.LoadSync(filename, type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realpath"></param>
        /// <param name="filename"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public void LoadAsync(string realpath, string filename, Type type, ResourceLoadComplete callback)
        {
            var fullpath = realpath + AppConst.BUNDLE_SUFFIX;
            var bundle = GetBundle(fullpath);
            if (bundle == null)
                return;

            bundle.LoadAsync(filename, type, callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realpath"></param>
        /// <param name="filename"></param>
        /// <param name="suffix"></param>
        /// <param name="progress"></param>
        /// <param name="complete"></param>
        public void LoadSceneAsync(string realpath, string filename, string suffix, ResourceLoadProgress progress = null, ResourceLoadComplete complete = null)
        {
            var fullpath = realpath + AppConst.BUNDLE_SUFFIX;
            var bundle = GetBundle(fullpath);
            if (bundle == null)
                return;

            bundle.LoadSceneAsync(filename, progress, complete);
        }

        /// <summary>
        /// 
        /// </summary>
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
    }
}

