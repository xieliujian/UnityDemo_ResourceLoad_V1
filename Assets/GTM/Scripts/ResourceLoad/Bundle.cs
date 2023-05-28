using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace gtm
{
    public class Bundle
    {
        enum State
        {
            Unloaded,
            Loading,
            Loaded
        }

        /// <summary>
        /// 状态
        /// </summary>
        State m_State = State.Unloaded;

        /// <summary>
        /// .
        /// </summary>
        string m_Path;

        /// <summary>
        /// .
        /// </summary>
        AssetBundleLoad m_Load;

        /// <summary>
        /// .
        /// </summary>
        List<Bundle> m_DependList = new List<Bundle>(ConstDefine.LIST_CONST_8);

        /// <summary>
        /// .
        /// </summary>
        AssetBundle m_AssetBundle;

        /// <summary>
        /// 即将加载列表
        /// </summary>
        List<AsyncTask> m_PendingLoadList = new List<AsyncTask>(ConstDefine.LIST_CONST_8);

        /// <summary>
        /// .
        /// </summary>
        public AssetBundleLoad load
        {
            get { return m_Load; }
            set { m_Load = value; }
        }

        /// <summary>
        /// assetBundle
        /// </summary>
        public AssetBundle assetBundle
        {
            get { return m_AssetBundle; }
        }

        /// <summary>
        /// .
        /// </summary>
        public bool isLoaded
        {
            get { return m_State == State.Loaded; }
        }

        /// <summary>
        /// .
        /// </summary>
        public bool dependIsLoaded
        {
            get
            {
                foreach (var depend in m_DependList)
                {
                    if (depend == null)
                        continue;

                    if (!depend.isLoaded)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// .
        /// </summary>
        /// <param name="path"></param>
        public Bundle(string path)
        {
            m_Path = path;
        }

        /// <summary>
        /// .
        /// </summary>
        /// <param name="manifest"></param>
        public void InitDependencies(AssetBundleManifest manifest)
        {
            if (m_Load == null)
                return;

            if (manifest == null)
                return;

            var dependarray = manifest.GetAllDependencies(m_Path);
            if (dependarray == null)
                return;

            foreach (var dependname in dependarray)
            {
                if (dependname == null)
                    continue;

                var bundle = m_Load.GetBundle(dependname);
                if (bundle == null)
                    continue;

                m_DependList.Add(bundle);
            }
        }

        /// <summary>
        /// .
        /// </summary>
        public void LoadBundleAsync()
        {
            if (m_State == State.Unloaded)
            {
                m_State = State.Loading;

                foreach (var depend in m_DependList)
                {
                    if (depend == null)
                        continue;

                    depend.LoadBundleAsync();
                }

                AsyncTask asynctask = new AsyncBundleRequest(this, m_Path, OnAsyncLoaded);
                if (asynctask != null)
                {
                    BaseAsyncTaskManager.instance.AddTask(asynctask);
                }
            }
        }

        /// <summary>
        /// .
        /// </summary>
        public void LoadBundleSync()
        {
            if (m_State == State.Unloaded)
            {
                m_State = State.Loaded;

                foreach (var depend in m_DependList)
                {
                    if (depend == null)
                        continue;

                    depend.LoadBundleSync();
                }

                var fullpath = File.GetBundleFullPath(m_Path);
                m_AssetBundle = AssetBundle.LoadFromFile(fullpath);
            }
        }

        /// <summary>
        /// .
        /// </summary>
        /// <returns></returns>
        public object[] LoadAllSync()
        {
            LoadBundleSync();

            if (m_AssetBundle == null)
                return null;

            return m_AssetBundle.LoadAllAssets();
        }

        /// <summary>
        /// .
        /// </summary>
        /// <param name="resname"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object LoadSync(string resname, Type type)
        {
            LoadBundleSync();

            if (m_AssetBundle == null)
                return null;

            return m_AssetBundle.LoadAsset(resname, type);
        }

        /// <summary>
        /// .
        /// </summary>
        /// <param name="resname"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
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

        /// <summary>
        /// .
        /// </summary>
        /// <param name="resname"></param>
        /// <param name="progress"></param>
        /// <param name="complete"></param>
        public void LoadSceneAsync(string resname, ResourceLoadProgress progress = null, ResourceLoadComplete complete = null)
        {
            LoadBundleAsync();

            AsyncTask asynctask = new AsyncSceneRequest(this, resname, progress, complete);
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

        /// <summary>
        /// .
        /// </summary>
        /// <param name="resname"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public AssetBundleRequest LoadAssetAsyncFromBundle(string resname, Type type)
        {
            if (m_AssetBundle == null)
                return null;

            return m_AssetBundle.LoadAssetAsync(resname, type);
        }

        /// <summary>
        /// .
        /// </summary>
        /// <param name="obj"></param>
        void OnAsyncLoaded(object obj)
        {
            if (obj == null)
                return;

            var bundle = (AssetBundle)obj;
            if (bundle == null)
                return;

            m_State = State.Loaded;
            m_AssetBundle = bundle;

            foreach(var load in m_PendingLoadList)
            {
                if (load == null)
                    continue;

                BaseAsyncTaskManager.instance.AddTask(load);
            }

            m_PendingLoadList.Clear();
        }
    }
}





