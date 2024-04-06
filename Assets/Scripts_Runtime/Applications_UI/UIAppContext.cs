using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MortiseFrame.Loom;
using UnityEngine;
using Leap.UI;

namespace Leap {

    public class UIAppContext {

        // Event
        public UIEventCenter evt;

        // Core
        public UICore uiCore;

        // Infra
        public TemplateInfraContext templateInfraContext;

        public UIAppContext(string label, Canvas mainCanvas, Transform worldSpaceFakeCanvas = null, Camera worldSpaceCamera = null) {
            uiCore = new UICore(label, mainCanvas, worldSpaceFakeCanvas, worldSpaceCamera);
            evt = new UIEventCenter();
        }

        #region Unique Panel
        public T UniquePanel_Open<T>(bool isWorldSpace = false) where T : IPanel {
            return uiCore.UniquePanel_Open<T>(isWorldSpace);
        }

        public T UniquePanel_Get<T>() where T : IPanel {
            return uiCore.UniquePanel_Get<T>();
        }

        public bool UniquePanel_TryGet<T>(out T panel) where T : IPanel {
            return uiCore.UniquePanel_TryGet<T>(out panel);
        }

        public void UniquePanel_Close<T>() where T : IPanel {
            uiCore.UniquePanel_Close<T>();
        }
        #endregion

        #region  Multiple Panel
        public T MultiplePanel_Open<T>(bool isWorldSpace) where T : IPanel {
            var panel = uiCore.MultiplePanel_Open<T>(isWorldSpace);
            return panel;
        }

        public void MultiplePanel_Close<T>(T panel) where T : IPanel {
            uiCore.MultiplePanel_Close<T>(panel);
        }

        public void MultiplePanel_GroupForEach<T>(Action<T> action) where T : IPanel {
            uiCore.MultiplePanel_GroupForEach<T>(action);
        }

        public void MultiplePanel_CloseGroup<T>() where T : IPanel {
            uiCore.MultiplePanel_CloseGroup<T>();
        }
        #endregion

    }

}