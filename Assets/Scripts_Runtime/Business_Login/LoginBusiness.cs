using UnityEngine;

namespace Leap{

    public class LoginBusiness {

        public static void Enter(LoginBusinessContext ctx) {
            UIApp.Login_Open(ctx.uiContext);
        }

        public static void Tick(LoginBusinessContext ctx, float dt) {
        }

        public static void Exit(LoginBusinessContext ctx) {
            UIApp.Login_Close(ctx.uiContext);
        }

        public static void ExitApplication(LoginBusinessContext ctx) {
            Exit(ctx);
            Application.Quit();
            GLog.Log("Application.Quit");
        }

        public static void OnUILoginClick(LoginBusinessContext ctx) {
            ctx.evt.Login();
        }

    }

}