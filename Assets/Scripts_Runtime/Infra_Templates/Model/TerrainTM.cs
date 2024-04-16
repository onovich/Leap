using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Leap {

    [CreateAssetMenu(fileName = "SO_Terrain", menuName = "Leap/TerrainTM")]
    public class TerrainTM : ScriptableObject {

        public int typeID;
        public TileBase tile;
        public float fallingFriction;

    }

}