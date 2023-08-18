/*******************************************************************************************
Projet Raylib pour l'atelier de première saison.
Auteur: Vinayak Ambigapathy
********************************************************************************************/

using System.CodeDom.Compiler;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Newton
{
    public class SpatialManager2D
    {
        public static int Main()
        {
            // Initialization
            //---------------------------------------------------------
            const int screenWidth = 1500;
            const int screenHeight = 1000;

            bool pause = false;

            int indexClick = -1;

            List<CorpMassif> lsCorpsMassifs = new List<CorpMassif>();

            Camera2D cam = new Camera2D();
            cam.offset = new Vector2(screenWidth/2, screenHeight/2);
            cam.rotation = 0.0f;
            cam.zoom = 0.5f;

            InitWindow(screenWidth, screenHeight, "La loie de Newton");

            // Set our game to run at 60 frames-per-second
            SetTargetFPS(60);
            //----------------------------------------------------------
            lsCorpsMassifs.Add(GenererCorp("Troulaxia", 0, -410, 50, 4, 4.6f, 0, Color.GOLD));
            lsCorpsMassifs.Add(GenererCorp("Moncul", 0, 0, 50, 46, 0.1f, 0.1f, Color.RED));
            // Main game loop
            while (!WindowShouldClose())
            {
                // Update
                //-----------------------------------------------------
                if (IsKeyPressed(KeyboardKey.KEY_SPACE))
                {
                    pause = !pause;
                }

                if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    indexClick = FindOnTarget(GetMousePosition(), lsCorpsMassifs);
                    Console.WriteLine("Boutton {0}", indexClick);
                }

                // Draw
                //-----------------------------------------------------
                BeginDrawing();
                BeginMode2D(cam);

                ClearBackground(Color.BLACK);
                ShowCommand();

                if (indexClick >= 0) 
                {
                    CorpMassif corp = lsCorpsMassifs[indexClick];
                    DrawParamInfo(corp);
                }

                if (!pause)
                {
                    MouvCorp(lsCorpsMassifs);
                }

                if (IsKeyDown(KeyboardKey.KEY_V))
                {
                    foreach (CorpMassif corp in lsCorpsMassifs)
                    {
                        DrawLineV(corp.position, corp.speed * 100 + corp.position, corp.color);
                    }
                }

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
        private static void ShowCommand()
        {
            DrawText("Enfoncé V pour voir les vecteurs", 0, 0, 40, Color.WHITE);
        }

        private static CorpMassif GenererCorp(string name, int x, int y, int radius, float masse, float vitX, float vitY, Color color)
        {
            Vector2 position = new Vector2(x, y);
            Vector2 vitesse = new Vector2(vitX, vitY);
            CorpMassif corp = new CorpMassif(name, position, vitesse, radius, masse, color);
            return corp;
        }

        private static void MouvCorp(List<CorpMassif> lsCorpsMassifs)
        {
            foreach (CorpMassif corp in lsCorpsMassifs)
            {
                corp.Graviter(lsCorpsMassifs);
            }

            foreach (CorpMassif corp in lsCorpsMassifs)
            {
                corp.ChangePosVitesse();
            }
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

        private static int FindOnTarget(Vector2 vMouse, List<CorpMassif> lsCorpsMassifs)
        {
            Vector2 vRelativeCheck = new Vector2(1500, 1000);
            bool isRelative = false;

            vMouse *= 2;
            vMouse.X -= 1500;
            vMouse.Y -= 1000;

            int i = 0;
            foreach(CorpMassif corp in lsCorpsMassifs)
            {
                Vector2 relativeDis = corp.position - vRelativeCheck;

                if (relativeDis.X > 0 || relativeDis.Y > 0)
                {
                    Console.WriteLine("oui");
                    vMouse += corp.position - vRelativeCheck;
                    isRelative = true;
                }
                float vMouseNorme = (float)Math.Sqrt(Math.Pow(Vector2Normalize(vMouse - corp.position), 2));

                if ( vMouseNorme <= corp.radius)
                {
                    return i;
                } 

                if (isRelative)
                {
                    vMouse -= corp.position - vRelativeCheck;
                    isRelative = false;
                }
                i++;
            }
            return -1;
        }

        private static float Vector2Normalize(Vector2 v)
        {
            float pitaRes = (float)Math.Sqrt(v.X * v.X +  v.Y * v.Y);
            return pitaRes;
        }

        private static void DrawParamInfo(CorpMassif corp)
        {
            float textOffset = 40f;
            DrawText(String.Format("Name : {0}", corp.name), (int)(corp.position.X + corp.radius + textOffset), (int)(corp.position.Y + corp.radius + textOffset), 40, Color.WHITE);
            DrawText(String.Format("Speed : {0}", (float)Math.Sqrt(Math.Pow(Vector2Normalize(corp.position), 2))), (int)(corp.position.X + corp.radius + textOffset), (int)(corp.position.Y + corp.radius + textOffset * 2), 40, Color.WHITE);
            DrawText(String.Format("Masse : {0}", corp.masse), (int)(corp.position.X + corp.radius + textOffset), (int)(corp.position.Y + corp.radius + textOffset * 3), 40, Color.WHITE);
        }
    }

    public class SpatialManager
    {
        
    }

    public class CorpMassif
    {
        const float CONSTGRAVITATION = 1;

        public string name;

        public Vector2 position;
        public Vector2 speed;

        public int radius;
        public float masse;

        public Color color;
        public CorpMassif(string name, Vector2 position, Vector2 vitesse, int radius, float masse, Color color)
        {
            this.name = name;
            this.position = position;
            this.speed = vitesse;
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
                    Vector2 vFab = new Vector2(1, 1);
                    Vector2 distance = corp.position - this.position;

                    float norme = Vector2Normalize(distance);
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

        private static float Vector2Normalize(Vector2 v)
        {
            float pitaRes = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
            return pitaRes;
        }
    } 
}