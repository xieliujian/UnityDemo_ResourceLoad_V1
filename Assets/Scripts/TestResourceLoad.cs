using gtm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestResourceLoad : MonoBehaviour
{
    const int TYPE_HEIGHT = 100;

    const int BUTTON_WIDTH = 300;

    ResourceLoad m_ResLoad = new ResourceLoad();

    AsyncTaskManager m_AsyncTaskMgr = new AsyncTaskManager();

    // Start is called before the first frame update
    void Start()
    {
        m_ResLoad.DoInit();
        m_AsyncTaskMgr.DoInit();
    }

    // Update is called once per frame
    void Update()
    {
        m_ResLoad.DoUpdate();
        m_AsyncTaskMgr.DoUpdate();
    }

    void OnGUI()
    {
        GUI.skin.box.fontSize = 50;
        GUI.skin.button.fontSize = 50;

        GUILayout.BeginArea(new Rect(10, 10, BUTTON_WIDTH * 2, 1000));

        GUILayout.BeginHorizontal();

        var strPackBtnName = ResourceLoad.useAssetBundle ? "包模式加载" : "离散模式加载";

        if (GUILayout.Button(strPackBtnName, GUILayout.Width(BUTTON_WIDTH * 2), GUILayout.Height(TYPE_HEIGHT)))
        {
            ResourceLoad.useAssetBundle = !ResourceLoad.useAssetBundle;
        }

        GUILayout.EndHorizontal();

        // ui
        AddCategory("UI", "ui/uiprefab/", "ui_panel_battle", ".prefab");

        // 图片
        AddCategory("Sprite", "ui/icon/", "common0", ".png");

        // asset文件
        AddCategory("Lua", "config/lua/", "luapackage", ".asset");

        // 二进制数据
        AddCategory("Lua消息", "config/lua/", "luamsg", ".bytes", ResourceType.Bytes);

        // 声音
        AddCategory("声音", "audio/music/", "bangpai", ".mp3");

        GUILayout.EndArea();
    }

    void AddCategory(string category, string path, string fileName, string suffix, ResourceType resType = ResourceType.Default)
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button($"{category}同步", GUILayout.Width(BUTTON_WIDTH), GUILayout.Height(TYPE_HEIGHT)))
        {
            var res = m_ResLoad.LoadResourceSync(path, fileName, suffix, resType);
            int m = 0;
        }

        if (GUILayout.Button($"{category}异步", GUILayout.Width(BUTTON_WIDTH), GUILayout.Height(TYPE_HEIGHT)))
        {
            m_ResLoad.LoadResourceAsync(path, fileName, suffix, (obj) => 
            {
                int m = 0;
            }, resType);
        }

        GUILayout.EndHorizontal();
    }
}
