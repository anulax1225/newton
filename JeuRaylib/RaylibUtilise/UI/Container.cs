/*******************************************************************************************
Projet Raylib pour l'atelier de première saison.
Auteur: Vinayak Ambigapathy
Date: Septembre 2023
********************************************************************************************/
using Raylib_cs;
using System.Numerics;

namespace Raylib.RaylibUtiles;

/// <summary>
/// 
/// </summary>
public delegate void Callback();
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="input"></param>
public delegate void Validation<T>(T input);
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="input"></param>
/// <returns></returns>
public delegate bool Verifier<T>(T input);

/// <summary>
/// 
/// </summary>
public abstract class Container2D : GameObject2D
{
    /// <summary>
    /// 
    /// </summary>
    public int fontSize = 40;
    /// <summary>
    /// 
    /// </summary>
    public Vector2 size;
    /// <summary>
    /// 
    /// </summary>
    protected Rectangle border;
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected Rectangle Generate()
    {
        return new Rectangle((int)this.position.X, (int)this.position.Y, (int)this.size.X, (int)this.size.Y);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="newSize"></param>
    public void Resize(Vector2 newSize)
    {
        this.size = newSize;
        this.border = Generate();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="newPosition"></param>
    public void Mouv(Vector2 newPosition)
    {
        this.position = newPosition;
        this.border = Generate();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Rectangle GetBorder()
    {
        return this.border;
    }
}
