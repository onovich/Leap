using UnityEngine;

namespace Leap {

    public class RoleFSMComponent {

        public RoleFSMStatus status;

        public bool landing_isEntering;
        public float landing_duration;
        public float landing_timer;

        public bool airing_isEntering;

        public bool jumping_isEntering;

        public bool walling_isEntering;
        public Vector2 walling_dir;
        public float walling_duration;
        public float walling_timer;

        public bool wallJumping_isEntering;
        public Vector2 wallJumping_jumpingDir;
        public float wallJumping_duration;

        public bool dash_isEntering;
        public Vector2 dash_dir;
        public float dash_duration;

        public bool dying_isEntering;

        public RoleFSMComponent() { }

        public void EnterLanding(float duration) {
            Reset();
            status = RoleFSMStatus.Landing;
            landing_isEntering = true;
            landing_duration = duration;
            landing_timer = 0;
        }

        public bool LandingTimerIsEnd(float dt) {
            landing_timer += dt;
            return landing_timer >= landing_duration;
        }

        public void ResetLandingTimer() {
            landing_timer = 0;
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

        public void EnterWalling(Vector2 wallDir, float duration) {
            Reset();
            status = RoleFSMStatus.Walling;
            walling_isEntering = true;
            walling_dir = wallDir;
            walling_duration = duration;
            ResetWallingTimer();
        }

        public bool WallingTimerIsEnd(float dt) {
            walling_timer += dt;
            return walling_timer >= walling_duration;
        }

        public void ResetWallingTimer() {
            walling_timer = 0;
        }

        public void EnterWallJumping(Vector2 jumpingDir, float duration) {
            Reset();
            status = RoleFSMStatus.WallJumping;
            wallJumping_isEntering = true;
            wallJumping_jumpingDir = jumpingDir;
            wallJumping_duration = duration;
        }

        public void EnterDash(Vector2 dashDir, float duration) {
            Reset();
            status = RoleFSMStatus.Dash;
            dash_isEntering = true;
            dash_dir = dashDir;
            dash_duration = duration;
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
            wallJumping_isEntering = false;
            dash_isEntering = false;

            dash_dir = Vector2.zero;
            walling_dir = Vector2.zero;
            wallJumping_jumpingDir = Vector2.zero;

            landing_timer = 0;
            walling_timer = 0;
        }

    }

}