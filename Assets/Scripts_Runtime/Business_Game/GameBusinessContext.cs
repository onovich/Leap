using System;
using System.Collections.Generic;
using UnityEngine;

namespace Leap {

    public class GameBusinessContext {

        // Entity
        public GameEntity gameEntity;
        public PlayerEntity playerEntity;
        public InputEntity inputEntity; // External

        public RoleRepository roleRepo;

        // UI
        public UIAppContext uiContext;

        // Camera
        public Camera mainCamera;

        // Service
        public IDRecordService idRecordService;

        // Infra
        public TemplateInfraContext templateInfraContext;
        public AssetsInfraContext assetsInfraContext;

        public GameBusinessContext() {
            gameEntity = new GameEntity();
            playerEntity = new PlayerEntity();
            idRecordService = new IDRecordService();
            roleRepo = new RoleRepository();
        }

        public void Reset() {
            roleRepo.Clear();
        }

        // Role
        public RoleEntity Role_GetOwner() {
            roleRepo.TryGetRole(playerEntity.ownerRoleEntityID, out var role);
            return role;
        }

        public void Role_UpdatePosDict(RoleEntity role) {
            roleRepo.UpdatePosDict(role);
        }

        public void Role_ForEach(Action<RoleEntity> onAction) {
            roleRepo.ForEach(onAction);
        }

    }

}