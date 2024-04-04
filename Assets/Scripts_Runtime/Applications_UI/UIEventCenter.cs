using System;

namespace Leap.UI {

    public class UIEventCenter {

        // Login
        public Action Login_OnStartGameClickHandle;
        public void Login_OnStartGameClick() {
            Login_OnStartGameClickHandle?.Invoke();
        }

        public Action Login_OnExitGameClickHandle;
        public void Login_OnExitGameClick() {
            Login_OnExitGameClickHandle?.Invoke();
        }

        // Inventory
        public Action<int> Inventory_OnLeftClickTreasureHandle;
        public void Inventory_OnLeftClickTreasure(int index) {
            Inventory_OnLeftClickTreasureHandle?.Invoke(index);
        }

        public Action<int> Inventory_OnRightClickTreasureHandle;
        public void Inventory_OnRightClickTreasure(int index) {
            Inventory_OnRightClickTreasureHandle?.Invoke(index);
        }

        public Action Inventory_OnClickCloseHandle;
        public void Inventory_OnClickClose() {
            Inventory_OnClickCloseHandle?.Invoke();
        }

        // BluePrint
        public Action<int> BluePrint_OnClickChooseBluePrintHandle;
        public void BluePrint_OnClickChooseBluePrint(int index) {
            BluePrint_OnClickChooseBluePrintHandle?.Invoke(index);
        }

        public void Clear() {
            Login_OnStartGameClickHandle = null;
            Login_OnExitGameClickHandle = null;

            Inventory_OnLeftClickTreasureHandle = null;
            Inventory_OnRightClickTreasureHandle = null;
            Inventory_OnClickCloseHandle = null;

            BluePrint_OnClickChooseBluePrintHandle = null;
        }


    }

}