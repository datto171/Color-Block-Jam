using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class LevelEditorWindow
{
    // ---------------- GRID ----------------
    private int rows = 10;
    private int columns = 10;
    private float cellSize = 38f;
    private float cellSpacing = 2f;

    private int[,] gridData;        // 0 = trống, >0 = index item
    private int[,] itemInstanceId;  // lưu id instance để biết ô nào cùng 1 item
    private int nextInstanceId = 1;
    private Vector2 scrollPos;

    // ---------------- ITEM PALETTE ----------------
    [Header("Item Database")] 
    private ItemData[] items;
    private int selectedItemIndex = -1;
    private bool deleteMode = false;

    private LevelEditorDataTab dataTab = new LevelEditorDataTab();

    // ---------------- SAVE/LOAD ----------------
    private string selectedMapPath = "";
    private string defaultSavePath = "Assets/GameResource/Levels/new_map.json";

    // ---------------- CONSTRUCTOR ----------------
    public LevelEditorWindow()
    {
        InitGrid();

        // Load item database từ dataTab
        items = dataTab.GetItemDatabase().ToArray();

        // Chọn item cuối cùng trong list nếu có
        if (items != null && items.Length > 0)
            selectedItemIndex = items.Length - 1;

        // Tìm file map mới nhất trong folder, nếu có thì load
        string levelsFolder = "Assets/GameResource/Levels";
        if (!Directory.Exists(levelsFolder))
            Directory.CreateDirectory(levelsFolder);

        string[] files = Directory.GetFiles(levelsFolder, "*.json");
        if (files.Length > 0)
        {
            selectedMapPath = files.OrderByDescending(f => File.GetCreationTime(f)).First();
            LoadMapFromPath(selectedMapPath);
        }
        else
        {
            // Nếu không có map nào thì tạo map trắng và set selectedMapPath
            selectedMapPath = Path.Combine(levelsFolder, "new_map.json");
            SaveMap(); // lưu map trắng
        }
    }


    // ---------------- INIT GRID ----------------
    private void InitGrid()
    {
        gridData = new int[rows, columns];
        itemInstanceId = new int[rows, columns];
        nextInstanceId = 1;
    }

    // ---------------- DRAW GUI ----------------
    public void DrawGUI()
    {
        EditorGUILayout.LabelField("Level Editor", EditorStyles.boldLabel);
        rows = EditorGUILayout.IntField("Rows", rows);
        columns = EditorGUILayout.IntField("Columns", columns);

        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();

        DrawLoadPanel();
        DrawGridPanel();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawLoadPanel()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(250));
        EditorGUILayout.LabelField("Load Level", EditorStyles.boldLabel);

        string levelsFolder = "Assets/GameResource/Levels";
        if (!Directory.Exists(levelsFolder))
            Directory.CreateDirectory(levelsFolder);

        string[] files = Directory.GetFiles(levelsFolder, "*.json");

        for (int i = 0; i < files.Length; i++)
        {
            string fileName = Path.GetFileName(files[i]);
            Color oldColor = GUI.backgroundColor;
            if (files[i] == selectedMapPath) GUI.backgroundColor = Color.yellow;

            if (GUILayout.Button(fileName))
            {
                LoadMapFromPath(files[i]);
                selectedMapPath = files[i];
            }

            GUI.backgroundColor = oldColor;
        }

        if (GUILayout.Button("Refresh List"))
            Debug.Log("Refreshed map list");

        if (GUILayout.Button("Create Map"))
        {
            InitGrid(); // reset grid

            // Tạo file map mới
            string newFileName = "new_map.json";
            string newPath = Path.Combine(levelsFolder, newFileName);
            int counter = 1;
            while (File.Exists(newPath)) // tránh trùng tên
            {
                newFileName = $"new_map_{counter}.json";
                newPath = Path.Combine(levelsFolder, newFileName);
                counter++;
            }

            // Tạo file trắng
            MapData blankMap = new MapData
            {
                rows = rows,
                columns = columns,
                gridData = Flatten2DArray(gridData),
                itemInstanceId = Flatten2DArray(itemInstanceId)
            };
            File.WriteAllText(newPath, JsonUtility.ToJson(blankMap, true));
            
            AssetDatabase.Refresh();

            selectedMapPath = newPath;
            Debug.Log("🆕 New empty map created at: " + newPath);
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawGridPanel()
    {
        EditorGUILayout.BeginVertical();

        items = dataTab.GetItemDatabase().ToArray();

        if (GUILayout.Button("Generate Grid")) InitGrid();

        EditorGUILayout.Space();
        DrawPalette();
        EditorGUILayout.Space();
        ToggleDeleteMode();
        EditorGUILayout.Space();
        DrawGrid();
        EditorGUILayout.Space();

        if (GUILayout.Button("Save Map")) SaveMap();

        EditorGUILayout.EndVertical();
    }

    private void DrawPalette()
    {
        EditorGUILayout.LabelField("Item Palette:", EditorStyles.boldLabel);
        if (items == null) return;

        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null) continue;
            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = (selectedItemIndex == i && !deleteMode) ? Color.yellow : Color.white;

            if (GUILayout.Button(items[i].name + $" ({items[i].gridSize.x}x{items[i].gridSize.y})",
                    GUILayout.Width(150), GUILayout.Height(30)))
            {
                selectedItemIndex = i;
                deleteMode = false;
            }

            GUI.backgroundColor = oldColor;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void ToggleDeleteMode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = deleteMode ? Color.red : Color.white;
        if (GUILayout.Button(deleteMode ? "Delete Mode (ON)" : "Delete Mode (OFF)", GUILayout.Height(30)))
            deleteMode = !deleteMode;
        GUI.backgroundColor = oldColor;
    }

    private void DrawGrid()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        if (gridData == null) { EditorGUILayout.EndScrollView(); return; }

        float gridWidth = columns * (cellSize + cellSpacing) - cellSpacing;
        float gridHeight = rows * (cellSize + cellSpacing) - cellSpacing;
        Rect gridRect = GUILayoutUtility.GetRect(gridWidth, gridHeight, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Rect cellRect = new Rect(gridRect.x + x * (cellSize + cellSpacing),
                                         gridRect.y + y * (cellSize + cellSpacing),
                                         cellSize, cellSize);

                int id = gridData[y, x];
                Color oldColor = GUI.backgroundColor;

                if (id > 0 && id - 1 < items.Length && items[id - 1] != null)
                    GUI.backgroundColor = items[id - 1].color;
                else
                    GUI.backgroundColor = Color.gray;

                if (GUI.Button(cellRect, GUIContent.none))
                {
                    if (deleteMode) DeleteItem(x, y);
                    else PlaceItem(x, y);
                }

                GUI.backgroundColor = oldColor;
            }
        }

        EditorGUILayout.EndScrollView();
    }

    // ---------------- PLACE / DELETE ----------------
    private void PlaceItem(int startX, int startY)
    {
        if (selectedItemIndex < 0 || items == null || items[selectedItemIndex] == null) return;
        var item = items[selectedItemIndex];
        var gridSize = item.gridSize;

        if (startX + gridSize.x > columns || startY + gridSize.y > rows) return;

        for (int y = 0; y < gridSize.y; y++)
            for (int x = 0; x < gridSize.x; x++)
                if (item.slotsItem != null && x + y * gridSize.x < item.slotsItem.Length && item.slotsItem[x + y * gridSize.x])
                    if (gridData[startY + y, startX + x] != 0) return;

        int currentInstanceId = nextInstanceId++;
        for (int y = 0; y < gridSize.y; y++)
            for (int x = 0; x < gridSize.x; x++)
                if (item.slotsItem != null && x + y * gridSize.x < item.slotsItem.Length && item.slotsItem[x + y * gridSize.x])
                {
                    gridData[startY + y, startX + x] = selectedItemIndex + 1;
                    itemInstanceId[startY + y, startX + x] = currentInstanceId;
                }

        SaveMap();
    }

    private void DeleteItem(int x, int y)
    {
        int instanceId = itemInstanceId[y, x];
        if (instanceId == 0) return;

        for (int row = 0; row < rows; row++)
            for (int col = 0; col < columns; col++)
                if (itemInstanceId[row, col] == instanceId)
                {
                    gridData[row, col] = 0;
                    itemInstanceId[row, col] = 0;
                }

        SaveMap();
    }

    // ---------------- SAVE / LOAD ----------------
    [System.Serializable]
    public class MapData
    {
        public int rows;
        public int columns;
        public int[] gridData;
        public int[] itemInstanceId;
    }

    private int[] Flatten2DArray(int[,] array2D)
    {
        int[] flat = new int[rows * columns];
        for (int y = 0; y < rows; y++)
            for (int x = 0; x < columns; x++)
                flat[y * columns + x] = array2D[y, x];
        return flat;
    }

    private int[,] Unflatten2DArray(int[] flat)
    {
        int[,] array2D = new int[rows, columns];
        for (int y = 0; y < rows; y++)
            for (int x = 0; x < columns; x++)
                array2D[y, x] = flat[y * columns + x];
        return array2D;
    }

    private void SaveMap()
    {
        if (gridData == null) return;

        MapData data = new MapData
        {
            rows = rows,
            columns = columns,
            gridData = Flatten2DArray(gridData),
            itemInstanceId = Flatten2DArray(itemInstanceId)
        };

        string path = string.IsNullOrEmpty(selectedMapPath) ? defaultSavePath : selectedMapPath;
        File.WriteAllText(path, JsonUtility.ToJson(data, true));
        Debug.Log("💾 Map saved to " + path);
    }

    public void LoadMapFromPath(string path)
    {
        if (!File.Exists(path)) { Debug.LogWarning("⚠ File not found: " + path); return; }

        string json = File.ReadAllText(path);
        MapData data = JsonUtility.FromJson<MapData>(json);

        rows = data.rows;
        columns = data.columns;
        gridData = Unflatten2DArray(data.gridData);
        itemInstanceId = Unflatten2DArray(data.itemInstanceId);

        // Fix instanceId trùng: đảm bảo uniqueness
        var existingIds = itemInstanceId.Cast<int>().Where(id => id > 0).Distinct().ToList();
        nextInstanceId = existingIds.Count > 0 ? existingIds.Max() + 1 : 1;

        Debug.Log("📂 Level loaded from " + path);
    }
}
