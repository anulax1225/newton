using GUIInterfaceRaylib;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VectorUtilises;
using static Raylib_cs.Raylib;

namespace Newton
{
    public class SpatialManager2DRender
    {
        private int screenWidth;
        private int screenHeight;

        private bool isCamFixed = true;
        private bool isInfoHidden = false;
        private bool protectedContainer = false;
        private bool protectedVectorChange = false;

        private bool simuBufferFull = false;
        private int simuTarget = -1;

        private float zoom = 1f;
        private float timeStep = 1f;

        private int indexClick = -1;
        private int camTarget = -1;

        private Random rnd = new Random();

        private List<Button> lstBtns = new List<Button>();
        private List<TextBox> lstTextBox = new List<TextBox>();
        private List<MassiveBody> lstBodys = new List<MassiveBody>();
        private List<Vector2> lstSimuBodys = new List<Vector2>();

        private Camera2D cam = new Camera2D();
        private Vector2 camMouv = new Vector2(0, 0);

        public SpatialManager2DRender(uint screenWidth, uint screenHeight)
        {
            this.screenWidth = (int)(screenWidth);
            this.screenHeight = (int)(screenHeight);
        }

        private void Init()
        {
            // Initialization of the camera
            this.cam.offset = new Vector2(this.screenWidth / 2, this.screenHeight / 2);
            this.cam.zoom = 0.5f;
            InitWindow(this.screenWidth, this.screenHeight, "Simu de Newton");
            // Set our game to run at 60 frames-per-second
            SetTargetFPS(60);
            this.InitInterface();
        }

        private void InitInterface()
        {
            lstBtns.Add(GenerateButton("New body", 1200, -1000, 300, 100, Color.BROWN, () =>
            {
                lstBodys.Add(GenerateBody(MassiveBody.CreatName(), 1, 1, rnd.Next(100) + 50, rnd.Next(100) + 10, rnd.Next(9) - rnd.Next(9), rnd.Next(9) - rnd.Next(9), rndColor()));
            }));


            lstTextBox.Add(GenerateTextBox("Name", 1200, -850, 300, 100, Color.BROWN, (string input) =>
            {
                if (!string.IsNullOrEmpty(input))
                {
                    return true;
                }
                return false;
            },
            (string input) =>
            {
                if (this.indexClick >= 0) this.lstBodys[this.indexClick].name = input;
            }));

            lstTextBox.Add(GenerateTextBox("Radius", 1200, -750, 300, 100, Color.BROWN, (string input) =>
            {
                try { return Convert.ToInt32(input) > 0 && Convert.ToInt32(input) < 1000 ? true : false; }
                catch { return false; }
            },
            (string input) =>
            {
                if (this.indexClick >= 0) this.lstBodys[this.indexClick].SetRadius(Convert.ToInt32(input));
            }));

            lstTextBox.Add(GenerateTextBox("Surface G", 1200, -650, 300, 100, Color.BROWN, (string input) =>
            {
                try { return Convert.ToInt32(input) > 0 && Convert.ToInt32(input) < 1000 ? true : false; }
                catch { return false; }
            },
            (string input) =>
            {
                if (this.indexClick >= 0) this.lstBodys[this.indexClick].SetSurfaceGravity(Convert.ToInt32(input));
            }));
        }

        public void Start()
        {
            this.Init();
            // Main game loop
            while (!WindowShouldClose())
            {
                this.MouvCamRelativ();
                this.GetUserInput();

                BeginDrawing();
                BeginMode2D(this.cam);

                ClearBackground(Color.BLACK);

                this.MouvBody(this.lstBodys, this.timeStep);
                this.DrawButtons();
                this.DrawBodys();
                this.DrawConditional();

                EndMode2D();
                EndDrawing();
            }
            // De-Initialization
            CloseWindow();
        }

        public void Reset(int screenWidth, int screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.isCamFixed = true;
            this.isInfoHidden = false;
            this.protectedContainer = false;
            this.timeStep = 1f;
            this.zoom = 1f;
            this.indexClick = -1;
            this.camTarget = -1;
            this.lstBtns = new List<Button>();
            this.lstTextBox = new List<TextBox>();
            this.lstBodys = new List<MassiveBody>();
            this.camMouv = new Vector2(0, 0);
            this.cam = new Camera2D();
        }

