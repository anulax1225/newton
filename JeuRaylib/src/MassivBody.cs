using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VectorUtilises;
using Raylib.RaylibUtile;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Newton
{
    public class MassiveBody : GameObject
    {
        const float CONSTGRAVITATION = 1f;
        public Vector2 speed;
        public int radius;
        public float surfaceG;
        public float masse;
        public bool nonGravity = false;
        public bool showVector = false;
        public bool showParams = false;
        private bool vectorChange = false;
        public MassiveBody(string name)
        {
            this.name = name;
            this.position = new Vector2(0,0);
            this.speed = new Vector2(0,0);
            this.surfaceG = 0;
            this.SetRadius(0);
        }
        public void SetRadius(int radius)
        {
            this.radius = radius;
            this.masse = (this.surfaceG * this.radius * this.radius) / CONSTGRAVITATION;
        }
        public void SetSurfaceGravity(float surfaceG)
        {
            this.surfaceG = surfaceG;
            this.masse = (this.surfaceG * this.radius * this.radius) / CONSTGRAVITATION;
        }
        public void Gravity(List<MassiveBody> lstBody, float timeStep)
        {
            foreach (MassiveBody body in lstBody)
            {
                if (body != this)
                {
                    Vector2 distance = body.position - this.position;
                    float norme = VectorTools.Vector2Normalize(distance);
                    float Fab = CONSTGRAVITATION * ((body.masse) / norme);//Avec une seul masse 
                    double angle = Math.Atan2(body.position.Y - this.position.Y, body.position.X - this.position.X);
                    Vector2 Force = new Vector2(Fab * (float)Math.Cos(angle), Fab * (float)Math.Sin(angle));
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
        public override bool CheckCollison(Vector2 ptn, Scene scene)
        {
            ptn *= scene.zoom;
            return CheckCollisionPointCircle(ptn, this.position, this.radius);
        }
        public override void Render(RenderManager rdManager)
        {
            Vector2 pos = rdManager.WorldToScreen(this.position / rdManager.scene.zoom);
            if (showParams)
            {
                float textOffset = 40f;
                int fontSize = 35;
                int textPosX = (int)((pos.X + this.radius) / rdManager.scene.zoom + textOffset);
                string[] paramsInfo =
                {
                    "Paramètre info",
                    String.Format("Name : {0}", this.name),
                    String.Format("Masse : {0}", this.masse),
                    String.Format("Radius : {0}", this.radius),
                    String.Format("Surface G : {0}", this.surfaceG),
                    String.Format("Speed : {0}", VectorTools.Vector2Normalize(this.speed)),
                };

                for(int i = 0; i < paramsInfo.Length; i++)
                {
                    DrawText(paramsInfo[i], (int)textPosX, (int)(pos.Y + textOffset * (i + 1)), fontSize, this.color);
                }
            }
            if (showVector)
            {
                DrawLineV(pos, pos + this.speed * 20, this.color);
            }
            DrawCircleV(pos, this.radius / rdManager.scene.zoom, this.color);
        }
        public override void OnEvent(InputHandler inController)
        {
            if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT) && IsKeyDown(KeyboardKey.KEY_V))
            {
                this.vectorChange = true;
                this.showVector = true;
                inController.isProtectedObj = true;
                inController.ProtectedObj = this;
            } 
            else if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT))
            {
                foreach (GameObject gameObject in inController.scene.lstGameObjects)
                {
                    if (gameObject.GetType() == typeof(MassiveBody) && gameObject != this)
                    { 
                        MassiveBody body = (MassiveBody)gameObject;
                        body.showParams = false;
                    }
                }
                this.showParams = !this.showParams;
                if (this.showParams) inController.LastActivation = this;
            }
            if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
            {
                this.vectorChange = false;
                this.showVector = false;
                inController.isProtectedObj = false;
                inController.ProtectedObj = null;
            }
            if (this.vectorChange) 
            { 
                this.speed = (this.position - inController.vMouse) / 100;
            }
        }
        public override string ToString()
        {
            return $"etoile {this.name} position {this.position} vitesse {this.speed}";
        }
    }
}

