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
        int screenWidth;
        int screenHeight;

        private bool isCamFixed = true;
        private bool isInfoHidden = false;
        bool protectedContainer = false;

        private float zoom = 1f;
        private float timeStep = 1f;

        private int indexClick = -1;
        private int camTarget = -1;

        private Random rnd = new Random();
        
        private List<Button> lstBtns = new List<Button>();
        private List<TextBox> lstTextBox = new List<TextBox>();
        private List<MassiveBody> lstBodys = new List<MassiveBody>();

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

            lstTextBox.Add(GenerateTextBox("Change radius", 1200, -850, 300, 100, Color.BROWN, (string input) =>
            {
                try { return Convert.ToInt32(input) > 0 && Convert.ToInt32(input) < 1000 ? true : false; }
                catch { return false; }
            },
            (string input) =>
            {
                if (this.indexClick >= 0) this.lstBodys[this.indexClick].SetRadius(Convert.ToInt32(input));
            }));

            lstTextBox.Add(GenerateTextBox("Change name", 1200, -700, 300, 100, Color.BROWN, (string input) =>
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

                this.MouvBody();
                this.DrawTextBox();
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
            this.timeStep = 1;
            this.zoom = 1f;
            this.indexClick = -1;
            this.camTarget = -1;
            this.lstBtns = new List<Button>();
            this.lstTextBox = new List<TextBox>();
            this.lstBodys = new List<MassiveBody>();
            this.camMouv = new Vector2(0, 0);
            this.cam = new Camera2D();
            this.Init();
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
                    textBox.isWriting = true;
                    protectedContainer = true;
                    char input = TextBox.Write();
                    if (input == '\0')
                    {
                        if (textBox.GetVerification())
                        {
                            textBox.OnValidation();
                            textBox.error = "";
                        }
                        else
                        {
                            textBox.error = "Typing error";
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
            if (mouseWheelScroll > 0 && this.zoom < 100)
            {
                this.zoom *= 2;
            }
            if (mouseWheelScroll < 0)
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
            if (IsKeyPressed(KeyboardKey.KEY_SPACE))
            {
                this.timeStep = 0;
            }

            if (IsKeyPressed(KeyboardKey.KEY_M))
            {
                this.isInfoHidden = !this.isInfoHidden;
            }

            if (IsKeyPressed(KeyboardKey.KEY_R))
            {
                this.camTarget = this.indexClick;
                this.isCamFixed = !this.isCamFixed;
            }
            if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                this.indexClick = FindOnTarget(vMouse);
            }

            if (IsKeyDown(KeyboardKey.KEY_UP))
            {
                timeStep += 0.1f;
            }

            if (IsKeyDown(KeyboardKey.KEY_DOWN))
            {
                timeStep -= 0.1f;
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

        private void MouvBody()
        {
            foreach (MassiveBody body in this.lstBodys)
            {
                body.Graviter(this.lstBodys, timeStep);
            }

            foreach (MassiveBody body in this.lstBodys)
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
            int fontSize = 40;
            foreach (Button btn in this.lstBtns)
            {
                Vector2 screenPos = WorldToScreen(btn.position);
                DrawRectangle((int)screenPos.X, (int)screenPos.Y, (int)btn.size.X, (int)btn.size.Y, btn.color);
                int textLenght = MeasureText(btn.name, fontSize);
                DrawText(btn.name, (int)(screenPos.X + (btn.size.X - textLenght) / 2), (int)(textOffsetY + screenPos.Y + btn.size.Y / 2), fontSize, Color.WHITE);
            }
        }

        private void DrawTextBox()
        {
            int textOffsetY = -20;
            int fontSize = 40;
            foreach (TextBox textBox in this.lstTextBox)
            {
                Vector2 screenPos = WorldToScreen(textBox.position);
                DrawRectangle((int)screenPos.X, (int)screenPos.Y, (int)textBox.size.X, (int)textBox.size.Y, textBox.color);
                string input = textBox.GetBufferInput();
                if (textBox.isWriting && !(input == null))
                {
                    int textLenght = MeasureText(input, fontSize);
                    DrawText(input, (int)(screenPos.X + (textBox.size.X - textLenght) / 2), (int)(textOffsetY + screenPos.Y + textBox.size.Y / 2), fontSize, Color.WHITE);
                } 
                else if (!String.IsNullOrEmpty(textBox.error))
                {
                    int textLenght = MeasureText(textBox.error, fontSize);
                    DrawText(textBox.error, (int)(screenPos.X + (textBox.size.X - textLenght) / 2), (int)(textOffsetY + screenPos.Y + textBox.size.Y / 2), fontSize, Color.WHITE);
                } 
                else
                {
                    int textLenght = MeasureText(textBox.name, fontSize);
                    DrawText(textBox.name, (int)(screenPos.X + (textBox.size.X - textLenght) / 2), (int)(textOffsetY + screenPos.Y + textBox.size.Y / 2), fontSize, Color.WHITE);
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
            if (!this.isInfoHidden)
            {
                this.DrawCommand();
            }
            if (this.indexClick >= 0)
            {
                this.DrawParamInfo(this.lstBodys[this.indexClick]);
            }
            if (IsKeyDown(KeyboardKey.KEY_V))
            {
                foreach (MassiveBody body in this.lstBodys)
                {
                    DrawLineV(this.WorldToScreen(body.position / this.zoom), body.speed * 20 + this.WorldToScreen(body.position / this.zoom), body.color);
                }
            }
        }

        private void DrawParamInfo(MassiveBody body)
        {
            float textOffset = 40f;
            int fontSize = 30;
            Vector2 pos = WorldToScreen(body.position / this.zoom);
            int textPosX = (int)(pos.X + body.radius + textOffset / this.zoom);
            DrawText(String.Format("Name : {0}", body.name), textPosX, (int)(pos.Y + body.radius + textOffset), fontSize, Color.WHITE);
            DrawText(String.Format("Speed : {0}", VectorTools.Vector2Normalize(body.speed), 2), textPosX, (int)(pos.Y + body.radius + textOffset * 2), fontSize, Color.WHITE);
            DrawText(String.Format("Masse : {0}", body.masse), textPosX, (int)(pos.Y + body.radius + textOffset * 3), fontSize, Color.WHITE);
            DrawText(String.Format("Radius : {0}", body.radius), textPosX, (int)(pos.Y + body.radius + textOffset * 4), fontSize, Color.WHITE);
        }

        private void DrawCommand()
        {
            int fontSize = 35;
            int textOffsetY = 40;
            int textPosX = (int)(this.cam.target.X - this.cam.offset.X / cam.zoom);
            int textPosY = (int)(this.cam.target.Y - this.cam.offset.Y / cam.zoom);
            DrawText("Newton's simulation :", textPosX, textPosY, fontSize, Color.WHITE);
            DrawText("Press V to see the bodys speed Vector.", textPosX, textPosY + textOffsetY, fontSize, Color.WHITE);
            DrawText("Left click on a body to get additional info.", textPosX, textPosY + textOffsetY * 2, fontSize, Color.WHITE);
            DrawText("Press M to hide/show the menu.", textPosX, textPosY + textOffsetY * 3, fontSize, Color.WHITE);
            DrawText("Press R to be centered after left clicking.", textPosX, textPosY + textOffsetY * 4, fontSize, Color.WHITE);
            DrawText("Press UP to Fast forward time. ", textPosX, textPosY + textOffsetY * 5, fontSize, Color.WHITE);
            DrawText("Press DOWN to go back in time.", textPosX, textPosY + textOffsetY * 6, fontSize, Color.WHITE);
        }
    }

    public class MassiveBody
    {
        const float CONSTGRAVITATION = 1f;

        public string name;
        public Vector2 position;
        public Vector2 speed;
        public int radius;
        public float surfaceG;
        public float masse;
        public Color color;

        public MassiveBody(string name, Vector2 position, Vector2 vitesse, int radius, float surfaceG, Color color)
        {
            this.name = name;
            this.position = position;
            this.speed = vitesse;
            this.surfaceG = surfaceG;
            this.color = color;
            this.SetRadius(radius);
        }

        public void SetRadius(int radius)
        {
            this.radius = radius;
            this.masse = (surfaceG * radius * radius) / CONSTGRAVITATION;
        }

        public void Graviter(List<MassiveBody> lstBody, float timeStep)
        {
            foreach (MassiveBody body in lstBody)
            {
                if (body != this)
                {
                    Vector2 vFab = new Vector2(1, 1);
                    Vector2 distance = body.position - this.position;
                    float norme = VectorTools.Vector2Normalize(distance);
                    float Fab = CONSTGRAVITATION * ((body.masse) / norme);//Avec une seul masse 
                    Vector2 Force = vFab * Fab;

                    if (this.position.X - body.position.X >= 0)
                    {
                        Force.X *= -1;
                    }

                    if (this.position.Y - body.position.Y >= 0)
                    {
                        Force.Y *= -1;
                    }

                    Vector2 acceleration = Force / this.masse;

                    this.speed += acceleration * timeStep;
                }
            }
        }

        public void ChangePosSpeed(float timeStep)
        {
            //Console.WriteLine(this.ToString());
            this.position += this.speed * timeStep;
        }

        public static string CreatName()
        {
            string[] principal = { "Prime", "Star", "Luna", "Planetrium" };
            char[] chars = { '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'Z', 'Y', 'X', 'C' };
            Random rnd = new Random();
            string rndName = principal[rnd.Next(principal.Length)];
            for (int i = 0; i < rnd.Next(5); i++)
            {
                rndName += chars[rnd.Next(chars.Length)];
            }
            return rndName;
        }

        public override string ToString()
        {
            return $"etoile {this.name} position {this.position} vitesse {this.speed}";
        }
    }
}