        private void GetUserInput()
        {

            Vector2 vMouse = GetMousePosition();
            vMouse *= 2;
            vMouse.X -= this.screenWidth;
            vMouse.Y -= this.screenHeight;

            foreach (Button btn in lstBtns)
            {
                if (btn.CheckCollison(vMouse) && IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    btn.OnClick();
                }
            }
            if (indexClick >= 0) this.GetTextBoxInput(vMouse);
            if (!this.protectedContainer) this.GetKeyCommandInput(vMouse);
        }

        private void GetTextBoxInput(Vector2 vMouse)
        {
            foreach (TextBox textBox in lstTextBox)
            {
                if ((textBox.CheckCollison(vMouse) && IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) || textBox.isWriting)
                {
                    DisableCursor();
                    textBox.fontSize = 40;
                    textBox.isWriting = true;
                    textBox.error = "";
                    protectedContainer = true;
                    char input = TextBox.Write();
                    if (input == '\0')
                    {
                        if (textBox.GetVerification())
                        {
                            textBox.OnValidation();
                        }
                        else
                        {
                            textBox.error = "Typing Error";
                            textBox.fontSize = 20;
                        }
                        protectedContainer = false;
                        textBox.isWriting = false;
                        textBox.FlushBuffer();
                        EnableCursor();

                    }
                    else if (!(input == '$'))
                    {
                        textBox.inputsBuffer.Add(input);
                    }
                }
            }
        }

        private void GetKeyCommandInput(Vector2 vMouse)
        {
            float mouseWheelScroll = GetMouseWheelMove();

            // Zoom Input
            if (mouseWheelScroll < 0 && this.zoom < 100)
            {
                this.zoom *= 2;
            }
            if (mouseWheelScroll > 0)
            {
                this.zoom *= 0.5f;
            }

            // Camera mouvement Input
            if (IsKeyDown(KeyboardKey.KEY_W))
            {
                this.camMouv.Y += 4;
            }
            else if (IsKeyDown(KeyboardKey.KEY_S))
            {
                this.camMouv.Y -= 4;
            }
            else if (IsKeyDown(KeyboardKey.KEY_A))
            {
                this.camMouv.X += 4;
            }
            else if (IsKeyDown(KeyboardKey.KEY_D))
            {
                this.camMouv.X -= 4;
            }
            else
            {
                this.camMouv.X = 0;
                this.camMouv.Y = 0;
            }
            MouvBodys(camMouv);

            //Command Input
            if (IsKeyPressed(KeyboardKey.KEY_SPACE)) this.timeStep = 0;
            if (IsKeyPressed(KeyboardKey.KEY_M)) this.isInfoHidden = !this.isInfoHidden;
            if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) this.indexClick = FindOnTarget(vMouse);
            if (IsKeyDown(KeyboardKey.KEY_UP) && !this.protectedVectorChange) timeStep += 0.1f;
            if (IsKeyDown(KeyboardKey.KEY_DOWN) && !this.protectedVectorChange) timeStep -= 0.1f;

