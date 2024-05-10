namespace Leap {

    public class RoleFSMComponent {

        public RoleFSMStatus status;

        public bool normal_isEntering;
        public bool wallJumping_isEntering;
        public bool dead_isEntering;

        public RoleFSMComponent() { }

        public void EnterNormal() {
            status = RoleFSMStatus.Normal;
            normal_isEntering = true;
        }

        public void EnterWallJumping() {
            status = RoleFSMStatus.WallJumping;
            wallJumping_isEntering = true;
        }

        public void EnterDead() {
            status = RoleFSMStatus.Dead;
            dead_isEntering = true;
        }

    }

}