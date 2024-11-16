using System;
using System.Collections.Generic;
using TriInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Leap.Modifier {

    public class MapEditorEntity : MonoBehaviour {

        [SerializeField] int typeID;
        [SerializeField] GameObject constraint;
        [SerializeField] MapTM mapTM;
        [SerializeField] Tilemap tilemap_terrain;
        [SerializeField] TileBase tilebase_terrain;
        [SerializeField] Transform blockGroup;
        [SerializeField] Transform spikeGroup;
        [SerializeField] Transform spawnPointGroup;
        [SerializeField] Transform pathGroup;
        [SerializeField] Vector2 cameraConfinerWorldMax;
        [SerializeField] Vector2 cameraConfinerWorldMin;

        IndexService indexService;
        List<TerrainTM> terrainTMArr;

        [Button("Bake")]
        void Bake() {
            GetAllTerrainTM();

            indexService = new IndexService();
            indexService.ResetIndex();
            BakeTerrain();
            BakeMapInfo();
            BakeBlock();
            BakeSpike();
            BakePath();
            BakeSpawnPoint();

            AddressableHelper.SetAddressable(mapTM, "TM_Map", "TM_Map", true);
            EditorUtility.SetDirty(mapTM);
            AssetDatabase.SaveAssets();
            Debug.Log("Bake Sucess");
        }

        void BakeMapInfo() {
            mapTM.typeID = typeID;
            mapTM.tileBase_terrain = tilebase_terrain;
            mapTM.constraintSize = constraint.transform.lossyScale;
            mapTM.constraintCenter = constraint.transform.position;
            mapTM.mapPos = transform.position.RoundToVector2Int();
            this.transform.position = mapTM.mapPos.ToVector3Int();
        }

        void BakeTerrain() {
            var terrainSpawnPosList = new List<Vector2Int>();
            var terrainTypeIDList = new List<int>();
            for (int x = tilemap_terrain.cellBounds.x; x < tilemap_terrain.cellBounds.xMax; x++) {
                for (int y = tilemap_terrain.cellBounds.y; y < tilemap_terrain.cellBounds.yMax; y++) {
                    var pos = new Vector3Int(x, y, 0);
                    var tile = tilemap_terrain.GetTile(pos);
                    if (tile == null) continue;
                    terrainSpawnPosList.Add(pos.ToVector2Int());
                    var terrainTM = GetTerrainTM(tile);
                    if (terrainTM == null) {
                        Debug.Log("TerrainTM Not Found");
                        continue;
                    }
                    terrainTypeIDList.Add(terrainTM.typeID);
                }
            }
            mapTM.terrainSpawnPosArr = terrainSpawnPosList.ToArray();
            mapTM.terrainTypeIDArr = terrainTypeIDList.ToArray();
        }

        void BakeSpike() {
            List<SpikeTM> spikeTMList = new List<SpikeTM>();
            List<Vector2Int> spikeSpawnPosList = new List<Vector2Int>();
            List<Vector2Int> spikeSpawnSizeList = new List<Vector2Int>();
            List<int> spikeSpawnRotationZList = new List<int>();
            List<int> spikeIndexList = new List<int>();
            var spikeEditors = spikeGroup.GetComponentsInChildren<SpikeEditorEntity>();
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

         void BakePath() {
            if (pathGroup == null) {
                return;
            }
            var editors = pathGroup.GetComponentsInChildren<PathEditorEntity>();
            if (editors == null || editors.Length == 0) {
                mapTM.pathSpawnTMArr = null;
                mapTM.pathTravelerTypeArr = null;
                mapTM.pathTravelerIndexArr = null;
                mapTM.pathIndexArr = null;
                mapTM.pathTMArr = null;
                mapTM.pathIsCircleLoopArr = null;
                mapTM.pathIsPingPongLoopArr = null;
                mapTM.pathTravelerHalfSizeArr = null;
                return;
            }
            var pathTMArr = new List<PathTM>();
            var pathSpawnTMArr = new List<PathSpawnTM>();
            var pathTravelerTypeArr = new List<EntityType>();
            var pathTravelerIndexArr = new List<int>();
            var pathIndexArr = new List<int>();
            var pathIsCircleLoopArr = new List<bool>();
            var pathIsPingPongLoopArr = new List<bool>();
            var pathTravelerHalfSizeArr = new List<Vector2>();
            var index = 0;
            foreach (var editor in editors) {
                index++;
                var pathSpawnTM = new PathSpawnTM();
                pathSpawnTM.pathNodeArr = editor.GetPathNodeArr();
                pathSpawnTMArr.Add(pathSpawnTM);
                editor.Rename(index);
                var travelerType = editor.GetTravelerType();
                var travlerIndex = editor.GetTravlerIndex();
                pathTravelerTypeArr.Add(travelerType);
                pathTravelerIndexArr.Add(travlerIndex);
                pathIndexArr.Add(index);
                pathTMArr.Add(editor.pathTM);
                pathIsCircleLoopArr.Add(editor.isCircleLoop);
                pathIsPingPongLoopArr.Add(editor.isPingPongLoop);
                pathTravelerHalfSizeArr.Add(editor.GetTravelerSize(editor.traveler) / 2);
            }
            mapTM.pathSpawnTMArr = pathSpawnTMArr.ToArray();
            mapTM.pathTravelerTypeArr = pathTravelerTypeArr.ToArray();
            mapTM.pathTravelerIndexArr = pathTravelerIndexArr.ToArray();
            mapTM.pathIndexArr = pathIndexArr.ToArray();
            mapTM.pathTMArr = pathTMArr.ToArray();
            mapTM.pathIsCircleLoopArr = pathIsCircleLoopArr.ToArray();
            mapTM.pathIsPingPongLoopArr = pathIsPingPongLoopArr.ToArray();
            mapTM.pathTravelerHalfSizeArr = pathTravelerHalfSizeArr.ToArray();
        }

        void BakeBlock() {
            List<BlockTM> blockTMList = new List<BlockTM>();
            List<Vector2Int> blockSpawnPosList = new List<Vector2Int>();
            List<Vector2Int> blockSpawnMeshSizeList = new List<Vector2Int>();
            List<Vector2> blockSpawnMeshOffsetList = new List<Vector2>();
            List<int> blockIndexList = new List<int>();
            var blockEditors = blockGroup.GetComponentsInChildren<BlockEditorEntity>();
            if (blockEditors == null) {
                Debug.Log("BlockEditors Not Found");
            }
            for (int i = 0; i < blockEditors.Length; i++) {
                var editor = blockEditors[i];
                editor.AdjustMeshSize();

                var tm = editor.blockTM;
                blockTMList.Add(tm);

                var posInt = editor.GetPosInt();
                blockSpawnPosList.Add(posInt);

                var meshSizeInt = editor.GetMeshSize();
                blockSpawnMeshSizeList.Add(meshSizeInt);

                var meshOffset = editor.GetMeshOffset();
                blockSpawnMeshOffsetList.Add(meshOffset);

                var index = indexService.PickBlockIndex();
                blockIndexList.Add(index);
                editor.index = index;

                editor.Rename();
            }
            mapTM.blockSpawnArr = blockTMList.ToArray();
            mapTM.blockSpawnPosArr = blockSpawnPosList.ToArray();
            mapTM.blockSpawnSizeArr = blockSpawnMeshSizeList.ToArray();
            mapTM.blockSpawnMeshOffsetArr = blockSpawnMeshOffsetList.ToArray();

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

        TerrainTM GetTerrainTM(TileBase tileBase) {
            foreach (var terrainTM in terrainTMArr) {
                if (terrainTM.tile == tileBase) {
                    return terrainTM;
                }
            }
            return null;
        }

        void GetAllTerrainTM() {
            terrainTMArr = FieldHelper.GetAllInstances<TerrainTM>();
        }

    }

}