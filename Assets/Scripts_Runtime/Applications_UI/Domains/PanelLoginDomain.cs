using System;
using UnityEngine;
using UnityEngine.UI;

namespace Leap.UI {

    public static class PanelLoginDomain {

        public static void Open(UIAppContext ctx) {

            Panel_Login panel = ctx.uiCore.UniquePanel_Open<Panel_Login>();
            panel.Ctor();

            panel.OnClickStartGameHandle += () => {
                ctx.evt.Login_OnStartGameClick();
            };

            panel.OnClickExitGameHandle += () => {
                ctx.evt.Login_OnExitGameClick();
            };

        }

        public static void Close(UIAppContext ctx) {
            ctx.uiCore.UniquePanel_Close<Panel_Login>();
        }

    }

}