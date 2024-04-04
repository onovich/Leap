namespace Leap {

    public enum AllyStatus {

        None = 0,
        Friend = 1,
        Enemy = 2,

    }

    public static class AllyStatusExtension {

        public static AllyStatus GetOpposite(this AllyStatus status) {
            if (status == AllyStatus.Friend) {
                return AllyStatus.Enemy;
            } else if (status == AllyStatus.Enemy) {
                return AllyStatus.Friend;
            } else {
                return AllyStatus.None;
            }
        }
    }

}