using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Leap {

    public class MapEntity : MonoBehaviour {

        public int typeID;
        public TileBase tileBase_terrain;

        public Vector2 constraintSize;
        public Vector2 constraintCenter;

        public Dictionary<Vector2Int, int> terrainTypeIDDict;

        [SerializeField] Tilemap tilemap_terrain;
        public Tilemap Tilemap_Terrain => tilemap_terrain;

        public void Ctor() {
            terrainTypeIDDict = new Dictionary<Vector2Int, int>();
        }

        public void Pos_Set(Vector2Int pos) {
            transform.position = pos.ToVector3Int();
        }

        public void Terrain_Set(Vector2Int pos, TileBase tile, int typeID) {
            tilemap_terrain.SetTile(pos.ToVector3Int(), tile);
            terrainTypeIDDict[pos] = typeID;
        }

        public bool Terrain_GetTypeID(Vector2Int pos, out int typeID) {
            if (terrainTypeIDDict.ContainsKey(pos)) {
                typeID = terrainTypeIDDict[pos];
                return true;
            }
            typeID = -1;
            return false;
        }

        public void Terrain_ClearAll() {
            tilemap_terrain.ClearAllTiles();
        }

        public void TearDown() {
            Destroy(gameObject);
        }

    }

}