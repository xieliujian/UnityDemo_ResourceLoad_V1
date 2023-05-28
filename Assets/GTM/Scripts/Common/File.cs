using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace gtm
{
    public class File
    {
        /// <summary>
        /// .
        /// </summary>
        /// <returns></returns>
        public static string GetFilePath()
        {
#if UNITY_EDITOR
            return Application.streamingAssetsPath + "/";
#else
            // windows程序
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {            
                return Application.dataPath + "/StreamingAssets/";
            }

            // mobile程序
            return Application.streamingAssetsPath + "/";
#endif
        }

        /// <summary>
        /// .
        /// </summary>
        /// <param name="respath"></param>
        /// <returns></returns>
        public static string GetBundleFullPath(string respath)
        {
            string path = GetFilePath() + AppConst.APP_NAME + "/" + respath;
            return path;
        }
    }
}
