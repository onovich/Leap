namespace Leap {

    public class GameEntity {

        bool isDebugMode;
        public bool IsDebugMode => isDebugMode;

        public GameFSMComponent fsmComponent;

        public GameEntity(bool isDebugMode) {
            fsmComponent = new GameFSMComponent();
            this.isDebugMode = isDebugMode;
        }

    }

}