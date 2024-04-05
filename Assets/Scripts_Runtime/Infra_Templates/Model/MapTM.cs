using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Leap {

    [CreateAssetMenu(fileName = "SO_Map", menuName = "Leap/MapTM")]
    public class MapTM : ScriptableObject {

        public int typeID;

        public Vector2Int mapSize;
        public TileBase tileBase_terrain;

        // Terrain
        public Vector2Int[] terrainSpawnPosArr;

        // Role Spawn 
        public Vector2 SpawnPoint;

        // Block Spawn
        public BlockTM[] blockSpawnArr;
        public Vector2Int[] blockSpawnPosArr;
        public Vector2Int[] blockSpawnSizeArr;

    }

}