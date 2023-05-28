using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace gtm
{
    public class ResourceLoad : BaseResourceLoad
    {
        /// <summary>
        /// 是否使用打包资源
        /// </summary>
        public static bool useAssetBundle = false;

        /// <summary>
        /// 资源目录加载
        /// </summary>
        EditorResourceLoad m_EditorResLoad = new EditorResourceLoad();

        /// <summary>
        /// asset bundle加载
        /// </summary>
        AssetBundleLoad m_AssetBundleLoad = new AssetBundleLoad();

        /// <summary>
        /// 
        /// </summary>
        public override void DoClose()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public override void DoInit()
        {

            m_AssetBundleLoad.DoInit();

            // 安装编码器
            InstallDecorator(new LuaAssetDecorator());
        }

        /// <summary>
        /// 
        /// </summary>
        public override void DoUpdate()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public override object[] LoadAllResourceSync(string path, string filename, string suffix)
        {
            string realpath = path + filename;

#if UNITY_EDITOR
            if (!useAssetBundle)
            {
                return m_EditorResLoad.LoadAllSync(realpath, suffix);
            }
            else
            {
                return m_AssetBundleLoad.LoadAllSync(realpath);
            }
#else
            return m_AssetBundleLoad.LoadAllSync(realpath);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <param name="suffix"></param>
        /// <param name="restype"></param>
        /// <returns></returns>
        public override object LoadResourceSync(string path, string filename, string suffix, ResourceType restype = ResourceType.Default)
        {
            string realpath = path + filename;

            var type = Type2Native(restype);
            var originType = type;

            BeforeLoad(ref realpath, ref type);

            object obj = null;

#if UNITY_EDITOR
            if (!useAssetBundle)
            {
                obj = m_EditorResLoad.LoadSync(realpath, suffix, type);
            }
            else
            {
                obj = m_AssetBundleLoad.LoadSync(realpath, filename, type);
            }
#else
            obj = m_AssetBundleLoad.LoadSync(realpath, filename, type);
#endif

            return AfterLoad(realpath, originType, obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <param name="suffix"></param>
        /// <param name="callback"></param>
        /// <param name="restype"></param>
        public override void LoadResourceAsync(string path, string filename, string suffix, ResourceLoadComplete callback, ResourceType restype = ResourceType.Default)
        {
            string realpath = path + filename;

            var type = Type2Native(restype);
            var originType = type;

            BeforeLoad(ref realpath, ref type);

#if UNITY_EDITOR
            if (!useAssetBundle)
            {
                m_EditorResLoad.LoadAsync(realpath, suffix, type, (obj) => {
                    ResourceAsyncCallback(realpath, originType, obj, callback);
                });
            }
            else
            {
                m_AssetBundleLoad.LoadAsync(realpath, filename, type, (obj) => {
                    ResourceAsyncCallback(realpath, originType, obj, callback);
                });
            }
#else
            
            m_AssetBundleLoad.LoadAsync(realpath, filename, type, (obj) => {
                ResourceAsyncCallback(realpath, originType, obj, callback);
            });
#endif
        }

        /// <summary>
        /// 场景异步加载
        /// </summary>
        /// <param name="scenename"></param>
        /// <param name="progress"></param>
        /// <param name="complete"></param>
        public override void LoadSceneAsync(string path, string filename, string suffix, ResourceLoadProgress progress = null, ResourceLoadComplete complete = null)
        {
            string realpath = path + filename;

#if UNITY_EDITOR
            if (!useAssetBundle)
            {
                m_EditorResLoad.LoadSceneAsync(realpath, suffix, progress, complete);
            }
            else
            {
                m_AssetBundleLoad.LoadSceneAsync(realpath, filename, suffix, progress, complete);
            }
#else

            m_AssetBundleLoad.LoadSceneAsync(realpath, filename, suffix, progress, complete);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="callback"></param>
        void ResourceAsyncCallback(string assetName, Type type, object obj, ResourceLoadComplete callback)
        {
            AfterLoad(assetName, type, obj);

            if (callback != null)
                callback(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        void BeforeLoad(ref string assetName, ref Type type)
        {
            for (var i = m_Decorators.Count - 1; i >= 0; --i)
            {
                m_Decorators[i].BeforeLoad(ref assetName, ref type);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="type"></param>
        /// <param name="asset"></param>
        /// <returns></returns>
        object AfterLoad(string assetName, Type type, object asset)
        {
            for (var i = 0; i < this.m_Decorators.Count; ++i)
            {
                m_Decorators[i].AfterLoad(assetName, type, ref asset);
            }

            return asset;
        }
    }
}

