﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Diagnostics;

namespace gtm.Editor
{
    public class Packager
    {
        public enum EAndroidBuildPlatform
        {
            NoPlatform,
        }

        private static string[] TexturePackageDir =
        {
            "ui/icon/",
            "ui/image/",
        };

        private static string[] FontPackageDir =
        {
            "font/",
        };

        private static string[] AudioMusicPackageDir =
        {
            "audio/music/",
        };

        private static string[] AudioSoundPackageDir =
        {
            "audio/sound/",
        };

        private static string[] PrefabPackageDir =
        {
            "prefabs/",
            "ui/uiprefab/"
        };

        private static string[] SceneDir =
        {
            "scene/"
        };

        const string PackagePathPrefix = "/Package/";

        const string LuaCodePath = "/Lua/";

        const string LuaCExeRelativePath = "/tools/xlua_v2.1.14/build/luac/build64/Release/luac.exe";

        const string LuaCExeMacRelativePath = "/tools/xlua_v2.1.14/build/luac/build_unix/luac";

        const string LuaCSrcFileRelativePath = "/Assets/Lua/";

        const string LuaCGenFileRelativePath = "/Temp/";

        const string LuaPath = "config/lua/";

        const string LuaFileName = "luapackage";

        const string Lua_Suffix = ".asset";

        const string Bytes_Suffix = ".bytes";

        [MenuItem("unityframework/Build iPhone Resource", false, 100)]
        public static void BuildiPhoneResource()
        {
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.iOS.appleDeveloperTeamID = "24AZABCKN4";

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            BuildAssetResource(BuildTarget.iOS, AppPlatform.streamingAssetsPath);
        }

        [MenuItem("unityframework/Build Android Resource", false, 101)]
        public static void BuildAndroidResource()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            BuildAssetResource(BuildTarget.Android, AppPlatform.streamingAssetsPath);
        }

