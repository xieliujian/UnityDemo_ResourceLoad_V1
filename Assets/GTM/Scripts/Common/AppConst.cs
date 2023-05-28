using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace gtm
{
    public class AppConst
    {
        /// <summary>
        /// 游戏名字
        /// </summary>
        public const string APP_NAME = "unityframework";

        /// <summary>
        /// 打包目录
        /// </summary>
        public const string ASSET_DIR = "assetBundle";

        /// <summary>
        /// assetbundle后缀
        /// </summary>
        public const string BUNDLE_SUFFIX = ".unity3d";
    }

    public class AppPlatform
    {
        public static string dataPath
        {
            get
            {
                //string assetdir = AppConst.ASSET_DIR;

#if UNITY_EDITOR
                return Application.dataPath + "/../";
#else

                // windows程序
                if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    return Application.dataPath + "/";
                }

                // mobile程序
                return Application.persistentDataPath + "/";
#endif
            }
        }

        public static string streamingAssetsPath
        {
            get
            {
                string appname = AppConst.APP_NAME.ToLower();
                string path = Application.streamingAssetsPath + "/" + appname + "/";
                return path;
            }
        }

#if UNITY_EDITOR

        public static BuildTarget GetCurBuildTarget()
        {
            var target = BuildTarget.NoTarget;

#if UNITY_ANDROID
            target = BuildTarget.Android;
#endif

#if UNITY_IOS
            target = BuildTarget.iOS;
#endif

#if UNITY_STANDALONE_WIN
            target = BuildTarget.StandaloneWindows;
#endif

            return target;
        }

        public static BuildTargetGroup GetCurBuildTargetGroup()
        {
            var targetgroup = BuildTargetGroup.Standalone;

#if UNITY_ANDROID
            targetgroup = BuildTargetGroup.Android;
#endif

#if UNITY_IOS
            targetgroup = BuildTargetGroup.iOS;
#endif

#if UNITY_STANDALONE_WIN
            targetgroup = BuildTargetGroup.Standalone;
#endif

            return targetgroup;
        }

        public static string GetPackageResPath(BuildTarget target)
        {
            string platformpath = "";
            if (target == BuildTarget.StandaloneWindows)
            {
                platformpath = RuntimePlatform.WindowsPlayer.ToString().ToLower();
            }
            else if (target == BuildTarget.Android)
            {
                platformpath = RuntimePlatform.Android.ToString().ToLower();
            }
            else if (target == BuildTarget.iOS)
            {
                platformpath = RuntimePlatform.IPhonePlayer.ToString().ToLower();
            }

            string assetdir = AppConst.ASSET_DIR;
            string appname = AppConst.APP_NAME.ToLower();
            string respath = Application.dataPath + "/../" + assetdir + "/" + platformpath + "/" + appname + "/";
            return respath;
        }

#endif
    }
}
