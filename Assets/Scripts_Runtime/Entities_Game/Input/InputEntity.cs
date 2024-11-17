using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Leap {

    public class InputEntity {

        public Vector2 moveAxis;
        public float jumpAxis;
        public float jumpAxis_Temp;
        public float jumpAxis_ResetInterval_Timer;
        public float jumpAxis_ResetInterval_Interval;
        public Vector2 dashAxis;
        public bool isDash;

        InputKeybindingComponent keybindingCom;

        public void Ctor() {
            keybindingCom.Ctor();
            jumpAxis_ResetInterval_Interval = .1f;
            jumpAxis_ResetInterval_Timer = 0f;
        }

        public void ProcessInput(Camera camera, float dt) {

            if (keybindingCom.IsKeyPressing(InputKeyEnum.MoveLeft)) {
                moveAxis.x = -1;
                dashAxis.x = -1;
            }
            if (keybindingCom.IsKeyPressing(InputKeyEnum.MoveRight)) {
                moveAxis.x = 1;
                dashAxis.x = 1;
            }
            if (keybindingCom.IsKeyPressing(InputKeyEnum.Up)) {
                dashAxis.y = 1;
            }
            if (keybindingCom.IsKeyPressing(InputKeyEnum.Down)) {
                dashAxis.y = -1;
            }
            if (keybindingCom.IsKeyDown(InputKeyEnum.Jump)) {
                jumpAxis = 1;
                jumpAxis_Temp = 1;
                ResetJumpAxisTimer();
            }
            if (keybindingCom.IsKeyUp(InputKeyEnum.Dash)) {
                isDash = true;
            }

            if (dashAxis == Vector2.zero) {
                dashAxis = Vector2.up;
            }

            jumpAxis_ResetInterval_Timer += dt;

        }

        public void Keybinding_Set(InputKeyEnum key, KeyCode[] keyCodes) {
            keybindingCom.Bind(key, keyCodes);
        }

        public void Reset() {
            moveAxis = Vector2.zero;
            jumpAxis = 0f;
            isDash = false;
            dashAxis = Vector2.zero;

            if (jumpAxis_ResetInterval_Timer >= jumpAxis_ResetInterval_Interval) {
                jumpAxis_Temp = 0f;
                jumpAxis_ResetInterval_Timer = 0f;
            }
        }

        public void ResetJumpAxisTemp() {
            jumpAxis_Temp = 0f;
            jumpAxis_ResetInterval_Timer = 0f;
        }

        public void ResetJumpAxisTimer() {
            jumpAxis_ResetInterval_Timer = 0f;
        }

    }

}