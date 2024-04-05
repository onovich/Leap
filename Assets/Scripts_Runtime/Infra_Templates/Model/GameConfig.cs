using System;
using UnityEngine;

namespace Leap {

    [CreateAssetMenu(fileName = "SO_GameConfig", menuName = "Leap/GameConfig")]
    public class GameConfig : ScriptableObject {

        public int ownerRoleTypeID;
        public int originalMapTypeID;

    }

}