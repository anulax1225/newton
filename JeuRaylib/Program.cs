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
    public class Program
    {
        public static int Main()
        {
            GUISpatialManager2D spm = new GUISpatialManager2D();
            spm.start();
            return 0;
        }
    }

    public class GUISpatialManager2D
    {
        Camera2D cam = new Camera2D();
        const int screenWidth = 1500;
        const int screenHeight = 1000;
        bool camFixed = true;

        List<MassiveBody> lsMassiveBody = new List<MassiveBody>();


        private Vector2 WorldToScreen(Vector2 position)
        {
            Vector2 relativPos = position + cam.target;
            return relativPos;
        }

        private Vector2 ScreenToWorld(Vector2 position)
        {
            Vector2 relativPos = position - cam.target;
            return relativPos;
        }

        private MassiveBody GenerateBody(string name, int x, int y, int radius, float masse, float speedX, float speedY, Color color)
        {
            Vector2 position = new Vector2(x, y);
            Vector2 speed = new Vector2(speedX, speedY);
            MassiveBody body = new MassiveBody(name, position, speed, radius, masse, color);
            return body;
        }

        private void MouvBody()
        {
            foreach (MassiveBody body in this.lsMassiveBody)
            {
                body.Graviter(this.lsMassiveBody);
            }

            foreach (MassiveBody body in this.lsMassiveBody)
            {
                body.ChangePosVitesse();
            }
        }

        private int FindOnTarget(Vector2 vMouse)
        {
            vMouse *= 2;
            vMouse.X -= screenWidth;
            vMouse.Y -= screenHeight;

            int i = 0;
            foreach (MassiveBody body in lsMassiveBody)
            {
                float vMouseNorme = (float)Math.Sqrt(Math.Pow(VectorTools.Vector2Normalize(vMouse - body.position), 2));

                if (vMouseNorme <= body.radius)
                {
                    return i;
                }

                i++;
            }
            return -1;
        }

        private void MouvCamRelativ(int targetIndex)
        {
            if (targetIndex >= 0 && targetIndex < lsMassiveBody.Count() && camFixed)
            {
                Vector2 dest = WorldToScreen(lsMassiveBody[targetIndex].position) - cam.target;
                dest *= -1;
                foreach (MassiveBody body in lsMassiveBody)
                {
                    body.position += dest;
                }
                Console.WriteLine($"cam {cam.target} body {WorldToScreen(lsMassiveBody[targetIndex].position)}");
            }
        }

        private void DrawParamInfo(MassiveBody body)
        {
            float textOffset = 40f;
            Vector2 pos = WorldToScreen(body.position);
            DrawText(String.Format("Name : {0}", body.name), (int)(pos.X + body.radius + textOffset), (int)(pos.Y + body.radius + textOffset), 40, Color.WHITE);
            DrawText(String.Format("Speed : {0}", (float)Math.Sqrt(Math.Pow(VectorTools.Vector2Normalize(body.speed), 2))), (int)(pos.X + body.radius + textOffset), (int)(pos.Y + body.radius + textOffset * 2), 40, Color.WHITE);
            DrawText(String.Format("Masse : {0}", body.masse), (int)(pos.X + body.radius + textOffset), (int)(pos.Y + body.radius + textOffset * 3), 40, Color.WHITE);
        }
        private void ShowCommand()
        {
            DrawText("Test.", (int)(cam.offset.X / 2f * cam.zoom), -screenHeight, (int)(40 / cam.zoom), Color.WHITE);
            DrawText("Press V to see the bodys speed Vector.", (int)(cam.target.X - 1499), (int)(cam.target.Y / cam.zoom - 900), (int)(40 / cam.zoom), Color.WHITE);
            DrawText("Left click on a body to get additional info.", (int)(cam.target.X - 1499), (int)(cam.target.Y - 800), 40, Color.WHITE);
            DrawText("Press M to hide/show the menu.", (int)(cam.target.X - 1499), (int)(cam.target.Y - 700), 40, Color.WHITE);
        }

        public int start()
        {
            // Variables
            //---------------------------------------------------------
            bool isHidden = false;
            bool pause = false;
            int indexClick = -1;
            int camTarget = -1;

            // Initialization
            //---------------------------------------------------------
            cam.offset = new Vector2(screenWidth / 2, screenHeight / 2);
            cam.zoom = 0.5f;
            InitWindow(screenWidth, screenHeight, "La loie de Newton");
            // Set our game to run at 60 frames-per-second
            SetTargetFPS(60);

            // Creating objects of the game
            //----------------------------------------------------------
            lsMassiveBody.Add(GenerateBody("lune corva", 0, 520, 5, 1f, -9f, 0, Color.VIOLET));
            lsMassiveBody.Add(GenerateBody("Troulaxia", 0, -450, 18, 4, 4.6f, 0, Color.GOLD));
            lsMassiveBody.Add(GenerateBody("Moncul", 0, 0, 50, 46, 0.1f, 0.1f, Color.RED));

            // Main game loop
            while (!WindowShouldClose())
            {
                // Update
                //-----------------------------------------------------
                MouvCamRelativ(camTarget);
                float mouseWheelScroll = GetMouseWheelMove();
                if (mouseWheelScroll > 0)
                {
                    cam.zoom += 0.1f;
                }
                if (mouseWheelScroll < 0)
                {
                    cam.zoom -= 0.1f;
                }

                if (IsKeyPressed(KeyboardKey.KEY_SPACE))
                {
                    pause = !pause;
                }

                if (IsKeyPressed(KeyboardKey.KEY_M))
                {
                    isHidden = !isHidden;
                }

                if (IsKeyPressed(KeyboardKey.KEY_A))
                {
                    camTarget = indexClick;
                    camFixed = !camFixed;
                }

                if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    indexClick = FindOnTarget(GetMousePosition());
                    Console.WriteLine("Boutton {0}", indexClick);
                }

                // Draw
                //-----------------------------------------------------
                BeginDrawing();
                BeginMode2D(cam);
                ClearBackground(Color.BLACK);

                if (!isHidden)
                {
                    ShowCommand();
                }
                if (indexClick >= 0)
                {
                    MassiveBody body = lsMassiveBody[indexClick];
                    DrawParamInfo(body);
                }
                if (!pause)
                {
                    MouvBody();
                }

                if (IsKeyDown(KeyboardKey.KEY_V))
                {
                    foreach (MassiveBody body in lsMassiveBody)
                    {
                        DrawLineV(WorldToScreen(body.position), body.speed * 10 + WorldToScreen(body.position), body.color);
                    }
                }


                foreach (MassiveBody body in lsMassiveBody)
                {
                    DrawCircleV(WorldToScreen(body.position), body.radius, body.color);
                }

                EndMode2D();
                EndDrawing();
                //-----------------------------------------------------
            }
            // De-Initialization
            //---------------------------------------------------------
            CloseWindow();
            //----------------------------------------------------------
            return 0;
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
            foreach(MassiveBody corp in lsCorpsMassifs)
            {
                if(corp != this)
                {
                    Vector2 vFab = new Vector2(1, 1);
                    Vector2 distance = corp.position - this.position;

                    float norme = VectorTools.Vector2Normalize(distance);
                    float Fab = CONSTGRAVITATION * ((corp.masse)/norme);//Avec une seul masse 
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

        public void ChangePosVitesse()
        {
            //Console.WriteLine("etoile {0} position {1} vitesse {2}", this.masse, this.position, this.vitesse);
            this.position += this.speed;
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
    }
}