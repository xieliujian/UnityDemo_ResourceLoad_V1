using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace gtm
{
    public class EditorResourceLoad
    {
        /// <summary>
        /// 编辑器路径的前缀
        /// </summary>
        public const string EDITOR_PATH_PREFIX = "Assets/Package/";

        /// <summary>
        /// .
        /// </summary>
        /// <param name="realpath"></param>
        /// <param name="suffix"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object LoadSync(string realpath, string suffix, Type type)
        {
#if UNITY_EDITOR
            string loadpath = EDITOR_PATH_PREFIX + realpath;
            loadpath += suffix;

            var res = UnityEditor.AssetDatabase.LoadAssetAtPath(loadpath, type);
            return res;
#else
            return null;
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realpath"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public object[] LoadAllSync(string realpath, string suffix)
        {
#if UNITY_EDITOR
            string loadpath = EDITOR_PATH_PREFIX + realpath;
            loadpath += suffix;

            var res = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(loadpath);
            return res;
#else
            return null;
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realpath"></param>
        /// <param name="suffix"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public void LoadAsync(string realpath, string suffix, Type type, ResourceLoadComplete callback)
        {
            string loadpath = EDITOR_PATH_PREFIX + realpath;
            loadpath += suffix;

            AsyncTask asynctask = new EditorAsyncAssetRequest(loadpath, type);
            if (asynctask != null)
            {
                if (callback != null)
                    asynctask.completeEvent = callback;

                BaseAsyncTaskManager.instance.AddTask(asynctask);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="realpath"></param>
        /// <param name="suffix"></param>
        /// <param name="progress"></param>
        /// <param name="complete"></param>
        public void LoadSceneAsync(string realpath, string suffix, ResourceLoadProgress progress = null, ResourceLoadComplete complete = null)
        {
#if UNITY_EDITOR

            string loadpath = EDITOR_PATH_PREFIX + realpath;
            loadpath += suffix;

            AsyncTask asynctask = new EditorAsyncSceneRequest(loadpath);
            if (asynctask != null)
            {
                asynctask.progressEvent = progress;
                asynctask.completeEvent = complete;

                BaseAsyncTaskManager.instance.AddTask(asynctask);
            }

#endif
        }
    }
}
