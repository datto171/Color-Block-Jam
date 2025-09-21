using UnityEngine;
using UnityEditor;

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
            currentLevel = (LevelData)EditorGUILayout.ObjectField("Level Data", currentLevel, typeof(LevelData), false);

            if (currentLevel == null) return;

            GUILayout.Space(10);
            if (GUILayout.Button("Add New Layer"))
            {
                var newLayer = new TileLayer
                {
                    z = currentLevel.layers.Count,
                    width = 10,
                    height = 10,
                    offsetX = 0f,
                    offsetY = 0f,
                    tiles = new string[10, 10]
                };
                currentLevel.layers.Add(newLayer);
            }

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

            GUILayout.Space(10);
            currentTileType = EditorGUILayout.TextField("Tile Type", currentTileType);

            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(400));
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
        }
    }
}