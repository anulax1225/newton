using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VectorUtilises;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Newton
{
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

        public MassiveBody(string name, Vector2 position, Vector2 speed, int radius, float surfaceG, Color color)
        {
            this.name = name;
            this.position = position;
            this.speed = speed;
            this.surfaceG = surfaceG;
            this.color = color;
            this.SetRadius(radius);
        }

        public void SetRadius(int radius)
        {
            this.radius = radius;
            this.masse = (this.surfaceG * this.radius * this.radius) / CONSTGRAVITATION;
        }

        public void SetSurfaceGravity(int surfaceG)
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

        public static List<MassiveBody> CopyList(List<MassiveBody> ls)
        {
            List<MassiveBody> newLs = new List<MassiveBody>();
            foreach (MassiveBody body in ls)
            {
                newLs.Add(new MassiveBody(body.name, body.position, body.speed, body.radius, body.surfaceG, body.color));
            }
            return newLs;
        }

        public override string ToString()
        {
            return $"etoile {this.name} position {this.position} vitesse {this.speed}";
        }
    }
}

