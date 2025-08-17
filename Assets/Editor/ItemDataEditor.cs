using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BagFight
{
    [CustomEditor(typeof(ItemData))]
    public class ItemDataEditor : Editor
    {
        private SerializedProperty gridSize;
        private SerializedProperty slots;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            gridSize = serializedObject.FindProperty("gridSize");
            slots = serializedObject.FindProperty("slotsItem");
            DrawGridButtons(gridSize.vector2IntValue, slots);
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGridButtons(Vector2Int vector2Int, SerializedProperty slots)
        {
            slots.arraySize = vector2Int.x * vector2Int.y;

            for (int y = 0; y < vector2Int.y; y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < vector2Int.x; x++)
                {
                    var index = y * vector2Int.x + x;
                    var slot = slots.GetArrayElementAtIndex(index);
                    var buttonLabel = slot.boolValue ? "x" : "";
                    
                    if (GUILayout.Button(buttonLabel, GUILayout.Width(30), GUILayout.Height(30)))
                    {
                        slot.boolValue = !slot.boolValue;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}