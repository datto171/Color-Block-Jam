using System;
using System.Collections.Generic;
using UnityEngine;

namespace MatchStack
{
    [Serializable]
    public class TileLayer
    {
        public int z;              // index layer
        public int width;
        public int height;
        public float offsetX;      // offset so với layer 0
        public float offsetY;
        public string[,] tiles;    // mảng 2D chứa type của từng ô
    }

    [CreateAssetMenu(fileName = "LevelData", menuName = "MatchStack/LevelData")]
    public class LevelData : ScriptableObject
    {
        public string levelId;
        public List<TileLayer> layers = new List<TileLayer>();
    }

}