namespace Leap {

    public class RoleFSMComponent {

        public RoleFSMStatus status;

        public bool normal_isEntering;
        public bool wallJumping_isEntering;
        public bool dead_isEntering;

        public RoleFSMComponent() { }

        public void EnterNormal() {
            Reset();
            status = RoleFSMStatus.Normal;
            normal_isEntering = true;
        }

        public void EnterWallJumping() {
            Reset();
            status = RoleFSMStatus.WallJumping;
            wallJumping_isEntering = true;
        }

        public void EnterDead() {
            Reset();
            status = RoleFSMStatus.Dead;
            dead_isEntering = true;
        }

        public void Reset() {
            normal_isEntering = false;
            wallJumping_isEntering = false;
            dead_isEntering = false;
        }

    }

}