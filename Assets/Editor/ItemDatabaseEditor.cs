// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;
//
// [System.Serializable]
// public class ItemData
// {
//     public int width = 2;
//     public int height = 2;
//     public List<Vector2Int> occupiedCells = new List<Vector2Int>();
//     public Color color = Color.green;
// }
//
// [CustomEditor(typeof(ItemDatabase))]
// public class ItemDatabaseEditor : Editor
// {
//     private int cellSize = 30;
//     private int cellSpacing = 4;
//
//     private ItemDatabase db;
//     private Vector2 scrollPos;
//
//     private void OnEnable()
//     {
//         db = (ItemDatabase)target;
//     }
//
//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector();
//
//         if (GUILayout.Button("Add New Item"))
//         {
//             db.items.Add(new ItemData());
//         }
//
//         scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
//         for (int i = 0; i < db.items.Count; i++)
//         {
//             EditorGUILayout.BeginVertical("box");
//
//             var item = db.items[i];
//             item.width = EditorGUILayout.IntField("Width", item.width);
//             item.height = EditorGUILayout.IntField("Height", item.height);
//             item.color = EditorGUILayout.ColorField("Color", item.color);
//
//             // Vẽ grid
//             Rect gridRect = GUILayoutUtility.GetRect(item.width * (cellSize + cellSpacing), item.height * (cellSize + cellSpacing));
//
//             Handles.BeginGUI();
//             for (int x = 0; x < item.width; x++)
//             {
//                 for (int y = 0; y < item.height; y++)
//                 {
//                     Vector2Int cellPos = new Vector2Int(x, y);
//                     Rect r = new Rect(
//                         gridRect.x + x * (cellSize + cellSpacing),
//                         gridRect.y + y * (cellSize + cellSpacing),
//                         cellSize,
//                         cellSize
//                     );
//
//                     // Kiểm tra ô này có được chiếm không
//                     bool occupied = item.occupiedCells.Contains(cellPos);
//                     EditorGUI.DrawRect(r, occupied ? item.color : Color.gray * 0.5f);
//
//                     // Nếu click chuột trái -> toggle
//                     if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition))
//                     {
//                         if (Event.current.button == 0)
//                         {
//                             if (occupied)
//                                 item.occupiedCells.Remove(cellPos);
//                             else
//                                 item.occupiedCells.Add(cellPos);
//
//                             Event.current.Use();
//                             Repaint();
//                         }
//
//                         // Nếu click chuột phải -> xóa toàn bộ item
//                         if (Event.current.button == 1)
//                         {
//                             db.items.RemoveAt(i);
//                             Event.current.Use();
//                             Repaint();
//                             GUIUtility.ExitGUI();
//                         }
//                     }
//
//                     // Vẽ border
//                     Handles.color = Color.black;
//                     Handles.DrawAAPolyLine(2, new Vector3[] {
//                         new Vector3(r.x, r.y),
//                         new Vector3(r.xMax, r.y),
//                         new Vector3(r.xMax, r.yMax),
//                         new Vector3(r.x, r.yMax),
//                         new Vector3(r.x, r.y)
//                     });
//                 }
//             }
//             Handles.EndGUI();
//
//             EditorGUILayout.EndVertical();
//         }
//         EditorGUILayout.EndScrollView();
//
//         if (GUI.changed)
//         {
//             EditorUtility.SetDirty(db);
//         }
//     }
// }
