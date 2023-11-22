/*******************************************************************************************
Projet Raylib pour l'atelier de première saison.
Auteur: Vinayak Ambigapathy
Date: Septembre 2023
********************************************************************************************/
using Raylib_cs;
using System.Numerics;

namespace Newton;

/// <summary>
/// The container base class for most the UI 
/// </summary>
public abstract class Container2D : GameObject2D
{
    /// <summary>
    /// Size of the UI object font
    /// </summary>
    public int fontSize = 40;
    /// <summary>
    /// Size of the container
    /// </summary>
    public Vector2 size;
    /// <summary>
    /// Collision box
    /// </summary>
    protected Rectangle collisionBox;
    /// <summary>
    /// Generates the collision box
    /// </summary>
    /// <returns>Collision box</returns>
    protected Rectangle Generate()
    {
        return new Rectangle((int)this.position.X, (int)this.position.Y, (int)this.size.X, (int)this.size.Y);
    }
    /// <summary>
    /// Resizes the container
    /// </summary>
    /// <param name="newSize">New size</param>
    public void Resize(Vector2 newSize)
    {
        this.size = newSize;
        this.collisionBox = Generate();
    }
    /// <summary>
    /// Mouves the container
    /// </summary>
    /// <param name="newPosition">New position</param>
    public void Mouv(Vector2 newPosition)
    {
        this.position = newPosition;
        this.collisionBox = Generate();
    }
    /// <summary>
    /// Assesor to the collision box
    /// </summary>
    /// <returns>Collision box</returns>
    public Rectangle GetBorder()
    {
        return this.collisionBox;
    }
}
