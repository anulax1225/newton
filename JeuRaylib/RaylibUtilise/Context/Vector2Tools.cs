/*******************************************************************************************
Projet Raylib pour l'atelier de première saison.
Auteur: Vinayak Ambigapathy
Date: Septembre 2023
********************************************************************************************/
using System.Numerics;
namespace Newton;
/// <summary>
/// Wrapper classe for static 2D vector utilses. 
/// </summary>
public class Vector2Tools
{
    /// <summary>
    /// Gives the magnitude of a 2D vector by 
    /// doing pitagor. Only works in the cartisienne plain.
    /// </summary>
    /// <param name="v">In vector</param>
    /// <returns>Magnitude of vector</returns>
    public static float Magnifie(Vector2 v)
    {
        float pitaRes = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
        return pitaRes;
    }
    /// <summary>
    /// Gives the normelized vector(Unite vector)
    /// </summary>
    /// <param name="v">In vector</param>
    /// <returns>Unite vector</returns>
    public static Vector2 Normelize(Vector2 v)
    {
        float magnitude = Magnifie(v);
        return new Vector2( v.X / magnitude, v.Y / magnitude);
    }
    /// <summary>
    /// Transition smoothly a list of 2D vector to another
    /// by averaging x and y position.
    /// </summary>
    /// <param name="ls1">First List of vectors</param>
    /// <param name="ls2">Second List of vectors</param>
    /// <returns>Smoothed list</returns>
    public static List<Vector2> SmoothTransitionList(List<Vector2> ls1, List<Vector2> ls2)
    {
        List<Vector2> lsResult = new List<Vector2>();
        int length = Math.Min(ls1.Count, ls2.Count);
        for (int i = 0; i < length; i++)
        {
            lsResult.Add(new Vector2((ls1[i].X + ls2[i].X) / 2, (ls1[i].Y + ls2[i].Y) / 2));
        }
        return lsResult;
    }
    /// <summary>
    /// gives a 2D vector from the average of 
    /// the 2D vector cordonete in the list 
    /// </summary>
    /// <param name="lsVectors">List of vectors to average</param>
    /// <returns>Averaged vector</returns>
    public static Vector2 Average(List<Vector2> lsVectors)
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
    /// <summary>
    /// Gives a 2D vector represanting the remainder 
    /// from the division of two vectors on the x and y axis. 
    /// </summary>
    /// <param name="v">vector to divide</param>
    /// <param name="div">scaler divider</param>
    /// <returns></returns>
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
}

