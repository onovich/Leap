using System;
using System.Collections.Generic;
using UnityEngine;

namespace Leap {

    public class GameBusinessContext {

        // Entity
        public GameEntity gameEntity;
        public PlayerEntity playerEntity;
        public InputEntity inputEntity; // External
        public MapEntity currentMapEntity;

        public RoleRepository roleRepo;
        public BlockRepository blockRepo;
        public SpikeRepository spikeRepo;

        // App
        public UIAppContext uiContext;
        public VFXAppContext vfxContext;
        public CameraAppContext cameraContext;

        // Camera
        public Camera mainCamera;

        // Service
        public IDRecordService idRecordService;

        // Infra
        public TemplateInfraContext templateInfraContext;
        public AssetsInfraContext assetsInfraContext;

        // Timer
        public float fixedRestSec;

        // SpawnPoint
        public Vector2 ownerSpawnPoint;

        public GameBusinessContext() {
            gameEntity = new GameEntity();
            playerEntity = new PlayerEntity();
            idRecordService = new IDRecordService();
            roleRepo = new RoleRepository();
            blockRepo = new BlockRepository();
            spikeRepo = new SpikeRepository();
        }

        public void Reset() {
            idRecordService.Reset();
            roleRepo.Clear();
            blockRepo.Clear();
            spikeRepo.Clear();
        }

        // Role
        public RoleEntity Role_GetOwner() {
            roleRepo.TryGetRole(playerEntity.ownerRoleEntityID, out var role);
            return role;
        }

        public void Role_ForEach(Action<RoleEntity> onAction) {
            roleRepo.ForEach(onAction);
        }

        // Block
        public void Block_ForEach(Action<BlockEntity> onAction) {
            blockRepo.ForEach(onAction);
        }

    }

}