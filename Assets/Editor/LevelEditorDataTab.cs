using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LevelEditorDataTab
{
    // ✅ Đường dẫn cố định
    private const string fixedPath = "Assets/GameResource/DataConfig";

    private List<ItemData> itemDatabase = new List<ItemData>();
    private Vector2 scrollPos;

    public LevelEditorDataTab()
    {
        // ✅ Khi khởi tạo tab thì load luôn
        LoadItemDataFromFolder();
    }

    public List<ItemData> GetItemDatabase() => itemDatabase;

    public void OnGUI()
    {
        GUILayout.Label("Item Database", EditorStyles.boldLabel);

        if (itemDatabase != null && itemDatabase.Count > 0)
        {
            GUILayout.Space(5);
            GUILayout.Label("Item Database (" + itemDatabase.Count + ")", EditorStyles.boldLabel);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));
            foreach (var item in itemDatabase)
            {
                if (item == null) continue;

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                Texture2D preview = item.itemImage != null
                    ? AssetPreview.GetAssetPreview(item.itemImage)
                    : AssetPreview.GetMiniThumbnail(item);

                GUILayout.Label(preview, GUILayout.Width(40), GUILayout.Height(40));

                if (GUILayout.Button(item.name, GUILayout.Height(40)))
                {
                    // ✅ Ping và chọn item trong Project
                    EditorGUIUtility.PingObject(item);
                    Selection.activeObject = item;
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("⚠ No ItemData found in " + fixedPath, EditorStyles.helpBox);
        }
    }

    private void LoadItemDataFromFolder()
    {
        itemDatabase.Clear();
        string[] guids = AssetDatabase.FindAssets("t:ItemData", new[] { fixedPath });
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
            if (item != null) itemDatabase.Add(item);
        }
        Debug.Log("✅ Loaded " + itemDatabase.Count + " ItemData from " + fixedPath);
    }
}
