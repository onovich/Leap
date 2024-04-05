using System;
using System.Collections.Generic;
using TriInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Leap.Modifier {

    public class MapEditorEntity : MonoBehaviour {

        [SerializeField] int typeID;
        [SerializeField] GameObject mapSize;
        [SerializeField] MapTM mapTM;
        [SerializeField] Tilemap tilemap_terrain;
        [SerializeField] TileBase tilebase_terrain;

        [Button("Bake")]
        void Bake() {
            BakeTerrain();
            BakeMapInfo();
            BakeBlock();
            BakeSpawnPoint();

            EditorUtility.SetDirty(mapTM);
            AssetDatabase.SaveAssets();
            Debug.Log("Bake Sucess");
        }

        void BakeMapInfo() {
            mapTM.typeID = typeID;
            mapTM.tileBase_terrain = tilebase_terrain;
            mapTM.mapSize = mapSize.transform.localScale.RoundToVector2Int();
        }

        // void BakeTerrain() {
        //     var terrainSpawnPosList = new List<Vector2Int>();
        //     for (int x = tilemap_terrain.cellBounds.x; x < tilemap_terrain.cellBounds.xMax; x++) {
        //         for (int y = tilemap_terrain.cellBounds.y; y < tilemap_terrain.cellBounds.yMax; y++) {
        //             var pos = new Vector3Int(x, y, 0);
        //             var tile = tilemap_terrain.GetTile(pos);
        //             if (tile == null) continue;
        //             terrainSpawnPosList.Add(pos.ToVector2Int());
        //         }
        //     }
        //     mapTM.terrainSpawnPosArr = terrainSpawnPosList.ToArray();
        // }

        void BakeTerrain() {
            var terrainSpawnPosList = new List<Vector2Int>();
            TravelTilemap(tilemap_terrain, (tile, pos) => {
                var tilebase = tilebase_terrain;
                if (tilebase.name == tile.name) {
                    terrainSpawnPosList.Add(pos);
                    return;
                }
            });
            mapTM.terrainSpawnPosArr = terrainSpawnPosList.ToArray();
        }

        void BakeBlock() {
            List<BlockTM> blockTMList = new List<BlockTM>();
            List<Vector2Int> blockSpawnPosList = new List<Vector2Int>();
            List<Vector2Int> blockSpawnSizeList = new List<Vector2Int>();
            var group = transform.GetChild(2);
            var blockEditors = group.GetComponentsInChildren<BlockEditorEntity>();
            if (blockEditors == null) {
                Debug.Log("BlockEditors Not Found");
            }
            for (int i = 0; i < blockEditors.Length; i++) {
                var editor = blockEditors[i];
                editor.Rename();

                var tm = editor.blockTM;
                blockTMList.Add(tm);

                var posInt = editor.GetPosInt();
                blockSpawnPosList.Add(posInt);

                var sizeInt = editor.GetSizeInt();
                blockSpawnSizeList.Add(sizeInt);
            }
            mapTM.blockSpawnArr = blockTMList.ToArray();
            mapTM.blockSpawnPosArr = blockSpawnPosList.ToArray();
            mapTM.blockSpawnSizeArr = blockSpawnSizeList.ToArray();
        }

        void BakeSpawnPoint() {
            var group = transform.GetChild(3);
            var editor = group.GetComponent<SpawnPointEditorEntity>();
            if (editor == null) {
                Debug.Log("SpawnPointEditor Not Found");
            }
            editor.Rename();
            var posInt = editor.GetPosInt();
            mapTM.SpawnPoint = posInt;
        }

        void TravelTilemap(Tilemap tilemap, Action<TileBase, Vector2Int> action) {
            for (int x = tilemap.cellBounds.x; x < tilemap.cellBounds.xMax; x++) {
                for (int y = tilemap.cellBounds.y; y < tilemap.cellBounds.yMax; y++) {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    TileBase tile = tilemap.GetTile(pos);
                    if (tile != null) {
                        action.Invoke(tile, new Vector2Int(x, y));
                    }
                }
            }
        }

    }

}