/*******************************************************************************************
Projet Raylib pour l'atelier de première saison.
Auteur: Vinayak Ambigapathy
Date: Septembre 2023
********************************************************************************************/
using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;

namespace Newton;

/// <summary>
/// Class managing the game flow
/// </summary>
class GameManager
{
    /// <summary>
    /// Global scene of the game contei
    /// </summary>
    private Scene2D scene;
    /// <summary>
    /// Rendering component of the game
    /// </summary>
    private RenderManager2D renderManager;
    /// <summary>
    /// Input Control compotent of the game
    /// </summary>
    private InputHandler inputHandler = new InputHandler();
    /// <summary>
    /// Physique engine of the game
    /// </summary>
    private SpatialManager2D spatialManager = new SpatialManager2D();
    /// <summary>
    /// Is the target body of the camera. 
    /// Every thing will rendered based on this position
    /// if it's not null.
    /// </summary>
    private MassiveBody? camTarget;
    /// <summary>
    /// Indicates if the bodys vector should show there vectors
    /// </summary>
    private bool showVector = false;

    private Texture2D planetTexture; 
    private Texture2D backTexture;

    /// <summary>
    /// Creates an instance of the simmulation
    /// with the rendering an input control.
    /// </summary>
    /// <param name="width">Screen width</param>
    /// <param name="height">Screen height</param>
    /// <param name="renderManager">Passed on render</param>
    /// <param name="scene">Passed on scene</param>
    public GameManager(uint width, uint height, RenderManager2D renderManager, Scene2D scene)
    {
        scene.lstGameObjects.Clear();
        this.scene = scene;
        this.scene.sceneSize = new Vector2((int)width, (int)height);
        this.renderManager = renderManager;
    }
    /// <summary>
    /// Main game loop
    /// </summary>
    public void Start()
    {
        this.Init();
        while (renderManager.IsRendering)
        {
            this.inputHandler.InputEvent();
            if (this.spatialManager.doSimulation && this.spatialManager.lstBodys.Count > 0) this.spatialManager.SimulateCPU(this.camTarget, renderManager);
            if (!spatialManager.isCamFixed) Scene2D.MouvGameObjects(this.inputHandler.targetDirection, scene.GetListGameObjectFromType<MassiveBody>());
            if (this.spatialManager.timeStep != 0) this.spatialManager.DoInteractionParallel(this.spatialManager.lstBodys, this.spatialManager.timeStep, this.camTarget, renderManager);
            this.renderManager.RenderFrame();
        }
        renderManager.Close();
        UnloadTexture(planetTexture);

    }
    /// <summary>
    /// Hides or show all the simulation lines from the
    /// games depending on the parameter.
    /// </summary>
    /// <param name="isHidden">Flag for the lines</param>
    private void SetHiddenSimuLines(bool isHidden)
    {
        foreach (LineRenderer line in this.spatialManager.lstOfAnticipationLines)
        {
            line.isHidden = isHidden;
        }
    }
    /// <summary>
    /// Initializes the scene and all it's GameObjects by calling InitInterface, 
    /// the input events with InitInputEvents then add them to the inputHandler,
    /// the physique engine and the Render Manager.
    /// And set the scrolling behavior.
    /// </summary>
    private void Init()
    {
        this.scene.backGroundColor = Color.BLACK;
        this.InitInterface();
        this.inputHandler.Init(this.InitInputEvents(), this.scene);
        this.spatialManager.Init(this.scene);
        this.planetTexture = LoadTexture("assets/flame.png");
        this.inputHandler.SetScrollUP(() =>
        {
            if (this.scene.zoom < 40) this.scene.zoom *= 2;
        });
        this.inputHandler.SetScrollDOWN(() =>
        {
            if (this.scene.zoom > 0.0125) this.scene.zoom /= 2;
        });
    }
    /// <summary>
    /// Initializing All interface components and there callback if 
    /// there Interactive.
    /// </summary>
    private void InitInterface()
    {
        Button btnNewbody = new Button("New body");
        btnNewbody.Mouv(new Vector2(1200, -1000));
        btnNewbody.Resize(new Vector2(300, 100));
        btnNewbody.color = Color.BROWN;
        btnNewbody.SetBehavior(() =>
        {
            for (int i = 0; i < 1; i++)
            {
                this.spatialManager.GenerateBody(this.planetTexture);
            }
            this.spatialManager.lstBodys = this.spatialManager.lstBodys.OrderByDescending(x => x.Masse).ToList();
            //Console.WriteLine(this.spatialManager.lstBodys.Count);
        });

        TextBox txtBoxRadius = new TextBox("Radius");
        txtBoxRadius.Mouv(new Vector2(1200, -850));
        txtBoxRadius.Resize(new Vector2(300, 100));
        txtBoxRadius.color = Color.BROWN;
        txtBoxRadius.SetBehavior((string input) =>
        {
            try { return Convert.ToInt32(input) > 0 && Convert.ToInt32(input) < 10000 ? true : false; }
            catch { return false; }
        }, (string input) =>
        {
            if (this.inputHandler.LastActivation != null && this.inputHandler.LastActivation.GetType() == typeof(MassiveBody))
            {
                MassiveBody body = (MassiveBody)this.inputHandler.LastActivation;
                body.SetRadius(Convert.ToInt32(input));
                this.spatialManager.lstBodys = this.spatialManager.lstBodys.OrderByDescending(x => x.Masse).ToList();
            }
        });

        TextBox txtBoxSurfG = new TextBox("Surface G");
        txtBoxSurfG.Mouv(new Vector2(1200, -750));
        txtBoxSurfG.Resize(new Vector2(300, 100));
        txtBoxSurfG.color = Color.BROWN;
        txtBoxSurfG.SetBehavior((string input) =>
        {
            try { return Convert.ToInt32(input) > 0 && Convert.ToInt32(input) < 10000 ? true : false; }
            catch { return false; }
        }, (string input) =>
        {
            if (this.inputHandler.LastActivation != null && this.inputHandler.LastActivation.GetType() == typeof(MassiveBody))
            {
                MassiveBody body = (MassiveBody)this.inputHandler.LastActivation;
                body.SetSurfaceGravity(Convert.ToInt32(input));
                this.spatialManager.lstBodys = this.spatialManager.lstBodys.OrderByDescending(x => x.Masse).ToList();
            }
        });

        TextBox txtBoxName = new TextBox("Name");
        txtBoxName.Mouv(new Vector2(1200, -650));
        txtBoxName.Resize(new Vector2(300, 100));
        txtBoxName.color = Color.BROWN;
        txtBoxName.SetBehavior((string input) =>
        {
            if (this.inputHandler.LastActivation != null && this.inputHandler.LastActivation.GetType() == typeof(MassiveBody) && !string.IsNullOrEmpty(input))
            {
                MassiveBody body = (MassiveBody)this.inputHandler.LastActivation;
                body.name = input;
            }
        });

        TextLabel lbMenu = new TextLabel("Menu");
        lbMenu.fontSize = 28;
        lbMenu.Mouv(new Vector2(-1500, -1000));
        lbMenu.SetContent("Commades :", new List<string> {
            "|---------------------------------------------------------|",
            "| Press M to hide/show the menu.",
            "| Press V to see the bodys speed Vector.",
            "| Press the UP and DOWN arrows to change time.",
            "| Pressed BACK SPACE to erase all objects.",
            "| You can move with W S A D.",
            "| You can zoom with the scroolwheel.",
            "| Maintain Left mouse button to change speed.",
            "|---------------------------------------------|",
            "| Right click on a body to get focus.",
            "|---------------------------------------------|",
            "| After Right click :",
            "| Press R to be centered on the body.",
            "| Press T to cancel centering.",
            "| Press on the scrollwheel to reposition the body.",
            "| You can change some params, in the textboxs on the right.",
            "|---------------------------------------------------------|",
        });

        this.scene.AddGameObject(btnNewbody);
        this.scene.AddGameObject(txtBoxSurfG);
        this.scene.AddGameObject(txtBoxRadius);
        this.scene.AddGameObject(txtBoxName);
        this.scene.AddGameObject(lbMenu);
    }
    /// <summary>
    /// Set all the keyboard and mouse behavior and theres callbacks
    /// </summary>
    /// <returns>List of all the inputs control</returns>
    private List<InputEvent> InitInputEvents()
    {
        InputEvent fastForward = new InputEvent(() =>
        {
            if (this.spatialManager.timeStep < 1000) this.spatialManager.timeStep += 1;
        });
        fastForward.AddKeyBinder(KeyboardKey.KEY_UP, InputState.DOWN);

        InputEvent goBack = new InputEvent(() =>
        {
            if (this.spatialManager.timeStep > -1000) this.spatialManager.timeStep -= 1;
        });
        goBack.AddKeyBinder(KeyboardKey.KEY_DOWN, InputState.DOWN);

        InputEvent paused = new InputEvent(() =>
        {
            this.spatialManager.timeStep = 0;
        });
        paused.AddKeyBinder(KeyboardKey.KEY_SPACE, InputState.PRESS);

        InputEvent cleanScene = new InputEvent(() =>
        {
            foreach (MassiveBody body in this.spatialManager.lstBodys)
            {
                this.scene.RemoveGameObject(body);
            }
            foreach(LineRenderer line in this.spatialManager.lstOfAnticipationLines)
            {
                this.scene.RemoveGameObject(line);
            }
            this.spatialManager.lstOfAnticipationLines.Clear();
            this.spatialManager.lstBodys.Clear();
            this.spatialManager.doSimulation = false;
        });
        cleanScene.AddKeyBinder(KeyboardKey.KEY_BACKSPACE, InputState.PRESS);

        InputEvent hideMenu = new InputEvent(() =>
        {
            GameObject2D gameObject = this.scene.lstGameObjects.Where(gameObject => gameObject.name == "Menu").First();
            gameObject.isHidden = !gameObject.isHidden;
        });
        hideMenu.AddKeyBinder(KeyboardKey.KEY_M, InputState.PRESS);

        InputEvent newRef = new InputEvent(() =>
        {
            if (this.inputHandler.LastActivation != null && this.inputHandler.LastActivation.GetType() == typeof(MassiveBody))
            {
                this.spatialManager.isCamFixed = true;
                this.camTarget = (MassiveBody)this.inputHandler.LastActivation;
            }
            int i = 0;
            foreach(GameObject2D gameObject in spatialManager.lstBodys)
            {
                if(gameObject == this.camTarget)
                {
                    this.spatialManager.lstOfAnticipationLines[i].isHidden = true;
                    break;
                }
                i++;
            }
        });
        newRef.AddKeyBinder(KeyboardKey.KEY_R, InputState.PRESS);

        InputEvent cancelRef = new InputEvent(() =>
        {
            this.spatialManager.isCamFixed = false;
            this.camTarget = null;
            int i = 0;
            foreach (GameObject2D gameObject in spatialManager.lstBodys)
            {
                if (gameObject == this.camTarget)
                {
                    this.spatialManager.lstOfAnticipationLines[i].isHidden = false;
                    break;
                }
                i++;
            }
        });
        cancelRef.AddKeyBinder(KeyboardKey.KEY_T, InputState.PRESS);

        InputEvent showVector = new InputEvent(() =>
        {
            this.showVector = !this.showVector;
            foreach (MassiveBody body in this.spatialManager.lstBodys)
            {
                body.ShowVector = this.showVector;
            }
        });
        showVector.AddKeyBinder(KeyboardKey.KEY_V, InputState.PRESS);


        InputEvent hideParams = new InputEvent(() =>
        {
            foreach (MassiveBody body in this.spatialManager.lstBodys)
            {
                body.ShowParams = false;
            }
        });
        hideParams.AddKeyBinder(KeyboardKey.KEY_ENTER, InputState.PRESS);

        InputEvent moveBody = new InputEvent(() =>
        {
            if (this.inputHandler.LastActivation != null && this.inputHandler.LastActivation.GetType() == typeof(MassiveBody))
            {
                this.inputHandler.LastActivation.position = this.inputHandler.vMouse * this.scene.zoom;
            }
        });
        moveBody.AddMouseBinder(MouseButton.MOUSE_BUTTON_MIDDLE, InputState.DOWN);

        InputEvent simulate = new InputEvent(() =>
        {
            this.spatialManager.doSimulation = !this.spatialManager.doSimulation;
            this.SetHiddenSimuLines(!this.spatialManager.doSimulation);
        });
        simulate.AddKeyBinder(KeyboardKey.KEY_N, InputState.PRESS);

        return new List<InputEvent> { paused, hideMenu, cleanScene, goBack, fastForward, newRef, cancelRef, showVector, simulate, moveBody };
    }
}
