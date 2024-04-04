namespace Leap {

    public class IDRecordService {

        int roleEntityID;
        int blockEntityID;

        public IDRecordService() { }

        public int PickRoleEntityID() {
            return ++roleEntityID;
        }

        public int PickBlockEntityID() {
            return ++blockEntityID;
        }

        public void Reset() {
            roleEntityID = 0;
            blockEntityID = 0;
        }
    }

}