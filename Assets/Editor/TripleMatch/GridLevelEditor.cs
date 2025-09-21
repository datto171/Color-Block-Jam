using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace MatchStack
{
    public class GridLevelEditor : EditorWindow
    {
        private LevelData currentLevel;
        private int selectedLayer = 0;
        private string currentTileType = "Block";

        private float cellSize = 30f;
        private Vector2 scroll;

        [MenuItem("Tools/MatchStack/Grid Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<GridLevelEditor>("Grid Level Editor");
        }

        private void OnGUI()
        {
            // chọn LevelData
            currentLevel = (LevelData)EditorGUILayout.ObjectField("Level Data", currentLevel, typeof(LevelData), false);
            if (currentLevel == null) return;

            GUILayout.Space(5);

            // hàng nút nhỏ
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("➕ Add Layer", GUILayout.Width(100)))
            {
                var newLayer = new TileLayer
                {
                    z = currentLevel.layers.Count,
                    width = 5,
                    height = 5,
                    offsetX = 0f,
                    offsetY = 0f,
                    tiles = new string[5, 5]
                };
                currentLevel.layers.Add(newLayer);
                selectedLayer = currentLevel.layers.Count - 1;
            }

            if (GUILayout.Button("💾 Save All", GUILayout.Width(100)))
            {
                SaveAsset();
                Debug.Log($"Saved all layers of LevelData {currentLevel.name}");
            }

            if (GUILayout.Button("📤 Export JSON", GUILayout.Width(100)))
            {
                ExportJson(currentLevel);
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (currentLevel.layers.Count > 0)
            {
                selectedLayer = EditorGUILayout.IntSlider("Layer", selectedLayer, 0, currentLevel.layers.Count - 1);
                DrawLayerEditor(currentLevel.layers[selectedLayer]);
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(currentLevel);
            }
        }

        private void DrawLayerEditor(TileLayer layer)
        {
            EditorGUILayout.LabelField($"Editing Layer {layer.z}", EditorStyles.boldLabel);

            layer.width = EditorGUILayout.IntField("Width", layer.width);
            layer.height = EditorGUILayout.IntField("Height", layer.height);
            layer.offsetX = EditorGUILayout.FloatField("Offset X", layer.offsetX);
            layer.offsetY = EditorGUILayout.FloatField("Offset Y", layer.offsetY);

            if (layer.tiles == null || layer.tiles.GetLength(0) != layer.width ||
                layer.tiles.GetLength(1) != layer.height)
            {
                layer.tiles = new string[layer.width, layer.height];
            }

            GUILayout.Space(5);
            currentTileType = EditorGUILayout.TextField("Tile Type", currentTileType);

            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(300));
            for (int y = 0; y < layer.height; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < layer.width; x++)
                {
                    string label = string.IsNullOrEmpty(layer.tiles[x, y]) ? "." : layer.tiles[x, y].Substring(0, 1);
                    if (GUILayout.Button(label, GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                    {
                        if (string.IsNullOrEmpty(layer.tiles[x, y]))
                            layer.tiles[x, y] = currentTileType;
                        else
                            layer.tiles[x, y] = ""; // toggle off
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(5);
            if (GUILayout.Button("💾 Save This Layer", GUILayout.Height(25)))
            {
                SaveAsset();
                Debug.Log($"Saved Layer {layer.z} of LevelData {currentLevel.name}");
            }
        }

        private void SaveAsset()
        {
            EditorUtility.SetDirty(currentLevel);
            AssetDatabase.SaveAssets();
        }

        private void ExportJson(LevelData level)
        {
            var jsonObj = new LevelJson();
            jsonObj.layers = new List<LayerJson>();

            foreach (var l in level.layers)
            {
                if (l.tiles == null) l.tiles = new string[l.width, l.height];

                var lj = new LayerJson
                {
                    sizeX = l.width,
                    sizeY = l.height,
                    offsetX = l.offsetX,
                    offsetY = l.offsetY,
                    tiles = new string[l.width * l.height]
                };

                for (int y = 0; y < l.height; y++)
                {
                    for (int x = 0; x < l.width; x++)
                    {
                        string cell = l.tiles[x, y];
                        if (string.IsNullOrEmpty(cell)) cell = ".";
                        lj.tiles[y * l.width + x] = cell;
                    }
                }

                jsonObj.layers.Add(lj);
            }

            string dir = "Assets/Levels/";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            string path = dir + level.name + ".json";

            string json = JsonUtility.ToJson(jsonObj, true);
            File.WriteAllText(path, json);
            AssetDatabase.Refresh();

            Debug.Log("Exported JSON to " + path);
        }
    }
}