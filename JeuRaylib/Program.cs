/*******************************************************************************************
*
*   raylib [shapes] example - bouncing ball
*
*   This example has been created using raylib 1.0 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Copyright (c) 2013 Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.CodeDom.Compiler;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Newton
{
    public class GravADeux
    {
        public static int Main()
        {
            // Initialization
            //---------------------------------------------------------
            const int screenWidth = 1500;
            const int screenHeight = 1000;

            List<CorpMassif> lsCorpsMassifs = new List<CorpMassif>();
            Camera2D cam = new Camera2D();
            cam.offset = new Vector2(screenWidth/2, screenHeight/2);
            cam.rotation = 0.0f;
            cam.zoom = 1.0f;
            InitWindow(screenWidth, screenHeight, "raylib [shapes] example - bouncing ball");

            // Set our game to run at 60 frames-per-second
            SetTargetFPS(60);
            //----------------------------------------------------------
            lsCorpsMassifs.Add(GenererCorp(100, 300, 12, 100, 1.4f, -0.5f, Color.GOLD));
            lsCorpsMassifs.Add(GenererCorp(520, 350, 32, 1000, 2f, 1f, Color.RED));
            // Main game loop
            while (!WindowShouldClose())
            {
                cam.target = lsCorpsMassifs[1].position;
                // Update
                //-----------------------------------------------------

                // Draw
                //-----------------------------------------------------
                BeginDrawing();
                BeginMode2D(cam);
                ClearBackground(Color.BLACK);
                foreach (CorpMassif corp in lsCorpsMassifs)
                {
                    corp.Graviter(lsCorpsMassifs);
                }

                foreach (CorpMassif corp in lsCorpsMassifs)
                {
                    corp.ChangePosVitesse();
                }
                //Affiche les corp dans ma list
                foreach (CorpMassif corp in lsCorpsMassifs)
                {
                    DrawCircleV(corp.position, corp.radius, corp.color);
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

        private static CorpMassif GenererCorp(int x, int y, int radius, float masse, float vitX, float vitY, Color color)
        {
            Vector2 position = new Vector2(x, y);
            Vector2 vitesse = new Vector2(vitX, vitY);
            CorpMassif corp = new CorpMassif(position, vitesse, radius, masse, color);
            return corp;
        }

        private static Vector2 GetAVGPos(List<CorpMassif> lsCorpsMassifs)
        {
            float x = 0, y = 0;
            int cpt = 0;
            foreach(CorpMassif corp in lsCorpsMassifs)
            {
                x += corp.position.X;
                y += corp.position.Y;
                cpt += 1;
            }
            return new Vector2(x/cpt, y/cpt);
        }
    }

    public class CorpMassif
    {
        const float CONSTGRAVITATION = 0.005f;
        public Vector2 position;
        public Vector2 vitesse;
        public int radius;
        public float masse;
        public Color color;
        public CorpMassif(Vector2 position, Vector2 vitesse, int radius, float masse, Color color)
        {
            this.position = position;
            this.vitesse = vitesse;
            this.radius = radius;
            this.masse = masse;
            this.color = color;
        }
        public void Graviter(List<CorpMassif> lsCorpsMassifs)
        {
            foreach(CorpMassif corp in lsCorpsMassifs)
            {
                if(corp != this)
                {
                    Vector2 vecFab = new Vector2(1, 1);
                    Vector2 distance = corp.position - this.position;

                    float Norme = (float)Math.Sqrt(distance.X * distance.X + distance.Y * distance.Y);// Math.Pow(value, puiss)
                    float Fab = CONSTGRAVITATION * ((corp.masse * this.masse)/Norme);
                    Vector2 Force = vecFab * Fab;

                    if (this.position.X - corp.position.X >= 0)
                    {
                        Force.X *= -1;
                    }

                    if (this.position.Y - corp.position.Y >= 0)
                    {
                        Force.Y *= -1;
                    }

                    Vector2 acceleration = Force / masse;

                    this.vitesse += acceleration;
                }
            }
        }

        public void ChangePosVitesse()
        {
            Console.WriteLine("etoile {0} position {1} vitesse {2}", this.masse, this.position, this.vitesse);
            this.position += this.vitesse;
        }
    } 
}