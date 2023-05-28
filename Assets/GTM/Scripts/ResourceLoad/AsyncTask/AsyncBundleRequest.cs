using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace gtm
{
    public class AsyncBundleRequest : AsyncTask
    {
        /// <summary>
        /// 
        /// </summary>
        Bundle m_Bundle;

        /// <summary>
        /// 
        /// </summary>
        string m_Path;

        /// <summary>
        /// 
        /// </summary>
        AssetBundleCreateRequest m_CreateRequest;

        /// <summary>
        /// 
        /// </summary>
        public override float progress
        {
            get
            {
                float val = m_CreateRequest != null ? m_CreateRequest.progress : 0;
                return val;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public AssetBundle assetBundle
        {
            get
            {
                var val = null != m_CreateRequest ? m_CreateRequest.assetBundle : null;

                return val;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        public AsyncBundleRequest(Bundle bundle, string path, ResourceLoadComplete callback)
        {
            m_Bundle = bundle;
            m_Path = path;
            m_CompleteEvent = callback;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnEnd()
        {
            if (m_CompleteEvent != null)
            {
                m_CompleteEvent.Invoke(assetBundle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnStart()
        {
            var fullpath = File.GetBundleFullPath(m_Path);
            m_CreateRequest = AssetBundle.LoadFromFileAsync(fullpath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override ETaskState OnUpdate()
        {
            bool isdone = false;
            if (m_CreateRequest != null && m_Bundle != null)
            {
                isdone = m_CreateRequest.isDone && m_Bundle.dependIsLoaded;
            }

            return isdone ? ETaskState.Completed : ETaskState.Running;
        }

    }
}

