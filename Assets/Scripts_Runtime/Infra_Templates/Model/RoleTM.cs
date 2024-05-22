using System;
using UnityEngine;

namespace Leap {

    [CreateAssetMenu(fileName = "SO_Role", menuName = "Leap/RoleTM")]
    public class RoleTM : ScriptableObject {

        public int typeID;
        public AllyStatus allyStatus;
        public AIType aiType;

        public float moveSpeed;
        public float jumpForceY;
        public float wallJumpForceY;
        public float wallJumpForceX;
        public float g;
        public float fallingSpeedMax;

        public int hp;

        public Sprite mesh;
        public Vector2 size;
        public GameObject deadVFX;
        public float deadVFXDuration;

        public bool hasWallJump;
        public bool hasDoubleJump;
        public bool hasDash;

    }

}