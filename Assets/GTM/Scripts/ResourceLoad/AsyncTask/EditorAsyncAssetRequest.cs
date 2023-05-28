using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace gtm
{
    public class EditorAsyncAssetRequest : AsyncTask
    {
        /// <summary>
        /// 
        /// </summary>
        public override float progress
        {
            get { return 1.0f; }
        }

        /// <summary>
        /// 
        /// </summary>
        public EditorAsyncAssetRequest(string respath, Type type)
        {
#if UNITY_EDITOR
            m_Asset = UnityEditor.AssetDatabase.LoadAssetAtPath(respath, type);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnEnd()
        {
            m_CompleteEvent.Invoke(m_Asset);
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
            return ETaskState.Completed;
        }
    }
}

