using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace gtm
{
    /// <summary>
    /// .
    /// </summary>
    /// <param name="progress"></param>
    public delegate void ResourceLoadProgress(float progress);

    /// <summary>
    /// .
    /// </summary>
    /// <param name="asset"></param>
    public delegate void ResourceLoadComplete(object asset);

    /// <summary>
    /// .
    /// </summary>
    public enum ResourceType : byte
    {
        Default,
        String,
        Bytes,
        GameObject,
        Scene,
        Texture,
        Sprite,
        Material,
        Shader,
        AnimationClip,
        AudioClip,
        ScriptableObject,
    }

    public abstract class BaseResourceLoad : Manager
    {
        /// <summary>
        /// 
        /// </summary>
        protected static BaseResourceLoad s_Instance = null;

        /// <summary>
        /// 编码
        /// </summary>
        protected readonly List<AssetDecorator> m_Decorators = new List<AssetDecorator>(ConstDefine.LIST_CONST_8);

        /// <summary>
        /// .
        /// </summary>
        public static BaseResourceLoad instance
        {
            get { return s_Instance; }
        }

        /// <summary>
        /// .
        /// </summary>
        public BaseResourceLoad()
        {
            s_Instance = this;
            m_Decorators.Clear();
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public abstract object LoadResourceSync(string path, string filename, string suffix, ResourceType restype = ResourceType.Default);

        /// <summary>
        /// 同步加载所有的资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public abstract object[] LoadAllResourceSync(string path, string filename, string suffix);

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public abstract void LoadResourceAsync(string path, string filename, string suffix, ResourceLoadComplete callback, ResourceType restype = ResourceType.Default);

        /// <summary>
        /// 场景异步加载
        /// </summary>
        /// <param name="scenename"></param>
        /// <param name="progress"></param>
        /// <param name="complete"></param>
        public abstract void LoadSceneAsync(string path, string filename, string suffix, ResourceLoadProgress progress = null, ResourceLoadComplete complete = null);

        /// <summary>
        /// 安装编码器
        /// </summary>
        /// <param name="decorator"></param>
        public void InstallDecorator(AssetDecorator decorator)
        {
            if (m_Decorators.Contains(decorator))
                return;

            m_Decorators.Add(decorator);
        }

        /// <summary>
        /// .
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected Type Type2Native(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.String:
                    return typeof(string);
                case ResourceType.Bytes:
                    return typeof(byte[]);
                default:
                    return typeof(object);

                // 参考
                //case ResourceType.GameObject:
                //    return typeof(GameObject);
                //case ResourceType.Scene:
                //    return typeof(Scene);
                //case ResourceType.Texture:
                //    return typeof(Texture2D);
                //case ResourceType.Sprite:
                //    return typeof(Sprite);
                //case ResourceType.Material:
                //    return typeof(Material);
                //case ResourceType.Shader:
                //    return typeof(Shader);
                //case ResourceType.AnimationClip:
                //    return typeof(AnimationClip);
                //case ResourceType.AudioClip:
                //    return typeof(AudioClip);
                //case ResourceType.ScriptableObject:
                //    return typeof(ScriptableObject);
            }
        }
    }
}
