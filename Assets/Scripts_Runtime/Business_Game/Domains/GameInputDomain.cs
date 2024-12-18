namespace Leap {

    public static class GameInputDomain {

        public static void Player_BakeInput(GameBusinessContext ctx, float dt) {
            InputEntity inputEntity = ctx.inputEntity;
            inputEntity.ProcessInput(ctx.mainCamera, dt);
        }

        public static void Owner_BakeInput(GameBusinessContext ctx, RoleEntity owner) {
            InputEntity inputEntity = ctx.inputEntity;
            ref RoleInputComponent inputCom = ref owner.inputCom;
            inputCom.moveAxis = inputEntity.moveAxis;
            inputCom.jumpAxis = inputEntity.jumpAxis;
            inputCom.isDash = inputEntity.isDash;
            inputCom.dashAxis = inputEntity.dashAxis;
            inputCom.jumpAxis_Temp = inputEntity.jumpAxis_Temp;
        }

    }

}