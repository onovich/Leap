namespace Leap {

    public class IDRecordService {

        int roleEntityID;

        public IDRecordService() { }

        public int PickRoleEntityID() {
            return ++roleEntityID;
        }

        public void Reset() {
            roleEntityID = 0;
        }
    }

}