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
        public float wallJumpDuration;
        public float wallingDuration;
        public float wallJumpAccelerationX;
        public float wallJumpAccelerationY;
        public float g;
        public float fallingSpeedMax;

        public int hp;

        public Sprite mesh;
        public Vector2 meshSize;
        public Vector2 bodySize;
        public Vector2 headSize;
        public Vector2 headOffset;
        public Vector2 footSize;
        public Vector2 footOffset;
        public GameObject deadVFX;
        public float deadVFXDuration;

        public bool hasWallJump;
        public bool hasDoubleJump;
        public bool hasDash;

    }

}