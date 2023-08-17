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
    public class SpatialManager
    {
        public static int Main()
        {
            // Initialization
            //---------------------------------------------------------
            const int screenWidth = 1500;
            const int screenHeight = 1000;

            bool pause = false;

            int indexClick = -1;
            float textOffset = 30f;

            List<CorpMassif> lsCorpsMassifs = new List<CorpMassif>();
            Camera2D cam = new Camera2D();
            cam.offset = new Vector2(screenWidth/2, screenHeight/2);
            cam.rotation = 0.0f;
            cam.zoom = 0.5f;
            InitWindow(screenWidth, screenHeight, "La loie de Newton");

            // Set our game to run at 60 frames-per-second
            SetTargetFPS(60);
            //----------------------------------------------------------
            lsCorpsMassifs.Add(GenererCorp(0, 100, 50, 4, 0, 4.6f, Color.GOLD));
            lsCorpsMassifs.Add(GenererCorp(410, 100, 50, 46, 0.1f, 0.1f, Color.RED));
            // Main game loop
            while (!WindowShouldClose())
            {
                //cam.target = lsCorpsMassifs[1].position;
                //cam.target = GetAVGPos(lsCorpsMassifs);
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
                if (indexClick >= 0) 
                {
                    CorpMassif corp = lsCorpsMassifs[indexClick];
                    DrawText(String.Format("vitesse : {0}", corp.vitesse), (int)(corp.position.X + corp.radius + textOffset), (int)(corp.position.Y + corp.radius + textOffset), 40, Color.WHITE);
                    DrawText(String.Format("masse : {0}", corp.masse), (int)(corp.position.X + corp.radius + textOffset), (int)(corp.position.Y + corp.radius + textOffset*2), 40, Color.WHITE);

                }
                if (!pause)
                {
                    foreach (CorpMassif corp in lsCorpsMassifs)
                    {
                        corp.Graviter(lsCorpsMassifs);
                    }

                    foreach (CorpMassif corp in lsCorpsMassifs)
                    {
                        corp.ChangePosVitesse();
                    }
                    //Affiche les corp dans ma list
                }
                foreach (CorpMassif corp in lsCorpsMassifs)
                {
                    DrawLineV(corp.position, corp.vitesse*10 + corp.position, corp.color);
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

        private static int FindOnTarget(Vector2 vMouse, List<CorpMassif> lsCorpsMassifs)
        {
            Console.WriteLine("vMouse");
            Console.WriteLine(vMouse);
            int i = 0;
            foreach(CorpMassif corp in lsCorpsMassifs)
            {
                Console.WriteLine("{0}", corp.position);
                float NormevMouse = Vector2Normalize(vMouse) - Vector2Normalize(corp.position);
                if( NormevMouse <= corp.radius)
                {
                    return i;
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
    }

    public class CorpMassif
    {
        const float CONSTGRAVITATION = 1;
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
                    Vector2 vFab = new Vector2(1, 1);
                    Vector2 distance = corp.position - this.position;

                    float Norme = Vector2Normalize(distance);
                    float Fab = CONSTGRAVITATION * ((corp.masse)/Norme);//Avec une seul masse 
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

                    this.vitesse += acceleration;
                }
            }
        }

        public void ChangePosVitesse()
        {
            //Console.WriteLine("etoile {0} position {1} vitesse {2}", this.masse, this.position, this.vitesse);
            this.position += this.vitesse;
        }

        private static float Vector2Normalize(Vector2 v)
        {
            float pitaRes = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
            return pitaRes;
        }
    } 
}