namespace Leap {

    public class GameFSMComponent {

        public GameStatus status;

        public bool notInGame_isEntering;
        public bool gaming_isEntering;
        public bool gameOver_isEntering;
        public float gameOver_enterTime;

        public void NotInGame_Enter() {
            status = GameStatus.NotInGame;
            notInGame_isEntering = true;
        }

        public void Gaming_Enter() {
            status = GameStatus.Gaming;
            gaming_isEntering = true;
        }

        public void GameOver_Enter(float enterTime) {
            status = GameStatus.GameOver;
            gameOver_isEntering = true;
            gameOver_enterTime = enterTime;
        }

    }

}