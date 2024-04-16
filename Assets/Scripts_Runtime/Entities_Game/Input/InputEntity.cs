using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Leap {

    public class InputEntity {

        public Vector2 moveAxis;
        public float jumpAxis;
        public bool isHoldWall;

        InputKeybindingComponent keybindingCom;

        public void Ctor() {
            keybindingCom.Ctor();
        }

        public void ProcessInput(Camera camera, float dt) {

            if (keybindingCom.IsKeyPressing(InputKeyEnum.MoveLeft)) {
                moveAxis.x = -1;
            }
            if (keybindingCom.IsKeyPressing(InputKeyEnum.MoveRight)) {
                moveAxis.x = 1;
            }
            if (keybindingCom.IsKeyDown(InputKeyEnum.Jump)) {
                jumpAxis = 1;
            }

        }

        public void Keybinding_Set(InputKeyEnum key, KeyCode[] keyCodes) {
            keybindingCom.Bind(key, keyCodes);
        }

        public void Reset() {
            moveAxis = Vector2.zero;
            jumpAxis = 0f;
            isHoldWall = false; 
        }

    }

}