using UnityEngine;

namespace Leap {

    public static class CameraExtension {

        public static Vector3 WorldToGUIScreenPoint(this Camera camera, Vector3 worldPos) {
            var screenPos = camera.WorldToScreenPoint(worldPos);
            screenPos.y = Screen.height - screenPos.y;
            return screenPos;
        }

    }
}