using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gtm
{ 
    public abstract class BaseAsyncTaskManager : Manager
    {
        /// <summary>
        /// 
        /// </summary>
        protected static BaseAsyncTaskManager s_Instance;

        /// <summary>
        /// 
        /// </summary>
        public static BaseAsyncTaskManager instance
        {
            get { return s_Instance; }
        }

        /// <summary>
        /// 
        /// </summary>
        public BaseAsyncTaskManager()
        {
            s_Instance = this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asynctask"></param>
        public abstract void AddTask(AsyncTask asynctask);
    }
}
