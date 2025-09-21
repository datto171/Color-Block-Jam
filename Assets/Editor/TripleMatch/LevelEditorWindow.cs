// using UnityEngine;
// using UnityEditor;
// using System.IO;
//
// namespace TripleMatch
// {
//     public class LevelEditorWindow : EditorWindow
//     {
//         private LevelData currentLevel;
//         private string jsonPath = "Assets/Levels/";
//
//         [MenuItem("Tools/MatchStack/Level Editor")]
//         public static void ShowWindow()
//         {
//             GetWindow<LevelEditorWindow>("Level Editor");
//         }
//
//         private void OnGUI()
//         {
//             currentLevel = (LevelData)EditorGUILayout.ObjectField("Level Data", currentLevel, typeof(LevelData), false);
//
//             if (currentLevel == null) return;
//
//             if (GUILayout.Button("Add Tile"))
//             {
//                 currentLevel.tiles.Add(new TileInstance());
//             }
//
//             SerializedObject so = new SerializedObject(currentLevel);
//             SerializedProperty tilesProp = so.FindProperty("tiles");
//             EditorGUILayout.PropertyField(tilesProp, true);
//             so.ApplyModifiedProperties();
//
//             GUILayout.Space(10);
//
//             if (GUILayout.Button("Save as JSON"))
//             {
//                 SaveLevelAsJson(currentLevel);
//             }
//         }
//
//         private void SaveLevelAsJson(LevelData level)
//         {
//             if (!Directory.Exists(jsonPath))
//                 Directory.CreateDirectory(jsonPath);
//
//             string json = JsonUtility.ToJson(level, true);
//             string path = Path.Combine(jsonPath, level.levelId + ".json");
//             File.WriteAllText(path, json);
//             Debug.Log("Saved to: " + path);
//             AssetDatabase.Refresh();
//         }
//     }
// }