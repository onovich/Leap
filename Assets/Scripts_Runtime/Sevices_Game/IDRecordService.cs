namespace Leap {

    public class IDRecordService {

        int roleEntityID;
        int blockEntityID;
        int spikeEntityID;

        public IDRecordService() { }

        public int PickRoleEntityID() {
            return ++roleEntityID;
        }

        public int PickBlockEntityID() {
            return ++blockEntityID;
        }

        public int PickSpikeEntityID() {
            return ++spikeEntityID;
        }

        public void Reset() {
            roleEntityID = 0;
            blockEntityID = 0;
            spikeEntityID = 0;
        }
    }

}