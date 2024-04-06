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
        [SerializeField] Transform blockGroup;
        [SerializeField] Transform spikeGroup;
        [SerializeField] Transform spawnPointGroup;
        [SerializeField] Vector2 cameraConfinerWorldMax;
        [SerializeField] Vector2 cameraConfinerWorldMin;

        IndexService indexService;

        [Button("Bake")]
        void Bake() {
            indexService = new IndexService();
            indexService.ResetIndex();
            BakeTerrain();
            BakeMapInfo();
            BakeBlock();
            BakeSpike();
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

        void BakeTerrain() {
            var terrainSpawnPosList = new List<Vector2Int>();
            for (int x = tilemap_terrain.cellBounds.x; x < tilemap_terrain.cellBounds.xMax; x++) {
                for (int y = tilemap_terrain.cellBounds.y; y < tilemap_terrain.cellBounds.yMax; y++) {
                    var pos = new Vector3Int(x, y, 0);
                    var tile = tilemap_terrain.GetTile(pos);
                    if (tile == null) continue;
                    terrainSpawnPosList.Add(pos.ToVector2Int());
                }
            }
            mapTM.terrainSpawnPosArr = terrainSpawnPosList.ToArray();
        }

        void BakeSpike() {
            List<SpikeTM> spikeTMList = new List<SpikeTM>();
            List<Vector2Int> spikeSpawnPosList = new List<Vector2Int>();
            List<Vector2Int> spikeSpawnSizeList = new List<Vector2Int>();
            List<int> spikeSpawnRotationZList = new List<int>();
            List<int> spikeIndexList = new List<int>();
            var spikeEditors = spikeGroup.GetComponentsInChildren<SpikerEditorEntity>();
            if (spikeEditors == null) {
                Debug.Log("BlockEditors Not Found");
            }
            for (int i = 0; i < spikeEditors.Length; i++) {
                var editor = spikeEditors[i];

                var tm = editor.spikeTM;
                spikeTMList.Add(tm);

                var posInt = editor.GetPosInt();
                spikeSpawnPosList.Add(posInt);

                var sizeInt = editor.GetSizeInt();
                spikeSpawnSizeList.Add(sizeInt);

                var zInt = editor.GetRotationZInt();
                spikeSpawnRotationZList.Add(zInt);

                var index = indexService.PickSpikeIndex();
                spikeIndexList.Add(index);
                editor.index = index;

                editor.Rename();
            }
            mapTM.spikeSpawnArr = spikeTMList.ToArray();
            mapTM.spikeSpawnPosArr = spikeSpawnPosList.ToArray();
            mapTM.spikeSpawnSizeArr = spikeSpawnSizeList.ToArray();
            mapTM.spikeSpawnRotationZArr = spikeSpawnRotationZList.ToArray();
            mapTM.spikeSpawnIndexArr = spikeIndexList.ToArray();
            mapTM.cameraConfinerWorldMax = cameraConfinerWorldMax;
            mapTM.cameraConfinerWorldMin = cameraConfinerWorldMin;
        }

        void BakeBlock() {
            List<BlockTM> blockTMList = new List<BlockTM>();
            List<Vector2Int> blockSpawnPosList = new List<Vector2Int>();
            List<Vector2Int> blockSpawnSizeList = new List<Vector2Int>();
            List<int> blockIndexList = new List<int>();
            var blockEditors = blockGroup.GetComponentsInChildren<BlockEditorEntity>();
            if (blockEditors == null) {
                Debug.Log("BlockEditors Not Found");
            }
            for (int i = 0; i < blockEditors.Length; i++) {
                var editor = blockEditors[i];

                var tm = editor.blockTM;
                blockTMList.Add(tm);

                var posInt = editor.GetPosInt();
                blockSpawnPosList.Add(posInt);

                var sizeInt = editor.GetSizeInt();
                blockSpawnSizeList.Add(sizeInt);

                var index = indexService.PickBlockIndex();
                blockIndexList.Add(index);
                editor.index = index;

                editor.Rename();
            }
            mapTM.blockSpawnArr = blockTMList.ToArray();
            mapTM.blockSpawnPosArr = blockSpawnPosList.ToArray();
            mapTM.blockSpawnSizeArr = blockSpawnSizeList.ToArray();
            mapTM.blockSpawnIndexArr = blockIndexList.ToArray();
        }

        void BakeSpawnPoint() {
            var editor = spawnPointGroup.GetComponent<SpawnPointEditorEntity>();
            if (editor == null) {
                Debug.Log("SpawnPointEditor Not Found");
            }
            editor.Rename();
            var posInt = editor.GetPosInt();
            mapTM.SpawnPoint = posInt;
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube((cameraConfinerWorldMax + cameraConfinerWorldMin) / 2, cameraConfinerWorldMax - cameraConfinerWorldMin);
        }

    }

}