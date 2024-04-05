using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Leap {

    public class MapEntity : MonoBehaviour {

        public int typeID;
        public Vector2Int mapSize;
        public TileBase tileBase_terrain;

        [SerializeField] Tilemap tilemap_terrain;

        public void Ctor() { }

        public void Terrain_Set(Vector2Int pos, TileBase tile) {
            tilemap_terrain.SetTile(pos.ToVector3Int(), tile);
        }

        public void Terrain_ClearAll() {
            tilemap_terrain.ClearAllTiles();
        }

        public void TearDown() {
            Destroy(gameObject);
        }

    }

}