using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VectorUtilises
{
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
