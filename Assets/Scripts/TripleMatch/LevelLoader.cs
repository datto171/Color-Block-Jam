using UnityEngine;
using System.IO;

namespace MatchStack
{
    public class LevelLoader : MonoBehaviour
    {
        public string levelFile = "Assets/Levels/Level1.json";

        public LevelData LoadLevel()
        {
            if (File.Exists(levelFile))
            {
                string json = File.ReadAllText(levelFile);
                LevelData level = ScriptableObject.CreateInstance<LevelData>();
                JsonUtility.FromJsonOverwrite(json, level);
                return level;
            }
            else
            {
                Debug.LogError("Level file not found: " + levelFile);
                return null;
            }
        }
    }
}