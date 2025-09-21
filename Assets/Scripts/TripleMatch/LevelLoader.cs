using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace MatchStack
{
    [System.Serializable]
    public class LevelJson
    {
        public List<LayerJson> layers;
    }

    [System.Serializable]
    public class LayerJson
    {
        public int sizeX, sizeY;
        public float offsetX, offsetY;
        public string[] tiles; // mảng 1D, serialize được
    }

    public class LevelLoader : MonoBehaviour
    {
        [Header("Config")]
        public string jsonFileName = "LevelData.json"; // file trong Assets/Levels/
        public GameObject tilePrefab;                  // Prefab để hiển thị Block
        public Transform root;                         // Cha để chứa tất cả layer
        public float layerSpacing = 0.5f;              // khoảng cách Z giữa các layer

        private LevelJson levelJson;

        void Start()
        {
            LoadLevel();
            SpawnLevel();
        }

        void LoadLevel()
        {
            string path = Path.Combine(Application.dataPath, "Levels", jsonFileName);
            if (!File.Exists(path))
            {
                Debug.LogError("Level JSON not found: " + path);
                return;
            }

            string json = File.ReadAllText(path);
            levelJson = JsonUtility.FromJson<LevelJson>(json);
        }

        void SpawnLevel()
        {
            if (levelJson == null || levelJson.layers == null) return;
            if (tilePrefab == null)
            {
                Debug.LogError("Tile prefab not assigned!");
                return;
            }

            for (int i = 0; i < levelJson.layers.Count; i++)
            {
                var l = levelJson.layers[i];

                // tạo cha cho layer
                GameObject layerRoot = new GameObject($"Layer_{i}");
                if (root != null) layerRoot.transform.SetParent(root);
                layerRoot.transform.localPosition = new Vector3(0, 0, i * layerSpacing);

                for (int y = 0; y < l.sizeY; y++)
                {
                    for (int x = 0; x < l.sizeX; x++)
                    {
                        int index = y * l.sizeX + x;
                        string cell = (l.tiles != null && index < l.tiles.Length) ? l.tiles[index] : ".";

                        // vị trí tile
                        Vector3 pos = new Vector3(x + l.offsetX, y + l.offsetY, 0);

                        // instantiate tile
                        var obj = Instantiate(tilePrefab, layerRoot.transform);
                        obj.transform.localPosition = pos;
                        obj.name = $"Tile_{x}_{y}_{(string.IsNullOrEmpty(cell) ? "." : cell)}";

                        // Nếu muốn phân biệt Block / Empty thì thay màu
                        var rend = obj.GetComponent<Renderer>();
                        if (rend != null)
                        {
                            if (cell == "." || string.IsNullOrEmpty(cell))
                                rend.material.color = Color.gray; // ô trống
                            else
                                rend.material.color = Color.green; // có tile
                        }
                    }
                }
            }
        }
    }
}