            if (IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
            {
                this.lstBodys.Clear();
                this.indexClick = -1;
            }
            if (IsKeyPressed(KeyboardKey.KEY_R))
            {
                this.camTarget = this.indexClick;
                this.isCamFixed = !this.isCamFixed;
            }

            if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT) && this.indexClick >= 0 && timeStep == 0 && IsKeyDown(KeyboardKey.KEY_V)) 
            {
                this.lstBodys[indexClick].speed = this.lstBodys[indexClick].position - vMouse/this.zoom;
                this.simuTarget = this.indexClick;
                this.protectedVectorChange = true;
            }
            if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_RIGHT) && this.protectedVectorChange)
            {
                this.protectedVectorChange = false;
                this.simuTarget = -1;
            }
            if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT) && this.indexClick >= 0 && !this.protectedVectorChange) this.lstBodys[indexClick].position = vMouse * this.zoom;
        }

        private void Simulate()
        {
            if (this.simuTarget >= 0)
            {
                List<MassiveBody> cpLstBodys = MassiveBody.CopyList(this.lstBodys);
                for (int i = 0; i < 10000;  i++)
                {
                    MouvBody(cpLstBodys, 4);
                    this.lstSimuBodys.Add(cpLstBodys[this.simuTarget].position);
                }
                this.simuBufferFull = true;
            }
        }
        private Color rndColor()
        {
            Color[] colors = { Color.RED, Color.BLUE, Color.YELLOW, Color.LIME, Color.VIOLET, Color.DARKGRAY, Color.BEIGE };
            return colors[rnd.Next(colors.Length)];
        }

        private Vector2 WorldToScreen(Vector2 position)
        {
            Vector2 relativPos = position + this.cam.target;
            return relativPos;
        }

        private Vector2 ScreenToWorld(Vector2 position)
        {
            Vector2 relativPos = position - this.cam.target;
            return relativPos;
        }

        private static MassiveBody GenerateBody(string name, float x, float y, int radius, float masse, float speedX, float speedY, Color color)
        {
            Vector2 position = new Vector2(x, y);
            Vector2 speed = new Vector2(speedX, speedY);
            MassiveBody body = new MassiveBody(name, position, speed, radius, masse, color);
            return body;
        }

        private static Button GenerateButton(string name, int x, int y, int width, int height, Color color, Callback cb)
        {
            Vector2 position = new Vector2(x, y);
            Vector2 size = new Vector2(width, height);
            Button button = new Button(name, position, size, color, cb);
            return button;
        }

        private static TextBox GenerateTextBox(string name, int x, int y, int width, int height, Color color, Verifier verifier, Validation cb)
        {
            Vector2 position = new Vector2(x, y);
            Vector2 size = new Vector2(width, height);
            TextBox textBox = new TextBox(name, position, size, color, true, verifier, cb);
            return textBox;
        }

        private void MouvBody(List<MassiveBody> lstBodys, float timeStep)
        {
            foreach (MassiveBody body in lstBodys)
            {
                body.Gravity(lstBodys, timeStep);
            }

            foreach (MassiveBody body in lstBodys)
            {
                body.ChangePosSpeed(timeStep);
            }
        }

        private void MouvCamRelativ()
        {
            if (this.camTarget >= 0 && this.camTarget < this.lstBodys.Count() && this.isCamFixed)
            {
                Vector2 deltaDestination = WorldToScreen(this.lstBodys[this.camTarget].position) - this.cam.target;
                deltaDestination *= -1;
                MouvBodys(deltaDestination);
            }
        }

        private void MouvBodys(Vector2 deltaDestination)
        {
            foreach (MassiveBody body in this.lstBodys)
            {
                body.position += deltaDestination;
            }
        }

        private int FindOnTarget(Vector2 vMouse)
        {
            int i = 0;
            foreach (MassiveBody body in this.lstBodys)
            {
                float vMouseNorme = VectorTools.Vector2Normalize(vMouse - body.position / this.zoom);
                if (vMouseNorme <= body.radius / this.zoom)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        private void DrawButtons()
        {
            int textOffsetY = -20;
            foreach (Button btn in this.lstBtns)
            {
                Vector2 screenPos = WorldToScreen(btn.position);
                DrawRectangle((int)screenPos.X, (int)screenPos.Y, (int)btn.size.X, (int)btn.size.Y, btn.color);
                int textLenght = MeasureText(btn.name, btn.fontSize);
                DrawText(btn.name, (int)(screenPos.X + (btn.size.X - textLenght) / 2), (int)(textOffsetY + screenPos.Y + btn.size.Y / 2), btn.fontSize, Color.WHITE);
            }
        }

        private void DrawTextBox()
        {
            int textOffsetY = -20;
            foreach (TextBox textBox in this.lstTextBox)
            {
                Vector2 screenPos = WorldToScreen(textBox.position);
                DrawRectangle((int)screenPos.X, (int)screenPos.Y, (int)textBox.size.X, (int)textBox.size.Y, textBox.color);
                string input = textBox.GetBufferInput();
                if (textBox.isWriting && !(input == null))
                {
                    int textLenght = MeasureText(input, textBox.fontSize);
                    DrawText(input, (int)(screenPos.X + (textBox.size.X - textLenght) / 2), (int)(textOffsetY + screenPos.Y + textBox.size.Y / 2), textBox.fontSize, Color.WHITE);
                }
                else if (!String.IsNullOrEmpty(textBox.error))
                {
                    int textLenght = MeasureText(textBox.error, textBox.fontSize);
                    DrawText(textBox.error, (int)(screenPos.X + (textBox.size.X - textLenght) / 2), (int)(textOffsetY + screenPos.Y + textBox.size.Y / 2), textBox.fontSize, Color.WHITE);
                }
                else
                {
                    int textLenght = MeasureText(textBox.name, textBox.fontSize);
                    DrawText(textBox.name, (int)(screenPos.X + (textBox.size.X - textLenght) / 2), (int)(textOffsetY + screenPos.Y + textBox.size.Y / 2), textBox.fontSize, Color.WHITE);
                }
            }
        }

        private void DrawBodys()
        {
            foreach (MassiveBody body in this.lstBodys)
            {
                DrawCircleV(this.WorldToScreen(body.position / this.zoom), body.radius / this.zoom, body.color);
            }
        }

        private void DrawConditional()
        {
            if (indexClick >= 0) this.DrawTextBox();
            if (!this.isInfoHidden) this.DrawCommand();
            if (this.indexClick >= 0) this.DrawParamInfo(this.lstBodys[this.indexClick]);

            if (IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT))
            {
                foreach (MassiveBody body in this.lstBodys)
                {
                    DrawParamInfo(body);
                }
            }

            this.Simulate();
            if (this.simuBufferFull)
            {
                Vector2 oldPos = this.lstSimuBodys[0];
                foreach (Vector2 pos in this.lstSimuBodys)
                {
                    DrawLineV(WorldToScreen(oldPos / this.zoom), WorldToScreen(pos / this.zoom), Color.WHITE);
                    oldPos = pos;
                }
                this.simuBufferFull = false;
                lstSimuBodys.Clear();
            }
            
            if (IsKeyDown(KeyboardKey.KEY_V))
            {
                foreach (MassiveBody body in this.lstBodys)
                {
                    DrawLineV(this.WorldToScreen(body.position / this.zoom), body.speed / 0.1f + this.WorldToScreen(body.position / this.zoom), body.color);
                }
            }
        }

        private void DrawParamInfo(MassiveBody body)
        {
            float textOffset = 40f;
            int fontSize = 35;
            Vector2 pos = WorldToScreen(body.position / this.zoom);
            int textPosX = (int)(pos.X + body.radius + textOffset / this.zoom);
            string[] paramsInfo = 
            { 
                "Paramètre info", 
                String.Format("Name : {0}", body.name), 
                String.Format("Masse : {0}", body.masse), 
                String.Format("Radius : {0}", body.radius), 
                String.Format("Surface G : {0}", body.surfaceG), 
                String.Format("Speed : {0}", VectorTools.Vector2Normalize(body.speed)), 
            };
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                DrawText(paramsInfo[i], textPosX, (int)(pos.Y + body.radius + textOffset * i + 1), fontSize, Color.WHITE);
            }
        }

        private void DrawCommand()
        {
            int fontSize = 40;
            int textOffsetY = 42;
            int textPosX = (int)(this.cam.target.X - this.cam.offset.X / cam.zoom);
            int textPosY = (int)(this.cam.target.Y - this.cam.offset.Y / cam.zoom);
            string[] cmds = {
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
            };
            DrawText("Newton's simulation :" , textPosX, textPosY, fontSize, Color.WHITE);

            for (int i = 0; i < cmds.Length; i++)
            {
                DrawText(cmds[i], textPosX, textPosY + textOffsetY * (i + 1), fontSize, Color.WHITE);
            }
        }
    }
}
