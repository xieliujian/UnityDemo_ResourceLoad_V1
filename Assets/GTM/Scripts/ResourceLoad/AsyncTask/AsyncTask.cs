using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gtm
{
    public abstract class AsyncTask
    {
        /// <summary>
        /// 
        /// </summary>
        public enum ETaskState
        {
            NotStart,
            Running,
            Completed,
            End,
        }

        /// <summary>
        /// 
        /// </summary>
        protected ETaskState m_State = ETaskState.NotStart;

        /// <summary>
        /// 
        /// </summary>
        protected object m_Asset = null;

        /// <summary>
        /// 
        /// </summary>
        protected ResourceLoadProgress m_ProgressEvent;

        /// <summary>
        /// 
        /// </summary>
        protected ResourceLoadComplete m_CompleteEvent;

        /// <summary>
        /// 进度
        /// </summary>
        public abstract float progress { get; }

        /// <summary>
        /// 进度条事件
        /// </summary>
        public ResourceLoadProgress progressEvent
        {
            get { return m_ProgressEvent; }
            set { m_ProgressEvent = value; }
        }

        /// <summary>
        /// 完成事件
        /// </summary>
        public ResourceLoadComplete completeEvent
        {
            get { return m_CompleteEvent; }
            set { m_CompleteEvent = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool isEnd
        {
            get { return m_State == ETaskState.End; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            if (m_State == ETaskState.NotStart)
            {
                m_State = ETaskState.Running;
                OnStart();
            }

            m_State = OnUpdate();

            if (m_State == ETaskState.Completed)
            {
                m_State = ETaskState.End;
                OnEnd();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void OnStart();

        /// <summary>
        /// 
        /// </summary>
        protected abstract ETaskState OnUpdate();

        /// <summary>
        /// 
        /// </summary>
        protected abstract void OnEnd();
    }
}
