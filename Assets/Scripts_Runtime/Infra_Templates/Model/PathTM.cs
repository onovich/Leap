using System;
using MortiseFrame.Swing;
using UnityEngine;

namespace Leap {

    [CreateAssetMenu(fileName = "SO_Path", menuName = "Leap/PathTM")]
    public class PathTM : ScriptableObject {

        public int typeID;
        public EasingType easingType;
        public EasingMode easingMode;
        public float movingDuration;
        public Color lineColor;
        public Material lineMaterial;
        public float lineWidth;

    }

}