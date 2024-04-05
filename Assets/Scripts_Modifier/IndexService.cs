namespace Leap.Modifier {

    public class IndexService {

        int blockIndexRecord;
        int spikeIndexRecord;
        int trackIndexRecord;

        public void ResetIndex() {
            blockIndexRecord = 0;
            spikeIndexRecord = 0;
            trackIndexRecord = 0;
        }

        public int PickBlockIndex() {
            return ++blockIndexRecord;
        }

        public int PickSpikeIndex() {
            return ++spikeIndexRecord;
        }

        public int PickTrackIndex() {
            return ++trackIndexRecord;
        }

    }

}