        [MenuItem("unityframework/Build Windows Resource", false, 102)]
        public static void BuildWindowsResource()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
            BuildAssetResource(BuildTarget.StandaloneWindows, AppPlatform.streamingAssetsPath);
        }

        [MenuItem("unityframework/PackageAllResource", false, 103)]
        public static void PackageAllResource()
        {
            BuildAssetResource(BuildTarget.StandaloneWindows, AppPlatform.GetPackageResPath(BuildTarget.StandaloneWindows));
            BuildAssetResource(BuildTarget.Android, AppPlatform.GetPackageResPath(BuildTarget.Android));
            BuildAssetResource(BuildTarget.iOS, AppPlatform.GetPackageResPath(BuildTarget.iOS));

            BuildTargetGroup curtargetgroup = AppPlatform.GetCurBuildTargetGroup();
            BuildTarget curtarget = AppPlatform.GetCurBuildTarget();
            EditorUserBuildSettings.SwitchActiveBuildTarget(curtargetgroup, curtarget);
        }

        public static void BuildIosNoPlatform()
        {
            string xcodeprojname = "xcode_iphone";
            List<string> scenes = GetAllScenes();
            BuildPipeline.BuildPlayer(scenes.ToArray(), xcodeprojname, BuildTarget.iOS, BuildOptions.None);
        }

        public static void BuildAndroidNoPlatform()
        {
            string path = GetAndroidBuildPath(EAndroidBuildPlatform.NoPlatform);
            List<string> scenes = GetAllScenes();
            BuildPipeline.BuildPlayer(scenes.ToArray(), path, BuildTarget.Android, BuildOptions.None);
        }

        /// <summary>
        /// 生成绑定素材
        /// </summary>
        private static void BuildAssetResource(BuildTarget target, string resPath)
        {
            // 1.
            //if (Directory.Exists(AppPlatform.dataPath))
            //{
            //    Directory.Delete(AppPlatform.dataPath, true);
            //}

            if (Directory.Exists(resPath))
            {
                Directory.Delete(resPath, true);
            }

            Directory.CreateDirectory(resPath);
            AssetDatabase.Refresh();

            // 2.1
            GenerateLuaScriptableObject();

            // 2.2
            SetAllAssetBundleName();

            // 3.
            BuildPipeline.BuildAssetBundles(resPath, BuildAssetBundleOptions.None, target);

            // 4. 清理assetbundle名字
            AssetDatabase.Refresh();
            ClearAssetBundlesName();

            // 5.
            //BuildFileIndex();
            AssetDatabase.Refresh();
        }

        private static void SetAllAssetBundleName()
        {
            SetAssetBundleName(Application.dataPath + PackagePathPrefix);
        }

        /// <summary>
        /// 清除之前设置过的AssetBundleName，避免产生不必要的资源也打包
        /// 工程中只要设置了AssetBundleName的，都会进行打包
        /// </summary>
        static void ClearAssetBundlesName()
        {
            int length = AssetDatabase.GetAllAssetBundleNames().Length;
            string[] oldAssetBundleNames = new string[length];
            for (int i = 0; i < length; i++)
            {
                oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
            }

            for (int j = 0; j < oldAssetBundleNames.Length; j++)
            {
                AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
            }
        }

        private static void PackageFont(string rootpath)
        {
            foreach (var subdir in FontPackageDir)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(rootpath + subdir);

                foreach (FileInfo pngFile in dirInfo.GetFiles("*.ttf", SearchOption.AllDirectories))
                {
                    string source = pngFile.FullName.Replace("\\", "/");
                    string assetpath = "Assets" + source.Substring(Application.dataPath.Length);

                    AssetImporter assetImporter = AssetImporter.GetAtPath(assetpath);

                    assetpath = assetpath.Replace(EditorResourceLoad.EDITOR_PATH_PREFIX, "");
                    assetpath = assetpath.Replace(".ttf", "");
                    assetImporter.assetBundleName = assetpath;
                    assetImporter.assetBundleVariant = "unity3d";
                }
            }

            AssetDatabase.Refresh();
        }

        private static void PackageAudio(string rootpath)
        {
            foreach (var subdir in AudioMusicPackageDir)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(rootpath + subdir);

                foreach (FileInfo pngFile in dirInfo.GetFiles("*.mp3", SearchOption.AllDirectories))
                {
                    string source = pngFile.FullName.Replace("\\", "/");
                    string assetpath = "Assets" + source.Substring(Application.dataPath.Length);

                    AssetImporter assetImporter = AssetImporter.GetAtPath(assetpath);

                    assetpath = assetpath.Replace(EditorResourceLoad.EDITOR_PATH_PREFIX, "");
                    assetpath = assetpath.Replace(".mp3", "");
                    assetImporter.assetBundleName = assetpath;
                    assetImporter.assetBundleVariant = "unity3d";
                }
            }

            foreach (var subdir in AudioSoundPackageDir)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(rootpath + subdir);

                foreach (FileInfo pngFile in dirInfo.GetFiles("*.ogg", SearchOption.AllDirectories))
                {
                    string source = pngFile.FullName.Replace("\\", "/");
                    string assetpath = "Assets" + source.Substring(Application.dataPath.Length);

                    AssetImporter assetImporter = AssetImporter.GetAtPath(assetpath);

                    assetpath = assetpath.Replace(EditorResourceLoad.EDITOR_PATH_PREFIX, "");
                    assetpath = assetpath.Replace(".ogg", "");
                    assetImporter.assetBundleName = assetpath;
                    assetImporter.assetBundleVariant = "unity3d";
                }
            }

            AssetDatabase.Refresh();
        }

        private static void PackageTexture(string rootpath)
        {
            foreach (var subdir in TexturePackageDir)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(rootpath + subdir);

                foreach (FileInfo pngFile in dirInfo.GetFiles("*.png", SearchOption.AllDirectories))
                {
                    string source = pngFile.FullName.Replace("\\", "/");
                    string assetpath = "Assets" + source.Substring(Application.dataPath.Length);

                    AssetImporter assetImporter = AssetImporter.GetAtPath(assetpath);

                    assetpath = assetpath.Replace(EditorResourceLoad.EDITOR_PATH_PREFIX, "");
                    assetpath = assetpath.Replace(".png", "");
                    assetImporter.assetBundleName = assetpath;
                    assetImporter.assetBundleVariant = "unity3d";
                }
            }

            AssetDatabase.Refresh();
        }

        private static void PackagePrefab(string rootpath)
        {
            foreach (var subdir in PrefabPackageDir)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(rootpath + subdir);

                foreach (FileInfo pngFile in dirInfo.GetFiles("*.prefab", SearchOption.AllDirectories))
                {
                    string source = pngFile.FullName.Replace("\\", "/");
                    string assetpath = "Assets" + source.Substring(Application.dataPath.Length);

                    AssetImporter assetImporter = AssetImporter.GetAtPath(assetpath);

                    assetpath = assetpath.Replace(EditorResourceLoad.EDITOR_PATH_PREFIX, "");
                    assetpath = assetpath.Replace(".prefab", "");
                    assetImporter.assetBundleName = assetpath;
                    assetImporter.assetBundleVariant = "unity3d";
                }
            }

            AssetDatabase.Refresh();
        }

        public static void GenerateLuaScriptableObject()
        {
            var path = "Assets/" + PackagePathPrefix + LuaPath +
                LuaFileName + Lua_Suffix;

            var obj = AssetDatabase.LoadAssetAtPath<LuaScriptableObject>(path);
            if (obj == null)
            {
                obj = ScriptableObject.CreateInstance<LuaScriptableObject>();
                AssetDatabase.CreateAsset(obj, path);
            }
            else
            {
                obj.Clear();
            }

            var luacodepath = Application.dataPath + LuaCodePath;
            luacodepath = luacodepath.Replace("\\", "/");
            var luafilearray = Directory.GetFiles(luacodepath, "*.lua", SearchOption.AllDirectories);
            foreach (var luafile in luafilearray)
            {
                var luapath = luafile.Replace("\\", "/");
                var subluapath = luapath.Substring(luacodepath.Length);

                // 字符串压缩成二进制
                var destFile = Environment.CurrentDirectory + LuaCGenFileRelativePath + subluapath;

                var destfiledir = Path.GetDirectoryName(destFile);
                if (!Directory.Exists(destfiledir))
                {
                    Directory.CreateDirectory(destfiledir);
                }

                var srcFile = Environment.CurrentDirectory + LuaCSrcFileRelativePath + subluapath;

                var exepath = Environment.CurrentDirectory + LuaCExeRelativePath;

#if UNITY_IOS
            exepath = Environment.CurrentDirectory + LuaCExeMacRelativePath;
#endif

                var args = " -o " + destFile + " " + srcFile;

                if (!Util.ExecuteProcess(exepath, args))
                    continue;

                var luacode = System.IO.File.ReadAllBytes(destFile);
                System.IO.File.Delete(destFile);

                //var luacode1 = File.ReadAllBytes(luapath);

                obj.AddEntry(subluapath, luacode);
            }

            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
        }

        private static void PackageConfig(string rootpath)
        {
            PackageConfig_Lua(rootpath);
        }

        private static void PackageConfig_Lua(string rootpath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(rootpath + LuaPath);

            foreach (FileInfo scenefile in dirInfo.GetFiles("*.asset", SearchOption.AllDirectories))
            {
                string source = scenefile.FullName.Replace("\\", "/");
                string assetpath = "Assets" + source.Substring(Application.dataPath.Length);

                AssetImporter assetImporter = AssetImporter.GetAtPath(assetpath);

                assetpath = assetpath.Replace(EditorResourceLoad.EDITOR_PATH_PREFIX, "");
                assetpath = assetpath.Replace(Lua_Suffix, "");
                assetImporter.assetBundleName = assetpath;
                assetImporter.assetBundleVariant = "unity3d";
            }

            // luamsg
            foreach (FileInfo scenefile in dirInfo.GetFiles("*.bytes", SearchOption.AllDirectories))
            {
                string source = scenefile.FullName.Replace("\\", "/");
                string assetpath = "Assets" + source.Substring(Application.dataPath.Length);

                AssetImporter assetImporter = AssetImporter.GetAtPath(assetpath);

                assetpath = assetpath.Replace(EditorResourceLoad.EDITOR_PATH_PREFIX, "");
                assetpath = assetpath.Replace(Bytes_Suffix, "");
                assetImporter.assetBundleName = assetpath;
                assetImporter.assetBundleVariant = "unity3d";
            }

            AssetDatabase.Refresh();
        }

        private static void PackageScene(string rootpath)
        {
            foreach (var subdir in SceneDir)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(rootpath + subdir);

                foreach (FileInfo scenefile in dirInfo.GetFiles("*.unity", SearchOption.AllDirectories))
                {
                    string source = scenefile.FullName.Replace("\\", "/");
                    string assetpath = "Assets" + source.Substring(Application.dataPath.Length);

                    AssetImporter assetImporter = AssetImporter.GetAtPath(assetpath);

                    assetpath = assetpath.Replace(EditorResourceLoad.EDITOR_PATH_PREFIX, "");
                    assetpath = assetpath.Replace(".unity", "");
                    assetImporter.assetBundleName = assetpath;
                    assetImporter.assetBundleVariant = "unity3d";
                }
            }

            AssetDatabase.Refresh();
        }

        private static void SetAssetBundleName(string rootpath)
        {
            PackageFont(rootpath);
            PackageTexture(rootpath);
            PackageAudio(rootpath);
            PackagePrefab(rootpath);
            PackageScene(rootpath);
            PackageConfig(rootpath);
        }

        private static void BuildFileIndex()
        {
            string resPath = Application.streamingAssetsPath;

            ///----------------------创建文件列表-----------------------
            string newFilePath = resPath + "/files.txt";
            if (System.IO.File.Exists(newFilePath))
                System.IO.File.Delete(newFilePath);

            List<string> files = new List<string>();
            Recursive(resPath, ref files);

            FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);
            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];
                if (file.EndsWith(".meta") || file.Contains(".DS_Store"))
                    continue;

                string md5 = Util.Md5file(file);
                string value = file.Replace(resPath, string.Empty);
                sw.WriteLine(value + "|" + md5);
            }

            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 遍历目录及其子目录
        /// </summary>
        private static void Recursive(string path, ref List<string> files)
        {
            string[] names = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);

            foreach (string filename in names)
            {
                string ext = Path.GetExtension(filename);
                if (ext.Equals(".meta"))
                    continue;

                files.Add(filename.Replace('\\', '/'));
            }

            foreach (string dir in dirs)
            {
                Recursive(dir, ref files);
            }
        }

        /// <summary>
        /// 获取所有的场景
        /// </summary>
        /// <returns></returns>
        private static List<string> GetAllScenes()
        {
            List<string> scenes = new List<string>();
            scenes.Clear();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (!scene.enabled)
                    continue;

                scenes.Add(scene.path);
            }

            return scenes;
        }

        /// <summary>
        /// 获取android建造路径
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        private static string GetAndroidBuildPath(EAndroidBuildPlatform platform)
        {
            string prefix = "../../gameapp/";
            if (platform == EAndroidBuildPlatform.NoPlatform)
            {
                return string.Format("{0}{1}.apk", prefix, AppConst.APP_NAME);
            }

            return "";
        }
    }

}