using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Raylib.RaylibUtile;
using VectorUtilises;
using static Raylib_cs.Raylib;

namespace Newton
{
    public class SpatialManager2D
    {
        private Random rnd = new Random();

        private RenderManager renderManager;
        private InputHandler inputHandler;
        private Scene scene = new Scene();

        private float timeStep = 1f;
        private bool doSimulation = false; 
        private MassiveBody? simulationTarget;
        private LineRenderer simulationLine = new LineRenderer();
        private MassiveBody? camTarget;
        private List<MassiveBody> lstBodys = new List<MassiveBody>();
        private List<InputEvent> lstInputEvents = new List<InputEvent>();
        private List<Vector2> lstSimuBodys = new List<Vector2>();
        private Vector2 camMouv = new Vector2(0, 0);

        public SpatialManager2D(RenderManager renderManager, InputHandler inputController)
        {
            this.renderManager = renderManager;
            this.inputHandler = inputController;
        }
        public void Start()
        {
            this.Init();
            // Main game loop
            while (renderManager.isRendering)
            {
                this.inputHandler.InputEvent();
                if (doSimulation) this.Simulate();
                if (camTarget != null) this.MouvRelativ(camTarget.position, this.lstBodys);
                this.MouvBodys(this.inputHandler.targetDirection, this.lstBodys);
                this.DoInteraction(this.lstBodys, this.timeStep);
                this.renderManager.RenderFrame();
            }

        }
        private void Init()
        {
            this.scene.sceneSize = new Vector2(1500, 1000);
            this.scene.backGroundColor = Color.BLACK;
            this.InitInterface();
            this.InitInputEvents();

            this.inputHandler.Init(this.lstInputEvents, this.scene);
            this.renderManager.Init(this.scene);

            this.inputHandler.SetScrollUP(() =>
            {
                if (this.scene.zoom < 20) this.scene.zoom *= 2;
            });
            this.inputHandler.SetScrollDOWN(() =>
            {
                if (this.scene.zoom > 0.25) this.scene.zoom /= 2;
            });
        }
        private void InitInterface()
        {
            //Interface Object
            Button btnNewbody = new Button("New body");
            btnNewbody.Mouv(new Vector2(1200, -1000));
            btnNewbody.Resize(new Vector2(300, 100));
            btnNewbody.color = Color.BROWN;
            btnNewbody.SetBehavior(() =>
            {
                if (this.lstBodys.Count < 20) this.GenerateBody();
            });

            TextBox txtBoxRadius = new TextBox("Radius");
            txtBoxRadius.Mouv(new Vector2(1200, -850));
            txtBoxRadius.Resize(new Vector2(300, 100));
            txtBoxRadius.color = Color.BROWN;
            txtBoxRadius.SetBehavior((string input) =>
            {
                try { return Convert.ToInt32(input) > 0 && Convert.ToInt32(input) < 1000 ? true : false; }
                catch { return false; }
            }, (string input) =>
            {
                if (this.inputHandler.LastActivation != null && this.inputHandler.LastActivation.GetType() == typeof(MassiveBody)) 
                {
                    MassiveBody body = (MassiveBody)this.inputHandler.LastActivation;
                    body.SetRadius(Convert.ToInt32(input));
                }
            });

            TextBox txtBoxSurfG = new TextBox("Surface G");
            txtBoxSurfG.Mouv(new Vector2(1200, -750));
            txtBoxSurfG.Resize(new Vector2(300, 100));
            txtBoxSurfG.color = Color.BROWN;
            txtBoxSurfG.SetBehavior((string input) =>
            {
                try { return Convert.ToInt32(input) > 0 && Convert.ToInt32(input) < 1000 ? true : false; }
                catch { return false; }
            }, (string input) =>
            {
                if (this.inputHandler.LastActivation != null && this.inputHandler.LastActivation.GetType() == typeof(MassiveBody))
                {
                    MassiveBody body = (MassiveBody)this.inputHandler.LastActivation;
                    body.SetSurfaceGravity(Convert.ToInt32(input));
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
            lbMenu.fontSize = 30;
            lbMenu.Mouv(new Vector2(-1500, -1000));
            lbMenu.SetContent("Commades :", new List<string> {
                "|---------------------------------------------|",
                "| Press M to hide/show the menu.",
                "| Press V to see the bodys speed Vector.",
                "| Press UP to Fast forward time.",
                "| Press DOWN to go back in time.",
                "| Pressed BACK SPACE to erase all objects.",
                "| You can move with WSAD.",
                "| You can zoom with scroolwheel",
                "| Left click on a body to get additional info.",
                "|---------------------------------------------|",
                "| After Left click :",
                "| Press R to be centered on the body.",
                "| Right click to reposition the body.",
                "| Maintain right click to change speed.",
                "| You can change some params,",
                "| in the textboxs on the right.",
                "|---------------------------------------------|",
            });

 
            this.simulationLine.isHidden = true;

            this.scene.AddGameObject(this.simulationLine);
            this.scene.AddGameObject(btnNewbody);
            this.scene.AddGameObject(txtBoxSurfG);
            this.scene.AddGameObject(txtBoxRadius);
            this.scene.AddGameObject(txtBoxName);
            this.scene.AddGameObject(lbMenu);
        }
        private void InitInputEvents()
        {
            InputEvent fastForward = new InputEvent(() =>
            {
                this.timeStep += 0.1f;
            });
            fastForward.AddKeyBinder(KeyboardKey.KEY_UP, InputState.DOWN);

            InputEvent goBack = new InputEvent(() =>
            {
                this.timeStep -= 0.1f;
            });
            goBack.AddKeyBinder(KeyboardKey.KEY_DOWN, InputState.DOWN);

            InputEvent paused = new InputEvent(() =>
            {
                this.timeStep = 0;
            });
            paused.AddKeyBinder(KeyboardKey.KEY_SPACE, InputState.PRESS);

            InputEvent cleanScene = new InputEvent(() =>
            {
                foreach (MassiveBody body in this.lstBodys)
                {
                    this.scene.RemoveGameObject(body);
                }
                this.lstBodys.Clear();
            });
            cleanScene.AddKeyBinder(KeyboardKey.KEY_BACKSPACE, InputState.PRESS);

            InputEvent hideMenu = new InputEvent(() =>
            {
                GameObject gameObject = this.scene.lstGameObjects.Where(gameObject => gameObject.name == "Menu").First();
                gameObject.isHidden = !gameObject.isHidden;
            });
            hideMenu.AddKeyBinder(KeyboardKey.KEY_M, InputState.PRESS);

            InputEvent newRef = new InputEvent(() =>
            {
                if (this.inputHandler.LastActivation != null && this.inputHandler.LastActivation.GetType() == typeof(MassiveBody))
                {
                    this.camTarget = (MassiveBody)this.inputHandler.LastActivation;
                }
            });
            newRef.AddKeyBinder(KeyboardKey.KEY_R, InputState.PRESS);

            InputEvent cancelRef = new InputEvent(() =>
            {
                this.camTarget = null;
            });
            cancelRef.AddKeyBinder(KeyboardKey.KEY_T, InputState.PRESS);

            InputEvent showVector = new InputEvent(() =>
            {
                foreach (MassiveBody body in this.lstBodys)
                {
                    body.showVector = true;
                }
            });
            showVector.AddKeyBinder(KeyboardKey.KEY_V, InputState.PRESS);


            InputEvent hideParams = new InputEvent(() =>
            {
                foreach (MassiveBody body in this.lstBodys)
                {
                    body.showParams = false;
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
            moveBody.AddMouseBinder(MouseButton.MOUSE_BUTTON_MIDDLE, InputState.PRESS);

            InputEvent simulate = new InputEvent(() =>
            {
                this.doSimulation = !this.doSimulation;
                this.simulationLine.isHidden = !this.simulationLine.isHidden;
                if (this.doSimulation && this.inputHandler.LastActivation != null && this.inputHandler.LastActivation.GetType() == typeof(MassiveBody)) this.simulationTarget = (MassiveBody)this.inputHandler.LastActivation;
            });
            simulate.AddKeyBinder(KeyboardKey.KEY_N, InputState.PRESS);

            lstInputEvents.AddRange(new List<InputEvent> { paused, hideMenu, cleanScene, goBack, fastForward, newRef, cancelRef, showVector, simulate, moveBody });
        }
        private void GenerateBody()
        {
            MassiveBody body = new MassiveBody(MassiveBody.CreatName());
            body.position = new Vector2(rnd.Next(5) - rnd.Next(5), rnd.Next(5) - rnd.Next(5));
            body.radius = rnd.Next(100) + 50;
            body.SetSurfaceGravity(rnd.Next(100) + 10);
            body.speed = new Vector2(rnd.Next(9) - rnd.Next(9), rnd.Next(9) - rnd.Next(9));
            body.color = rndColor();
            this.lstBodys.Add(body);
            this.scene.AddGameObject(body);
        }
        private void DoInteraction(List<MassiveBody> lstBodys, float timeStep)
        {
            foreach (MassiveBody body in lstBodys)
            {
                body.Gravity(lstBodys, timeStep);
            }

            foreach (MassiveBody body in lstBodys)
            {
                body.ChangePosSpeed(timeStep);
            }
            this.MouvRelativ(this.scene.referencial, lstBodys);
        }
        private void MouvRelativ(Vector2 position, List<MassiveBody> lstBodys)
        {
            Vector2 deltaDestination = this.renderManager.WorldToScreen(position) - this.scene.referencial;
            deltaDestination *= -1;
            MouvBodys(deltaDestination, lstBodys);
        }
        public void MouvBodys(Vector2 deltaDestination, List<MassiveBody> lstBodys)
        {
            foreach (MassiveBody body in lstBodys)
            {
                body.position += deltaDestination;
            }
        }
        private void Simulate()
        {
            if (simulationTarget != null)
            {
                List<Vector2> lstSimuPosition = new List<Vector2>();
                List<MassiveBody> cpLstBodys = CopyList(this.lstBodys);
                for (int i = 0; i < 2000; i++)
                {
                    DoInteraction(cpLstBodys, 1);
                    MassiveBody body = cpLstBodys.Where(body => body.name == this.simulationTarget.name).First();
                    lstSimuPosition.Add(body.position);
                }
                if (simulationLine.ptnsOfLine.Count >= 1999) 
                {
                    this.simulationLine.SetPoints(simulationTarget.position, VectorTools.SmoothTransition(simulationLine.ptnsOfLine, lstSimuPosition), simulationTarget.color);
                } else
                {
                    this.simulationLine.SetPoints(simulationTarget.position, lstSimuPosition, simulationTarget.color);
                }
                
            }
        }
        private Color rndColor()
        {
            Color[] colors = { Color.RED, Color.BLUE, Color.YELLOW, Color.LIME, Color.VIOLET, Color.DARKGRAY, Color.BEIGE };
            return colors[rnd.Next(colors.Length)];
        }
        public static List<MassiveBody> CopyList(List<MassiveBody> ls)
        {
            List<MassiveBody> newLs = new List<MassiveBody>();
            foreach (MassiveBody body in ls)
            {
                MassiveBody newBody = new MassiveBody(body.name);
                newBody.position = new Vector2(body.position.X, body.position.Y);
                newBody.radius = body.radius;
                newBody.SetSurfaceGravity(body.surfaceG);
                newBody.speed = new Vector2(body.speed.X, body.speed.Y);
                newBody.color = body.color;
                newLs.Add(newBody);
            }
            return newLs;
        }
    }
}
