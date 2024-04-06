using System;
using System.Threading.Tasks;
using MortiseFrame.Swing;
using TenonKit.Prism;
using TenonKit.Vista.Camera2D;
using UnityEngine;

namespace Leap {

    public static class CameraApp {

        public static void Init(CameraAppContext ctx, Transform driver, Vector2 pos, Vector2 confinerWorldMax, Vector2 confinerWorldMin) {
            var cameraID = CreateMainCamera(ctx, pos, confinerWorldMax, confinerWorldMin);
            SetCurrentCamera(ctx, cameraID);
            var config = ctx.templateInfraContext.Config_Get();
            var deadZoneNormalizedSize = config.cameraDeadZoneNormalizedSize;

            SetDeadZone(ctx, deadZoneNormalizedSize);
            EnableDeadZone(ctx, true);
            SetMoveByDriver(ctx, driver);
        }

        public static void LateTick(CameraAppContext ctx, float dt) {
            ctx.cameraCore.Tick(dt);
        }

        public static void ShakeOnce(CameraAppContext ctx, int cameraID) {
            var config = ctx.templateInfraContext.Config_Get();
            var shakeFrequency = config.roleDeadShakeFrequency;
            var shakeAmplitude = config.roleDeadShakeAmplitude;
            var shakeDuration = config.roleDeadShakeDuration;
            var shakeEasingType = config.roleDeadShakeEasingType;
            var shakeEasingMode = config.roleDeadShakeEasingMode;
            ctx.cameraCore.ShakeOnce(cameraID, shakeFrequency, shakeAmplitude, shakeDuration, shakeEasingType, shakeEasingMode);
        }

        // Camera
        public static int CreateMainCamera(CameraAppContext ctx, Vector2 pos, Vector2 confinerWorldMax, Vector2 confinerWorldMin) {
            ctx.mainCameraID = ctx.cameraCore.CreateCamera2D(pos, confinerWorldMax, confinerWorldMin);
            return ctx.mainCameraID;
        }

        public static void SetCurrentCamera(CameraAppContext ctx, int cameraID) {
            ctx.cameraCore.SetCurrentCamera(cameraID);
        }

        // Move
        public static void SetMoveToTarget(CameraAppContext ctx, Vector2 target, float duration, EasingType easingType = EasingType.Linear, EasingMode easingMode = EasingMode.None, System.Action onComplete = null) {
            var mainCameraID = ctx.mainCameraID;
            ctx.cameraCore.SetMoveToTarget(mainCameraID, target, duration, easingType, easingMode, onComplete);
        }

        public static void SetMoveByDriver(CameraAppContext ctx, Transform driver) {
            var mainCameraID = ctx.mainCameraID;
            ctx.cameraCore.SetMoveByDriver(mainCameraID, driver);
        }

        // DeadZone
        public static void SetDeadZone(CameraAppContext ctx, Vector2 normalizedSize) {
            var mainCameraID = ctx.mainCameraID;
            ctx.cameraCore.SetDeadZone(mainCameraID, normalizedSize, Vector2.zero);
        }

        public static void EnableDeadZone(CameraAppContext ctx, bool enable) {
            var mainCameraID = ctx.mainCameraID;
            ctx.cameraCore.EnableDeadZone(mainCameraID, enable);
        }

        // Gizmos
        public static void OnDrawGizmos(CameraAppContext ctx) {
            ctx.cameraCore.DrawGizmos();
        }

    }

}