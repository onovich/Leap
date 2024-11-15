using UnityEngine;

namespace Leap {

    public class RoleFSMComponent {

        public RoleFSMStatus status;

        public bool landing_isEntering;

        public bool airing_isEntering;

        public bool jumping_isEntering;

        public bool walling_isEntering;
        public Vector2 walling_dir;
        public float walling_duration;
        public float walling_friction;

        public bool wallJumping_isEntering;
        public Vector2 wallJumping_jumpingDir;
        public float wallJumping_duration;

        public bool dying_isEntering;

        public RoleFSMComponent() { }

        public void EnterLanding() {
            Reset();
            status = RoleFSMStatus.Landing;
            landing_isEntering = true;
        }

        public void EnterAiring() {
            Reset();
            status = RoleFSMStatus.Airing;
            jumping_isEntering = true;
        }

        public void EnterJumping() {
            Reset();
            status = RoleFSMStatus.Jumping;
            jumping_isEntering = true;
        }

        public void EnterWalling(Vector2 wallDir, float friction, float duration) {
            Reset();
            status = RoleFSMStatus.Walling;
            walling_isEntering = true;
            walling_dir = wallDir;
            walling_duration = duration;
            walling_friction = friction;
        }

        public void EnterWallJumping(Vector2 jumpingDir, float duration) {
            Reset();
            status = RoleFSMStatus.WallJumping;
            wallJumping_isEntering = true;
            wallJumping_jumpingDir = jumpingDir;
            wallJumping_duration = duration;
        }

        public void EnterDying() {
            Reset();
            status = RoleFSMStatus.Dying;
            dying_isEntering = true;
        }

        public void Reset() {
            landing_isEntering = false;
            jumping_isEntering = false;
            airing_isEntering = false;
            walling_isEntering = false;
            dying_isEntering = false;
        }

    }

}