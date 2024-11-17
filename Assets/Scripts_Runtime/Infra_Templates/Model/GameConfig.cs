using System;
using MortiseFrame.Swing;
using UnityEngine;

namespace Leap {

    [CreateAssetMenu(fileName = "SO_GameConfig", menuName = "Leap/GameConfig")]
    public class GameConfig : ScriptableObject {

        // Game
        [Header("Game Config")]
        public float gameResetEnterTime;

        // Role
        [Header("Role Config")]
        public int ownerRoleTypeID;
        public int originalMapTypeID;

        // Friction
        public float roleTerrainFriction;
        public float roleBlockFriction;

        // Camera
        [Header("DeadZone Config")]
        public Vector2 cameraDeadZoneNormalizedSize;

        [Header("Shake Config - Role Dead")]
        public float roleDeadShakeFrequency;
        public float roleDeadShakeAmplitude;
        public float roleDeadShakeDuration;
        public EasingType roleDeadShakeEasingType;
        public EasingMode roleDeadShakeEasingMode;

        [Header("Shake Config - Role Dash")]
        public float roleDashShakeFrequency;
        public float roleDashShakeAmplitude;
        public float roleDashShakeDuration;
        public EasingType roleDashShakeEasingType;
        public EasingMode roleDashShakeEasingMode;

    }

}