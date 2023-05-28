
#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System;

namespace gtm
{
    public class EditorAsyncSceneRequest : AsyncTask
    {
        /// <summary>
        /// 
        /// </summary>
        AsyncOperation m_Operation;

        /// <summary>
        /// 
        /// </summary>
        string m_ScenePath;

        /// <summary>
        /// 
        /// </summary>
        public override float progress
        {
            get
            {
                if (m_Operation == null || m_Operation.isDone)
                {
                    return 1;
                }

                return m_Operation.progress;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public EditorAsyncSceneRequest(string scenepath)
        {
            m_ScenePath = scenepath;

            LoadSceneParameters param = new LoadSceneParameters(LoadSceneMode.Single);
            m_Operation = EditorSceneManager.LoadSceneAsyncInPlayMode(scenepath, param);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnEnd()
        {
            var asset = EditorSceneManager.GetSceneByPath(m_ScenePath);

            if (m_CompleteEvent != null)
            {
                m_CompleteEvent.Invoke(asset);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnStart()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        protected override ETaskState OnUpdate()
        {
            if (m_ProgressEvent != null)
                m_ProgressEvent.Invoke(progress);

            var iscomplete = m_Operation != null && m_Operation.isDone;

            return iscomplete ? ETaskState.Running : ETaskState.Completed;
        }
    }
}

#endif