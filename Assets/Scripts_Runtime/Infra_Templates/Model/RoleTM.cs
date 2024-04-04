using System;
using UnityEngine;

namespace Leap {

    [CreateAssetMenu(fileName = "SO_Role", menuName = "Leap/RoleTM")]
    public class RoleTM : ScriptableObject {

        public int typeID;
        public AllyStatus allyStatus;
        public AIType aiType;

        public float moveSpeed;

        public Sprite mesh;
    }

}