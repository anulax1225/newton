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
    public class Game
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

        private int indexClick = -1;
        private int camTarget = -1;

        private List<MassiveBody> lstBody = new List<MassiveBody>();
        private Camera2D cam = new Camera2D();
        public SpatialManager2DRender(uint screenWidth, uint screenHeight)
        {
            this.screenWidth = (int)(screenWidth);
            this.screenHeight = (int)(screenHeight);
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

            // Creating objects of the game
            this.lstBody.Add(GenerateBody("lune corva", 0, 520, 5, 1f, -9f, 0, Color.VIOLET));
            this.lstBody.Add(GenerateBody("Troulaxia", 0, -450, 18, 4, 4.6f, 0, Color.GOLD));
            this.lstBody.Add(GenerateBody("Moncul", 0, 0, 50, 46, 0.1f, 0.1f, Color.RED));
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
                    this.ShowCommand();
                }
                if (this.indexClick >= 0)
                {
                    this.DrawParamInfo(this.lstBody[this.indexClick]);
                }
                if (!this.isPaused)
                {
                    this.MouvBody();
                }
                if (IsKeyPressed(KeyboardKey.KEY_V))
                {
                    foreach (MassiveBody body in this.lstBody)
                    {
                        DrawLineV(this.WorldToScreen(body.position), body.speed * 10 + this.WorldToScreen(body.position), body.color);
                    }
                }
                foreach (MassiveBody body in this.lstBody)
                {
                    DrawCircleV(this.WorldToScreen(body.position), body.radius, body.color);
                }

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

            if (mouseWheelScroll > 0 && this.cam.zoom < 2.1)
            {
                this.cam.zoom += 0.1f;
            }
            if (mouseWheelScroll < 0 && this.cam.zoom > 0.2)
            {
                this.cam.zoom -= 0.1f;
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
                this.indexClick = FindOnTarget(GetMousePosition());
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

        private static MassiveBody GenerateBody(string name, int x, int y, int radius, float masse, float speedX, float speedY, Color color)
        {
            Vector2 position = new Vector2(x, y);
            Vector2 speed = new Vector2(speedX, speedY);
            MassiveBody body = new MassiveBody(name, position, speed, radius, masse, color);
            return body;
        }

        private void MouvBody()
        {
            foreach (MassiveBody body in this.lstBody)
            {
                body.Graviter(this.lstBody);
            }

            foreach (MassiveBody body in this.lstBody)
            {
                body.ChangePosSpeed();
            }
        }

        private int FindOnTarget(Vector2 vMouse)
        {
            vMouse *= 2;
            vMouse.X -= this.screenWidth;
            vMouse.Y -= this.screenHeight;

            int i = 0;
            foreach (MassiveBody body in this.lstBody)
            {
                float vMouseNorme = VectorTools.Vector2Normalize(vMouse - body.position);
                Console.WriteLine($"norme {vMouseNorme}, body {body.name} {body.position}, mouse {vMouse}");
                if (vMouseNorme <= body.radius)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        private void MouvCamRelativ()
        {
            if (this.camTarget >= 0 && this.camTarget < this.lstBody.Count() && this.isCamFixed)
            {
                Vector2 deltaDestination = WorldToScreen(this.lstBody[this.camTarget].position) - this.cam.target;
                deltaDestination *= -1;
                foreach (MassiveBody body in this.lstBody)
                {
                    body.position += deltaDestination;
                }
            }
        }

        private void DrawParamInfo(MassiveBody body)
        {
            float textOffset = 20f / this.cam.zoom;
            int fontSize = (int)(20 / this.cam.zoom);
            Vector2 pos = WorldToScreen(body.position);
            DrawText(String.Format("Name : {0}", body.name), (int)(pos.X + body.radius + textOffset), (int)(pos.Y + body.radius + textOffset), fontSize, Color.WHITE);
            DrawText(String.Format("Speed : {0}", (float)Math.Sqrt(Math.Pow(VectorTools.Vector2Normalize(body.speed), 2))), (int)(pos.X + body.radius + textOffset), (int)(pos.Y + body.radius + textOffset * 2), fontSize, Color.WHITE);
            DrawText(String.Format("Masse : {0}", body.masse), (int)(pos.X + body.radius + textOffset), (int)(pos.Y + body.radius + textOffset * 3), fontSize, Color.WHITE);
        }

        private void ShowCommand()
        {
            int fontSize = (int)(20 / cam.zoom);
            int textOffsetY = (int)(30 / cam.zoom);
            int textPosX = (int)(this.cam.target.X - this.cam.offset.X / this.cam.zoom);
            int textPosY = (int)(this.cam.target.Y - this.cam.offset.Y / this.cam.zoom);
            DrawText("Simulation de Newton.", textPosX, textPosY, fontSize, Color.WHITE);
            DrawText("Press V to see the bodys speed Vector.", textPosX, textPosY + textOffsetY, fontSize, Color.WHITE);
            DrawText("Left click on a body to get additional info.", textPosX, textPosY + textOffsetY * 2, fontSize, Color.WHITE);
            DrawText("Press M to hide/show the menu.", textPosX, textPosY + textOffsetY * 3, fontSize, Color.WHITE);
        }
    }

    public class MassiveBody
    {
        const float CONSTGRAVITATION = 1f;

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
        public void Graviter(List<MassiveBody> lsCorpsMassifs)
        {
            foreach (MassiveBody corp in lsCorpsMassifs)
            {
                if (corp != this)
                {
                    Vector2 vFab = new Vector2(1, 1);
                    Vector2 distance = corp.position - this.position;

                    float norme = VectorTools.Vector2Normalize(distance);
                    float Fab = CONSTGRAVITATION * ((corp.masse) / norme);//Avec une seul masse 
                    Vector2 Force = vFab * Fab;

                    if (this.position.X - corp.position.X >= 0)
                    {
                        Force.X *= -1;
                    }

                    if (this.position.Y - corp.position.Y >= 0)
                    {
                        Force.Y *= -1;
                    }

                    Vector2 acceleration = Force / masse;

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