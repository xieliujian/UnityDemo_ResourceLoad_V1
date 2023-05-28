using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace gtm
{
    public class AsyncAssetRequest : AsyncTask
    {
        /// <summary>
        /// .
        /// </summary>
        Bundle m_Bundle;

        /// <summary>
        /// .
        /// </summary>
        string m_ResName;

        /// <summary>
        /// .
        /// </summary>
        AssetBundleRequest m_Request;

        /// <summary>
        /// .
        /// </summary>
        Type m_Type;

        public override float progress
        {
            get
            {
                float val = 0.0f;

                if (m_Request != null)
                {
                    val = m_Request.progress;
                }

                return val;
            }
        }

        /// <summary>
        /// .
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="resname"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public AsyncAssetRequest(Bundle bundle, string resname, Type type, ResourceLoadComplete callback)
        {
            m_Bundle = bundle;
            m_ResName = resname;
            m_CompleteEvent = callback;
            m_Type = type;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnEnd()
        {
            object val = null;
            if (m_Request != null)
            {
                val = m_Request.asset;
            }

            if (m_CompleteEvent != null)
            {
                m_CompleteEvent.Invoke(val);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnStart()
        {
            m_Request = m_Bundle.LoadAssetAsyncFromBundle(m_ResName, m_Type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override ETaskState OnUpdate()
        {
            if (m_ProgressEvent != null)
            {
                m_ProgressEvent.Invoke(progress);
            }

            bool isdone = false;
            if (m_Request != null && m_Request.isDone)
            {
                isdone = true;
            }

            return isdone ? ETaskState.Completed : ETaskState.Running;
        }
    }
}

