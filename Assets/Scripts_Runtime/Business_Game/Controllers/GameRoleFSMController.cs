using UnityEngine;

namespace Leap {

    public static class GameRoleFSMController {

        public static void FixedTickFSM(GameBusinessContext ctx, RoleEntity role, float fixdt) {

            FixedTickFSM_Any(ctx, role, fixdt);

            RoleFSMStatus status = role.FSM_GetStatus();
            if (status == RoleFSMStatus.Idle) {
                FixedTickFSM_Idle(ctx, role, fixdt);
            } else if (status == RoleFSMStatus.Dead) {
                FixedTickFSM_Dead(ctx, role, fixdt);
            } else {
                GLog.LogError($"GameRoleFSMController.FixedTickFSM: unknown status: {status}");
            }

        }

        static void FixedTickFSM_Any(GameBusinessContext ctx, RoleEntity role, float fixdt) {

        }

        static void FixedTickFSM_Idle(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            if (fsm.idle_isEntering) {
                fsm.idle_isEntering = false;
            }

            // Move
            GameRoleDomain.ApplyMove(ctx, role, fixdt);
        }

        static void FixedTickFSM_Dead(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            if (fsm.dead_isEntering) {
                fsm.dead_isEntering = false;
            }
        }

    }

}