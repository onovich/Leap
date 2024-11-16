using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Leap {

    [CreateAssetMenu(fileName = "SO_Map", menuName = "Leap/MapTM")]
    public class MapTM : ScriptableObject {

        public int typeID;

        public Vector2Int mapPos;
        public TileBase tileBase_terrain;

        public Vector2 constraintSize;
        public Vector2 constraintCenter;

        // Terrain
        public Vector2Int[] terrainSpawnPosArr;
        public int[] terrainTypeIDArr;

        // Role Spawn 
        public Vector2 SpawnPoint;

        // Block Spawn
        public BlockTM[] blockSpawnArr;
        public Vector2Int[] blockSpawnPosArr;
        public Vector2Int[] blockSpawnSizeArr;
        public Vector2[] blockSpawnOffsetArr;
        public int[] blockSpawnIndexArr;

        // Camera
        public Vector2 cameraConfinerWorldMax;
        public Vector2 cameraConfinerWorldMin;

        // Spike Spawn
        public SpikeTM[] spikeSpawnArr;
        public Vector2Int[] spikeSpawnPosArr;
        public Vector2[] spikeSpawnSizeArr;
        public Vector2[] spikeSpawnOffsetArr;
        public int[] spikeSpawnRotationZArr;
        public int[] spikeSpawnIndexArr;

        // Track
        [Header("Path")]
        public int[] pathIndexArr;
        public PathTM[] pathTMArr;
        public PathSpawnTM[] pathSpawnTMArr;
        public EntityType[] pathTravelerTypeArr;
        public int[] pathTravelerIndexArr;
        public bool[] pathIsCircleLoopArr;
        public bool[] pathIsPingPongLoopArr;
        public Vector2[] pathTravelerHalfSizeArr;

    }

}