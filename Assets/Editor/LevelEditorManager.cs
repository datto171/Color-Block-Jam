using UnityEditor;
using UnityEngine;

public class LevelEditorManager : EditorWindow
{
    private int currentTab;
    private string[] tabTitles = { "Level Editor", "Data Import" };

    private LevelEditorWindow levelEditorTab;
    private LevelEditorDataTab dataTab;

    [MenuItem("Window/Custom Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorManager>("Level Editor Manager");
    }

    private void OnEnable()
    {
        levelEditorTab = new LevelEditorWindow();
        dataTab = new LevelEditorDataTab();
    }

    private void OnGUI()
    {
        currentTab = GUILayout.Toolbar(currentTab, tabTitles);

        GUILayout.Space(10);

        switch (currentTab)
        {
            case 0:
                levelEditorTab.DrawGUI(); // ✅ gọi GUI của LevelEditorWindow
                break;
            case 1:
                dataTab.OnGUI();
                break;
        }
    }
}