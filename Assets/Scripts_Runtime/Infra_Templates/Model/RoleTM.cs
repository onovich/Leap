using System;
using UnityEngine;

namespace Leap {

    [CreateAssetMenu(fileName = "SO_Role", menuName = "Leap/RoleTM")]
    public class RoleTM : ScriptableObject {

        [Header(" ** Base Info ** ")]
        public int typeID;
        public AllyStatus allyStatus;
        public AIType aiType;

        [Header(" ** Attr_Base ** ")]
        public int hp;

        [Header(" ** Attr_Move ** ")]
        public float moveSpeed;

        [Header(" ** Attr_Jump ** ")]
        public float jumpForceY;
        public float jumpHangDuration;

        [Header(" ** Attr_Wall ** ")]
        public float wallingDuration;

        [Header(" ** Attr_WallJump ** ")]
        public float wallJumpForceY;
        public float wallJumpForceX;
        public float wallJumpDuration;
        public float wallJumpAccelerationX;
        public float wallJumpAccelerationY;

        [Header(" ** Attr_Dash ** ")]
        public float dashSpeed;
        public float dashAcceleration;
        public float dashDuration;
        public float dashForce;
        public float dashHangDuration;

        [Header(" ** Attr_Land ** ")]
        public float landDuration;
        public float g;
        public float fallingSpeedMax;

        [Header(" ** Render ** ")]
        public Sprite mesh;
        public Sprite mesh_debug;
        public GameObject deadVFX;
        public float deadVFXDuration;

        [Header(" ** Physics ** ")]
        public Vector2 bodyColliderSize;
        public Vector2 headColliderSize;

    }

}