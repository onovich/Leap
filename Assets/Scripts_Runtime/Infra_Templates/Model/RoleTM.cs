using System;
using UnityEngine;

namespace Leap {

    [CreateAssetMenu(fileName = "SO_Role", menuName = "Leap/RoleTM")]
    public class RoleTM : ScriptableObject {

        public int typeID;
        public AllyStatus allyStatus;
        public AIType aiType;

        public float moveSpeed;
        public float jumpForce;
        public float g;
        public float fallingSpeedMax;

        public int hp;

        public Sprite mesh;
        public GameObject deadVFX;
        public float deadVFXDuration;
    }

}