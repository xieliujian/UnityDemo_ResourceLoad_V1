using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace gtm
{
    public class AsyncSceneRequest : AsyncTask
    {
        /// <summary>
        /// 
        /// </summary>
        Bundle m_Bundle;

        /// <summary>
        /// 
        /// </summary>
        string m_ResName;

        /// <summary>
        /// 
        /// </summary>
        AsyncOperation m_Request;

        /// <summary>
        /// 
        /// </summary>
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
        /// 
        /// </summary>
        public AsyncSceneRequest(Bundle bundle, string resname, ResourceLoadProgress progress = null, ResourceLoadComplete complete = null)
        {
            m_Bundle = bundle;
            m_ResName = resname;
            m_ProgressEvent = progress;
            m_CompleteEvent = complete;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnEnd()
        {
            object val = SceneManager.GetSceneByName(m_ResName);

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
            m_Request = SceneManager.LoadSceneAsync(m_ResName, LoadSceneMode.Single);
        }

        /// <summary>
        /// 
        /// </summary>
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

