using UnityEngine;

namespace Leap {

    public class RoleFSMComponent {

        public RoleFSMStatus status;

        public bool walking_isEntering;
        public bool jumping_isEntering;
        public bool wallJumping_isEntering;
        public Vector2 wallJumping_jumpingDir;
        public float wallJumping_timer;
        public bool dead_isEntering;

        public RoleFSMComponent() { }

        public void EnterWalking() {
            Reset();
            status = RoleFSMStatus.Walking;
            walking_isEntering = true;
        }

        public void EnterWallJumping(Vector2 wallJumpDir, float wallJumpDuration) {
            Reset();
            status = RoleFSMStatus.WallJumping;
            wallJumping_isEntering = true;
            wallJumping_jumpingDir = wallJumpDir;
            wallJumping_timer = wallJumpDuration;
        }

        public void EnterJumping() {
            Reset();
            status = RoleFSMStatus.Jumping;
            jumping_isEntering = true;
        }

        public void EnterDying() {
            Reset();
            status = RoleFSMStatus.Dying;
            dead_isEntering = true;
        }

        public void Reset() {
            walking_isEntering = false;
            wallJumping_isEntering = false;
            dead_isEntering = false;
        }

    }

}