using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

namespace Leap {

    public class Main : MonoBehaviour {

        [SerializeField] bool drawCameraGizmos;

        InputEntity inputEntity;

        AssetsInfraContext assetsInfraContext;
        TemplateInfraContext templateInfraContext;

        LoginBusinessContext loginBusinessContext;
        GameBusinessContext gameBusinessContext;

        UIAppContext uiAppContext;
        VFXAppContext vfxAppContext;
        CameraAppContext cameraAppContext;

        bool isLoadedAssets;
        bool isTearDown;

        void Start() {

            isLoadedAssets = false;
            isTearDown = false;

            Canvas mainCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
            Transform hudFakeCanvas = GameObject.Find("HUDFakeCanvas").transform;
            Camera mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
            Transform vfxRoot = GameObject.Find("VFXRoot").transform;

            inputEntity = new InputEntity();

            loginBusinessContext = new LoginBusinessContext();
            gameBusinessContext = new GameBusinessContext();

            uiAppContext = new UIAppContext("UI", mainCanvas, hudFakeCanvas, mainCamera);
            vfxAppContext = new VFXAppContext("VFX", vfxRoot);
            cameraAppContext = new CameraAppContext(mainCamera, new Vector2(Screen.width, Screen.height));

            assetsInfraContext = new AssetsInfraContext();
            templateInfraContext = new TemplateInfraContext();

            // Inject
            loginBusinessContext.uiContext = uiAppContext;

            gameBusinessContext.inputEntity = inputEntity;
            gameBusinessContext.assetsInfraContext = assetsInfraContext;
            gameBusinessContext.templateInfraContext = templateInfraContext;
            gameBusinessContext.uiContext = uiAppContext;
            gameBusinessContext.vfxContext = vfxAppContext;
            gameBusinessContext.cameraContext = cameraAppContext;
            gameBusinessContext.mainCamera = mainCamera;

            cameraAppContext.templateInfraContext = templateInfraContext;

            // TODO Camera

            // Binding
            Binding();

            Action action = async () => {
                try {
                    await LoadAssets();
                    Init();
                    Enter();
                    isLoadedAssets = true;
                } catch (Exception e) {
                    GLog.LogError(e.ToString());
                }
            };
            action.Invoke();

        }

        void Enter() {
            LoginBusiness.Enter(loginBusinessContext);
        }

        void Update() {

            if (!isLoadedAssets) {
                return;
            }

            var dt = Time.deltaTime;
            LoginBusiness.Tick(loginBusinessContext, dt);
            GameBusiness.Tick(gameBusinessContext, dt);

            UIApp.LateTick(uiAppContext, dt);

        }

        void Init() {

            Application.targetFrameRate = 120;

            var inputEntity = this.inputEntity;
            inputEntity.Ctor();
            inputEntity.Keybinding_Set(InputKeyEnum.MoveLeft, new KeyCode[] { KeyCode.A, KeyCode.LeftArrow });
            inputEntity.Keybinding_Set(InputKeyEnum.MoveRight, new KeyCode[] { KeyCode.D, KeyCode.RightArrow });
            inputEntity.Keybinding_Set(InputKeyEnum.Jump, new KeyCode[] { KeyCode.Space });

            GameBusiness.Init(gameBusinessContext);

            UIApp.Init(uiAppContext);
            VFXApp.Init(vfxAppContext);

        }

        void Binding() {
            var uiEvt = uiAppContext.evt;

            // UI
            // - Login
            uiEvt.Login_OnStartGameClickHandle += () => {
                LoginBusiness.Exit(loginBusinessContext);
                GameBusiness.StartGame(gameBusinessContext);
            };

            uiEvt.Login_OnExitGameClickHandle += () => {
                LoginBusiness.ExitApplication(loginBusinessContext);
            };

        }

        async Task LoadAssets() {
            await UIApp.LoadAssets(uiAppContext);
            await VFXApp.LoadAssets(vfxAppContext);
            await AssetsInfra.LoadAssets(assetsInfraContext);
            await TemplateInfra.LoadAssets(templateInfraContext);
        }

        void OnApplicationQuit() {
            TearDown();
        }

        void OnDestroy() {
            TearDown();
        }

        void TearDown() {
            if (isTearDown) {
                return;
            }
            isTearDown = true;

            loginBusinessContext.evt.Clear();
            uiAppContext.evt.Clear();

            GameBusiness.TearDown(gameBusinessContext);
            AssetsInfra.ReleaseAssets(assetsInfraContext);
            TemplateInfra.Release(templateInfraContext);
            // TemplateInfra.ReleaseAssets(templateInfraContext);
            // UIApp.TearDown(uiAppContext);
        }

        void OnDrawGizmos() {
            GameBusiness.OnDrawGizmos(gameBusinessContext, drawCameraGizmos);
        }

    }

}