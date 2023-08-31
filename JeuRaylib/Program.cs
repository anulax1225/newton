/*******************************************************************************************
Projet Raylib pour l'atelier de première saison.
Auteur: Vinayak Ambigapathy
********************************************************************************************/

using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Newton
{
    public class GameMenu
    {
        public static int Main()
        {
            SpatialManager2DRender spm = new SpatialManager2DRender(1500, 1000);
            spm.Start();
            return 0;
        }
    }

    public class SpatialManager2DRender
    {
        int screenWidth;
        int screenHeight;

        private bool isCamFixed = true;
        private bool isInfoHidden = false;
        private bool isPaused = false;
        private float zoom = 1f;
        private int indexClick = -1;
        private int camTarget = -1;

        private Button btnNewBody;
        private List<Button> lstBtns = new List<Button>();
        private List<MassiveBody> lstBodys = new List<MassiveBody>();
        private Camera2D cam = new Camera2D();

        public SpatialManager2DRender(uint screenWidth, uint screenHeight)
        {
            this.screenWidth = (int)(screenWidth);
            this.screenHeight = (int)(screenHeight);
            btnNewBody = GenerateButton("New body", 1200, -1000, 300, 100, Color.BROWN);
            this.Init();
        }

        private void Init()
        {
            // Initialization of the camera
            this.cam.offset = new Vector2(this.screenWidth / 2, this.screenHeight / 2);
            this.cam.zoom = 0.5f;
            InitWindow(this.screenWidth, this.screenHeight, "Simu de Newton");
            // Set our game to run at 60 frames-per-second
            SetTargetFPS(60);
            // Button
            lstBtns.Add(btnNewBody);
        }

        public void Start()
        {
            // Main game loop
            while (!WindowShouldClose())
            {
                // Update
                //-----------------------------------------------------
                this.MouvCamRelativ();
                this.GetUserInput();

                // Draw
                //-----------------------------------------------------
                BeginDrawing();
                BeginMode2D(this.cam);
                ClearBackground(Color.BLACK);
                if (!this.isInfoHidden)
                {
                    this.DrawCommand();
                }
                if (this.indexClick >= 0)
                {
                    this.DrawParamInfo(this.lstBodys[this.indexClick]);
                }
                if (!this.isPaused)
                {
                    this.MouvBody();
                }
                if (IsKeyDown(KeyboardKey.KEY_V))
                {
                    foreach (MassiveBody body in this.lstBodys)
                    {
                        DrawLineV(this.WorldToScreen(body.position / this.zoom), body.speed * 20 + this.WorldToScreen(body.position / this.zoom), body.color);
                    }
                }

                foreach (MassiveBody body in this.lstBodys)
                {
                    DrawCircleV(this.WorldToScreen(body.position / this.zoom), body.radius / this.zoom, body.color);
                }
                this.DrawButtons();
                EndMode2D();
                EndDrawing();
                //-----------------------------------------------------
            }
            // De-Initialization
            CloseWindow();
        }

        private void GetUserInput()
        {
            float mouseWheelScroll = GetMouseWheelMove();
            Vector2 vMouse = GetMousePosition();
            if (mouseWheelScroll > 0 && this.zoom < 20)
            {
                this.zoom += 0.5f;
            }
            if (mouseWheelScroll < 0 && this.zoom > 0.5)
            {
                this.zoom -= 0.5f;
            }

            if (IsKeyPressed(KeyboardKey.KEY_SPACE))
            {
                this.isPaused = !this.isPaused;
            }

            if (IsKeyPressed(KeyboardKey.KEY_M))
            {
                this.isInfoHidden = !this.isInfoHidden;
            }

            if (IsKeyPressed(KeyboardKey.KEY_A))
            {
                this.camTarget = this.indexClick;
                this.isCamFixed = !this.isCamFixed;
            }

            if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                this.indexClick = FindOnTarget(vMouse);
            }


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

        private static Button GenerateButton(string name, int x, int y, int width, int height, Color color)
        {
            Vector2 position = new Vector2(x, y);
            Vector2 size = new Vector2(width, height);
            Button button = new Button(name, position, size, color);
            return button;
        }

        private void MouvBody()
        {
            foreach (MassiveBody body in this.lstBodys)
            {
                body.Graviter(this.lstBodys);
            }

            foreach (MassiveBody body in this.lstBodys)
            {
                body.ChangePosSpeed();
            }
        }

        private void MouvCamRelativ()
        {
            if (this.camTarget >= 0 && this.camTarget < this.lstBodys.Count() && this.isCamFixed)
            {
                Vector2 deltaDestination = WorldToScreen(this.lstBodys[this.camTarget].position) - this.cam.target;
                deltaDestination *= -1;
                foreach (MassiveBody body in this.lstBodys)
                {
                    body.position += deltaDestination;
                }
            }
        }

        private int FindOnTarget(Vector2 vMouse)
        {
            vMouse *= 2;
            vMouse.X -= this.screenWidth;
            vMouse.Y -= this.screenHeight;

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
        private void ModifieInfo(MassiveBody body)
        {

        }

        private void DrawButtons()
        {
            int textOffsetY = -20; 
            int fontSize = 40;
            foreach (Button btn in  this.lstBtns)
            {
                Vector2 screenPos = WorldToScreen(btn.position);
                DrawRectangle((int)screenPos.X, (int)screenPos.Y, (int)btn.size.X, (int)btn.size.Y, btn.color);
                int textLenght = MeasureText(btn.name, fontSize);
                DrawText(btn.name, (int)(screenPos.X + (btn.size.X - textLenght) / 2), (int)(textOffsetY + screenPos.Y + btn.size.Y / 2), fontSize, Color.WHITE);
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
        }

        private void DrawCommand()
        {
            int fontSize = 40;
            int textOffsetY = 40;
            int textPosX = (int)(this.cam.target.X - this.cam.offset.X/cam.zoom);
            int textPosY = (int)(this.cam.target.Y - this.cam.offset.Y/ cam.zoom);
            DrawText("Newton's simulation :", textPosX, textPosY, fontSize, Color.WHITE);
            DrawText("Press V to see the bodys speed Vector.", textPosX, textPosY + textOffsetY, fontSize, Color.WHITE);
            DrawText("Left click on a body to get additional info.", textPosX, textPosY + textOffsetY * 2, fontSize, Color.WHITE);
            DrawText("Press M to hide/show the menu.", textPosX, textPosY + textOffsetY * 3, fontSize, Color.WHITE);
            DrawText("Press A to be centered after left clicking.", textPosX, textPosY + textOffsetY * 4, fontSize, Color.WHITE);
        }
    }

    public class MassiveBody
    {
        const float CONSTGRAVITATION = 0.0001f;

        public string name;
        public Vector2 position;
        public Vector2 speed;
        public int radius;
        public float masse;
        public Color color;

        public MassiveBody(string name, Vector2 position, Vector2 vitesse, int radius, float masse, Color color)
        {
            this.name = name;
            this.position = position;
            this.speed = vitesse;
            this.radius = radius;
            this.masse = masse;
            this.color = color;
        }
        public void Graviter(List<MassiveBody> lstBody)
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

                    this.speed += acceleration;
                }
            }
        }

        public void ChangePosSpeed()
        {
            //Console.WriteLine(this.ToString());
            this.position += this.speed;
        }

        public override string ToString()
        {
            return $"etoile {this.name} position {this.position} vitesse {this.speed}";
        }
    }

    public class Button
    {
        public string name;
        public Vector2 position;
        public Vector2 size;
        private Rectangle border;
        public Color color;

        public Button(string name, Vector2 position, Vector2 size, Color color)
        {
            this.name = name;
            this.position = position;
            this.size = size;
            this.color = color;
        }
        private void Generate()
        {
            this.border = new Rectangle((int)this.position.X, (int)this.position.Y, (int)this.size.X, (int)this.size.Y);
        }

        public void Resize(Vector2 newSize)
        {
            this.size = newSize;
            Generate();
        }

        public void Mouv(Vector2 newPosition)
        {
            this.position = newPosition;
            Generate();
        }

        public Rectangle GetBorder()
        {
            return this.border;
        }

    }

    public class VectorTools
    {
        public static float Vector2Normalize(Vector2 v)
        {
            float pitaRes = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
            return pitaRes;
        }

        public static Vector2 Vector2AVG(List<Vector2> lsVectors)
        {
            float x = 0, y = 0;
            int cpt = 0;
            foreach (Vector2 v in lsVectors)
            {
                x += v.X;
                y += v.Y;
                cpt += 1;
            }
            return new Vector2(x / cpt, y / cpt);
        }
		
		public static Vector2 VectorGetRemainder(Vector2 v, Vector2 div)
        {
            float X = v.X;
            float Y = v.Y;

            while (X >= div.X && Y >= div.Y)
            {
                if (X >= div.X)
                {
                    X -= div.X;
                }

                if (Y >= div.Y)
                {
                    Y -= div.Y;
                }
            }
            return new Vector2(X, Y);
        }
		
		public static Vector2 VectorGetDivision(Vector2 v, Vector2 div)
        {	
            return new Vector2(v.X / div.X, v.Y / div.Y);
        }
    }
}