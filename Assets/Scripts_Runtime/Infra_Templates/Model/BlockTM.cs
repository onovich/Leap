using System;
using UnityEngine;

namespace Leap {

    [CreateAssetMenu(fileName = "SO_Block", menuName = "Leap/BlockTM")]
    public class BlockTM : ScriptableObject {

        public int typeID;
        public Sprite mesh;

    }

}