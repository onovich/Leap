namespace Leap {

    public class RoleFSMComponent {

        public RoleFSMStatus status;

        public bool idle_isEntering;
        public bool dead_isEntering;

        public RoleFSMComponent() { }

        public void EnterIdle() {
            status = RoleFSMStatus.Idle;
            idle_isEntering = true;
        }

        public void EnterDead() {
            status = RoleFSMStatus.Dead;
            dead_isEntering = true;
        }

    }

}